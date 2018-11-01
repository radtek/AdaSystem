using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Finance;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Finance;
using Ada.Core.ViewModel.Purchase;
using Ada.Framework.Filter;
using Ada.Services.Finance;
using Ada.Services.Purchase;
using Newtonsoft.Json;

namespace Finance.Controllers
{
    /// <summary>
    /// 采购请款
    /// </summary>
    public class PurchasePayController : BaseController
    {
        private readonly IBillPaymentService _billPaymentService;
        private readonly IRepository<PurchasePaymentDetail> _repository;
        private readonly IPurchasePaymentDetailService _purchasePaymentDetailService;
        public PurchasePayController(IBillPaymentService billPaymentService,
            IRepository<PurchasePaymentDetail> repository,
            IPurchasePaymentDetailService purchasePaymentDetailService)
        {
            _billPaymentService = billPaymentService;
            _repository = repository;
            _purchasePaymentDetailService = purchasePaymentDetailService;
        }
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult PayInfo(string id)
        {
            var payment = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("EditPayInfo", payment);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditPay(PurchasePaymentDetailView view)
        {
            var payment = _repository.LoadEntities(d => d.Id == view.Id).FirstOrDefault();
            payment.AccountBank = view.AccountBank;
            payment.AccountName = view.AccountName;
            payment.AccountNum = view.AccountNum;
            _purchasePaymentDetailService.Update(payment);
            return Json(new { State = 1, Msg = "修改成功" });
        }
        public ActionResult Pay(string id)
        {
            var payment = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            BillPaymentView viewModel = new BillPaymentView();
            viewModel.BillDate = DateTime.Now;
            viewModel.AccountBank = payment.AccountBank;
            viewModel.AccountName = payment.AccountName;
            viewModel.AccountNum = payment.AccountNum;
            viewModel.PayMoney = payment.PayMoney;
            viewModel.RequestNum = id;
            viewModel.PaymentType = payment.PaymentType;
            viewModel.IsInvoice = payment.PurchasePayment.IsInvoice;
            viewModel.InvoiceTitle = payment.PurchasePayment.InvoiceTitle;

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pay(BillPaymentView viewModel)
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
            var payment = _repository.LoadEntities(d => d.Id == viewModel.RequestNum).FirstOrDefault();
            if (payment.Status == Consts.StateNormal)
            {
                ModelState.AddModelError("message", "此请款已生成付款单据！");
                return View(viewModel);
            }
            BillPayment entity = new BillPayment();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedById = CurrentManager.Id;
            entity.Transactor = CurrentManager.UserName;
            entity.TransactorId = CurrentManager.Id;
            entity.AccountBank = viewModel.AccountBank;
            entity.AccountName = viewModel.AccountName;
            entity.AccountNum = viewModel.AccountNum;
            entity.BillDate = viewModel.BillDate;
            entity.BillNum = IdBuilder.CreateOrderNum("FK");
            entity.Image = viewModel.Image;
            entity.LinkManId = payment.PurchasePayment.LinkManId;
            entity.LinkManName = payment.PurchasePayment.LinkManName;
            entity.PaymentType = viewModel.PaymentType;
            entity.RequestNum = viewModel.RequestNum;//这里是申请的ID号
            entity.RequestType = Consts.StateLock;//采购付款单
            entity.Remark = viewModel.Remark;
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
            if (money > payment.PayMoney)
            {
                ModelState.AddModelError("message", "付款金额已超出申请金额！");
                return View(viewModel);
            }
            //修改付款状态
            payment.Status = Consts.StateNormal;
            _billPaymentService.Add(entity);
            TempData["Msg"] = "付款成功";
            return RedirectToAction("Index");
        }
    }
}