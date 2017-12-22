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
using Newtonsoft.Json;

namespace Finance.Controllers
{
    /// <summary>
    /// 媒介发票
    /// </summary>
    public class PurchaseInvoiceController : BaseController
    {
        private readonly IPurchasePaymentService _purchasePaymentService;
        private readonly IRepository<PurchasePayment> _repository;
        public PurchaseInvoiceController(IPurchasePaymentService purchasePaymentService,
            IRepository<PurchasePayment> repository)
        {
            _purchasePaymentService = purchasePaymentService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Update(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            //PurchasePaymentView viewModel = new PurchasePaymentView();
            //viewModel.BillDate = entity.BillDate;
            //viewModel.Transactor = entity.Transactor;
            //viewModel.TransactorId = entity.TransactorId;
            //viewModel.PayMoney = entity.PayMoney;
            //viewModel.InvoiceTitle = entity.InvoiceTitle;
            //viewModel.InvoiceDate = entity.InvoiceDate;
            //viewModel.InvoiceNum = entity.InvoiceNum;
            //viewModel.InvoiceStauts = entity.InvoiceStauts ?? false;
            //viewModel.DiscountMoney = entity.DiscountMoney;
            //viewModel.Tax = entity.Tax;
            //viewModel.Id = id;
            //ViewBag.OrderDetails = entity.PurchasePaymentOrderDetails;
            //ViewBag.PayDetails = entity.PurchasePaymentDetails;
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(PurchasePaymentView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            
            PurchasePayment payment = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            payment.InvoiceNum = viewModel.InvoiceNum;
            payment.InvoiceStauts = viewModel.InvoiceStauts;
            payment.InvoiceDate = viewModel.InvoiceDate;
            payment.ModifiedBy = CurrentManager.UserName;
            payment.ModifiedById = CurrentManager.Id;
            payment.ModifiedDate = DateTime.Now;

            _purchasePaymentService.Update(payment);
            TempData["Msg"] = "保存成功";
            return RedirectToAction("Index");
        }
    }
}