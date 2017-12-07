using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Purchase;
using Ada.Framework.Filter;
using Ada.Services.Purchase;

namespace Purchase.Controllers
{
    /// <summary>
    /// 媒介请款
    /// </summary>
    public class PaymentController : BaseController
    {
        private readonly IPurchasePaymentService _purchasePaymentService;
        private readonly IRepository<PurchasePayment> _repository;
        public PaymentController(IPurchasePaymentService purchasePaymentService,
            IRepository<PurchasePayment> repository)
        {
            _purchasePaymentService = purchasePaymentService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(PurchasePaymentView viewModel)
        {
            var result = _purchasePaymentService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new PurchasePaymentView
                {
                    Id = d.Id,
                    Status = d.Status,
                    LinkManName = d.LinkManName,
                    AccountBank=d.AccountBank,
                    AccountName = d.AccountName,
                    AccountNum = d.AccountNum,
                    ApplicationNum = d.ApplicationNum,
                    PaymentType = d.PaymentType,
                    AuditStatus = d.AuditStatus,
                    ApplicationDate=d.ApplicationDate,
                    Transactor = d.Transactor,
                    PayMoney = d.PayMoney
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            PurchasePaymentView viewModel = new PurchasePaymentView();
            viewModel.ApplicationDate = DateTime.Now;
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
            viewModel.PayMoney = 0;
            return View(viewModel);
        }
        //public ActionResult Details(string id)
        //{
        //    var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
        //    var payments = entity.PurchasePaymentDetails.Select(d => new PurchasePaymentDetailsView
        //    {
        //        ApplicationNum = d.ApplicationNum,
        //        AccountName = d.AccountName,
        //        AccountBank = d.AccountBank,
        //        AccountNum = d.AccountNum,
        //        PayMoney = d.PayMoney,
        //        AuditStatus = d.AuditStatus,
        //        PaymentType = d.PaymentType,
        //        ApplicationDate = d.ApplicationDate,
        //        Status = d.Status

        //    });
        //    return PartialView("Payments", payments);
        //}

        public ActionResult OrderDetails()
        {
            return PartialView("OrderDetails");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(PurchasePaymentView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            
            PurchasePayment payment = new PurchasePayment();
            payment.Id = IdBuilder.CreateIdNum();
            payment.AccountBank = viewModel.AccountBank;
            payment.AccountName = viewModel.AccountName;
            payment.AccountNum = viewModel.AccountNum;
            payment.ApplicationDate = DateTime.Now;
            payment.AuditStatus = Consts.StateLock;//待审核
            payment.PayMoney = viewModel.PayMoney;
            payment.PaymentType = viewModel.PaymentType;
            payment.Transactor = viewModel.Transactor;
            payment.TransactorId = viewModel.TransactorId;
            payment.Status = Consts.StateLock;//待付款
            payment.AddedBy = CurrentManager.UserName;
            payment.AddedById = CurrentManager.Id;
            payment.AddedDate = DateTime.Now;
            payment.ApplicationNum = IdBuilder.CreateOrderNum("QK");
            payment.ApplicationDate = viewModel.ApplicationDate;
            //_purchasePaymentService.Add(payment);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            //_businessPayeeService.Delete(entity);//物理删除
            return Json(new { State = 1, Msg = "撤销成功" });
        }
    }
}