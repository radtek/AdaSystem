using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Finance;
using Ada.Core.Domain.Purchase;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Finance;
using Ada.Framework.Filter;
using Ada.Services.Finance;
using Newtonsoft.Json;

namespace Finance.Controllers
{
    /// <summary>
    /// 付款单据
    /// </summary>
    public class BillPaymentController : BaseController
    {
        private readonly IBillPaymentService _billPaymentService;
        private readonly IRepository<BillPayment> _repository;
        private readonly IRepository<BillPaymentDetail> _billPaymentDetailrepository;
        private readonly IRepository<BusinessPayment> _businessPaymentRepository;
        private readonly IRepository<PurchasePaymentDetail> _purchasePaymentDetailRepository;
        public BillPaymentController(IBillPaymentService billPaymentService, 
            IRepository<BillPayment> repository,
            IRepository<BillPaymentDetail> billPaymentDetailrepository,
            IRepository<BusinessPayment> businessPaymentRepository,
            IRepository<PurchasePaymentDetail> purchasePaymentDetailRepository)
        {
            _billPaymentService = billPaymentService;
            _repository = repository;
            _billPaymentDetailrepository = billPaymentDetailrepository;
            _businessPaymentRepository = businessPaymentRepository;
            _purchasePaymentDetailRepository = purchasePaymentDetailRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BillPaymentView viewModel)
        {
            var result = _billPaymentService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BillPaymentView
                {
                    Id = d.Id,
                    Transactor = d.Transactor,
                    AccountName = d.AccountName,
                    AccountNum = d.AccountNum,
                    AccountBank = d.AccountBank,
                    PayMoney = d.BillPaymentDetails.Sum(b=>b.Money),
                    BillNum = d.BillNum,
                    PaymentType = d.PaymentType,
                    BillDate = d.BillDate,
                    Image = d.Image,
                    PayInfo = string.Join(",",d.BillPaymentDetails.Select(p=>p.SettleAccount.SettleName))
                    
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Update(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            BillPaymentView entity = new BillPaymentView();
            entity.Id = item.Id;
            entity.BillDate = item.BillDate;
            entity.BillNum = item.BillNum;
            entity.PaymentType = item.PaymentType;
            entity.AccountBank = item.AccountBank;
            entity.AccountName = item.AccountName;
            entity.AccountNum = item.AccountNum;
            entity.Image = item.Image;
            entity.ThumbnailImage = Thumbnail.MakeThumbnailImageToBase64(Utils.GetMapPath(item.Image));
            var paydetails = item.BillPaymentDetails.Where(d => d.IsDelete == false).Select(d => new
            {
                d.Id,
                d.SettleAccountId,
                d.IncomeExpendId,
                d.Money
            });
            entity.PayDetails = JsonConvert.SerializeObject(paydetails);

            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(BillPaymentView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var payDetails = JsonConvert.DeserializeObject<List<BillPaymentDetail>>(viewModel.PayDetails);
            if (payDetails.Count <= 0)
            {
                ModelState.AddModelError("message", "请录入结算账户！");
                return View(viewModel);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.PaymentType = viewModel.PaymentType;
            entity.AccountBank = viewModel.AccountBank;
            entity.AccountName = viewModel.AccountName;
            entity.AccountNum = viewModel.AccountNum;
            entity.Image = viewModel.Image;
            //删除
            _billPaymentDetailrepository.Remove(payDetails.Select(d=>d.Id).ToArray());
            //新增
            decimal? money = 0;
            foreach (var billPaymentDetail in payDetails)
            {
                if (string.IsNullOrWhiteSpace(billPaymentDetail.IncomeExpendId) && string.IsNullOrWhiteSpace(billPaymentDetail.SettleAccountId))
                {
                    ModelState.AddModelError("message", "收入项目或结算账户不能为空！");
                    return View(viewModel);
                }
                billPaymentDetail.Id = IdBuilder.CreateIdNum();
                billPaymentDetail.AddedBy = CurrentManager.UserName;
                billPaymentDetail.AddedById = CurrentManager.Id;
                billPaymentDetail.AddedDate = DateTime.Now;
                money += billPaymentDetail.Money;
                entity.BillPaymentDetails.Add(billPaymentDetail);
            }
            decimal? paymoney = 0;
            if (entity.RequestType==Consts.StateNormal)//销售付款单据
            {
                var payment = _businessPaymentRepository.LoadEntities(d => d.ApplicationNum == entity.RequestNum).FirstOrDefault();
                paymoney = payment.PayMoney;
            }
            if (entity.RequestType == Consts.StateLock)//媒介付款单据
            {
                var payment = _purchasePaymentDetailRepository.LoadEntities(d => d.Id == entity.RequestNum).FirstOrDefault();
                paymoney = payment.PayMoney;
            }
            if (money > paymoney)
            {
                ModelState.AddModelError("message", "付款金额已超出申请金额！");
                return View(viewModel);
            }
            _billPaymentService.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            //请款单据恢复
            if (entity.RequestType == Consts.StateNormal)//销售付款单据
            {
                var payment = _businessPaymentRepository.LoadEntities(d => d.ApplicationNum == entity.RequestNum).FirstOrDefault();
                payment.Status=Consts.StateLock;
            }
            if (entity.RequestType == Consts.StateLock)//媒介付款单据
            {
                var payment = _purchasePaymentDetailRepository.LoadEntities(d => d.Id == entity.RequestNum).FirstOrDefault();
                payment.Status=Consts.StateLock;
            }
            _billPaymentService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}