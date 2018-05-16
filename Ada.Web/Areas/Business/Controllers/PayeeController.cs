using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Business;
using Ada.Services.Finance;

namespace Business.Controllers
{

    /// <summary>
    /// 领款记录  撤销  查看 请款申请
    /// </summary>
    public class PayeeController : BaseController
    {
        private readonly IBusinessPayeeService _businessPayeeService;
        private readonly IBusinessPaymentService _businessPaymentService;
        private readonly IRepository<BusinessPayee> _repository;
        private readonly IBillPaymentService _billPaymentService;
        public PayeeController(IBusinessPayeeService businessPayeeService,
            IRepository<BusinessPayee> repository,
            IBusinessPaymentService businessPaymentService,
            IBillPaymentService billPaymentService)
        {
            _businessPayeeService = businessPayeeService;
            _repository = repository;
            _businessPaymentService = businessPaymentService;
            _billPaymentService = billPaymentService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessPayeeView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _businessPayeeService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessPayeeView
                {
                    Id = d.Id,
                    LinkManName = d.LinkManName,
                    LinkManId = d.LinkManId,
                    Transactor = d.Transactor,
                    Money = d.Money,
                    ClaimDate = d.ClaimDate,
                    PayInfo = d.Receivables.AccountName,
                    ReceivableInfo = d.Receivables.SettleAccount.SettleName,
                    VerificationStatus = d.VerificationStatus,
                    VerificationMoney = d.VerificationStatus==Consts.StateNormal?0: d.Money - d.BusinessPayments.Where(p => p.AuditStatus == Consts.StateNormal).Sum(p => p.PayMoney),
                    PaymentCount = d.BusinessPayments.Count
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Payment(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            BusinessPaymentView viewModel = new BusinessPaymentView();
            viewModel.BusinessPayeeId = id;
            viewModel.PayMoney = 0;
            viewModel.LinkmanName = entity.LinkManName;
            var temp = entity.BusinessPayments.Sum(d => d.PayMoney);
            viewModel.TotalMoney = entity.Money - temp;
            return View(viewModel);
        }
        public ActionResult Payments(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            var payments = entity.BusinessPayments.Select(d => new BusinessPaymentView
            {
                ApplicationNum = d.ApplicationNum,
                AccountName = d.AccountName,
                AccountBank = d.AccountBank,
                AccountNum = d.AccountNum,
                PayMoney = d.PayMoney,
                AuditStatus = d.AuditStatus,
                PaymentType = d.PaymentType,
                ApplicationDate = d.ApplicationDate,
                Status = d.Status,
                PayImage = _billPaymentService.GetByRequestNum(d.ApplicationNum)?.Image

            });
            return PartialView("Payments", payments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(BusinessPaymentView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            if (string.IsNullOrWhiteSpace(viewModel.Image))
            {
                ModelState.AddModelError("message", "申请凭证不能为空");
                return View(viewModel);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.BusinessPayeeId).FirstOrDefault();
            if (entity.VerificationStatus == Consts.StateNormal)
            {
                ModelState.AddModelError("message", "此款项已核销，无法请款");
                return View(viewModel);
            }
            var temp = entity.BusinessPayments.Sum(d => d.PayMoney);
            //校验金额不能超出领款金额
            if (viewModel.PayMoney > entity.Money - temp)
            {
                ModelState.AddModelError("message", "申请金额超出领款金额");
                return View(viewModel);
            }
            BusinessPayment payment = new BusinessPayment();
            payment.Id = IdBuilder.CreateIdNum();
            payment.AccountBank = viewModel.AccountBank;
            payment.AccountName = viewModel.AccountName;
            payment.AccountNum = viewModel.AccountNum;
            payment.ApplicationDate = DateTime.Now;
            payment.AuditStatus = Consts.StateLock;//待审核
            payment.PayMoney = viewModel.PayMoney;
            payment.BusinessPayeeId = viewModel.BusinessPayeeId;
            payment.PaymentType = viewModel.PaymentType;
            payment.Image = viewModel.Image;
            payment.Remark = viewModel.Remark;
            payment.Transactor = CurrentManager.UserName;
            payment.TransactorId = CurrentManager.Id;
            payment.Status = Consts.StateLock;//待付款
            payment.AddedBy = CurrentManager.UserName;
            payment.AddedById = CurrentManager.Id;
            payment.AddedDate = DateTime.Now;
            payment.ApplicationNum = IdBuilder.CreateOrderNum("QK");
            _businessPaymentService.Add(payment);

            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.BusinessPayments.Count > 0)//如果有申请单，就不能撤销
            {
                return Json(new { State = 0, Msg = "此单据有请款记录，无法撤销" });
            }
            if (entity.BusinessWriteOffs.Count > 0)//如果有核销，就不能撤销
            {
                return Json(new { State = 0, Msg = "此单据有核销记录，无法撤销" });
            }
            entity.Receivables.BalanceMoney = entity.Receivables.BalanceMoney + entity.Money;
            _businessPayeeService.Delete(entity);//物理删除
            return Json(new { State = 1, Msg = "撤销成功" });
        }
    }
}