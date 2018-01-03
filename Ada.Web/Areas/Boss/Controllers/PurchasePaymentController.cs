using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Customer;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Purchase;
using Ada.Framework.Filter;
using Ada.Services.Finance;
using Ada.Services.Purchase;

namespace Boss.Controllers
{
    public class PurchasePaymentController : BaseController
    {
        private readonly IPurchasePaymentDetailService _purchasePaymentDetailService;
        private readonly IRepository<PurchasePaymentDetail> _repository;
        private readonly IBillPaymentService _billPaymentService;
        public PurchasePaymentController(IPurchasePaymentDetailService purchasePaymentDetailService, IRepository<PurchasePaymentDetail> repository, IBillPaymentService billPaymentService)
        {
            _purchasePaymentDetailService = purchasePaymentDetailService;
            _repository = repository;
            _billPaymentService = billPaymentService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(PurchasePaymentDetailView viewModel)
        {
            var result = _purchasePaymentDetailService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new PurchasePaymentDetailView
                {
                    Id = d.Id,
                    AccountBank = d.AccountBank,
                    Transactor = d.PurchasePayment.Transactor,
                    AccountName = d.AccountName,
                    AccountNum = d.AccountNum,
                    PayMoney = d.PayMoney,
                    BillNum = d.PurchasePayment.BillNum,
                    PaymentType = d.PaymentType,
                    BillDate = d.PurchasePayment.BillDate,
                    LinkManName = d.PurchasePayment.LinkManName,
                    AuditStatus = d.AuditStatus
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Audit(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            PurchasePaymentDetailView viewModel = new PurchasePaymentDetailView();
            viewModel.Transactor = entity.PurchasePayment.Transactor;
            viewModel.Id = entity.Id;
            //viewModel.ApplicationDate = entity.ApplicationDate;
            viewModel.AccountBank = entity.AccountBank;
            viewModel.AccountName = entity.AccountName;
            viewModel.AccountNum = entity.AccountNum;
            viewModel.PaymentType = entity.PaymentType;
            viewModel.PayMoney = entity.PayMoney;
            viewModel.AuditStatus = entity.AuditStatus;
            viewModel.IsInvoice = entity.PurchasePayment.IsInvoice;
            viewModel.InvoiceTitle = entity.PurchasePayment.InvoiceTitle;
            viewModel.OrderMoney = entity.PurchasePayment.PurchasePaymentOrderDetails.Sum(d => d.PurchaseOrderDetail.Money);
            viewModel.DiscountMoney = entity.PurchasePayment.DiscountMoney;
            viewModel.TotalPayMoney = entity.PurchasePayment.PurchasePaymentDetails.Sum(d => d.PayMoney);
            
            ViewBag.LinkMan = entity.PurchasePayment.LinkMan;
            ViewBag.OrderDetail = entity.PurchasePayment.PurchasePaymentOrderDetails.ToList();
            ViewBag.PayDetail = entity.PurchasePayment.PurchasePaymentDetails.ToList();
            var account = entity.PurchasePayment.LinkMan.PayAccounts.FirstOrDefault(d =>
                d.AccountName.Equals(viewModel.AccountName, StringComparison.CurrentCultureIgnoreCase) &&
                d.AccountNum.Equals(viewModel.AccountNum, StringComparison.CurrentCultureIgnoreCase)
                && d.IsDelete == false);
            if (account == null)
            {
                viewModel.WarningMsg = "注：此请款账户不在相应供应商的账户列表中。";
            }
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Audit(PurchasePaymentDetailView viewModel)
        {
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.AuditStatus = viewModel.AuditStatus;
            entity.AuditBy = CurrentManager.UserName;
            entity.AuditById = CurrentManager.Id;
            entity.AuditDate = DateTime.Now;
            //通过就增加客户账户信息
            if (entity.AuditStatus == Consts.StateNormal)
            {
                //判断是否增加账户
                var account = entity.PurchasePayment.LinkMan.PayAccounts.FirstOrDefault(d =>
                    d.AccountName.Equals(viewModel.AccountName, StringComparison.CurrentCultureIgnoreCase) &&
                    d.AccountNum.Equals(viewModel.AccountNum, StringComparison.CurrentCultureIgnoreCase)
                    && d.IsDelete == false);
                if (account == null)
                {
                    PayAccount payAccount = new PayAccount();
                    payAccount.Id = IdBuilder.CreateIdNum();
                    payAccount.AccountName = viewModel.AccountName;
                    payAccount.AccountNum = viewModel.AccountNum;
                    payAccount.AccountType = viewModel.AccountBank;
                    payAccount.AddedBy = CurrentManager.UserName;
                    payAccount.AddedById = CurrentManager.Id;
                    payAccount.AddedDate = DateTime.Now;
                    entity.PurchasePayment.LinkMan.PayAccounts.Add(payAccount);
                }
            }
            else
            {
                //判断是否已经付款了
                var bill = _billPaymentService.GetByRequestNum(entity.Id);
                if (bill != null)
                {
                    ModelState.AddModelError("message", "此申请，已生成了付款单！");
                    ViewBag.LinkMan = entity.PurchasePayment.LinkMan;
                    ViewBag.OrderDetail = entity.PurchasePayment.PurchasePaymentOrderDetails.ToList();
                    ViewBag.PayDetail = entity.PurchasePayment.PurchasePaymentDetails.ToList();
                    return View(viewModel);
                }
            }
            _purchasePaymentDetailService.Update(entity);
            TempData["Msg"] = "审批成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            //付款了的不能清除
            if (entity.Status == Consts.StateNormal)
            {
                return Json(new { State = 0, Msg = "此申请已付款，无法删除" });
            }
            _purchasePaymentDetailService.Delete(entity);//物理删除
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}