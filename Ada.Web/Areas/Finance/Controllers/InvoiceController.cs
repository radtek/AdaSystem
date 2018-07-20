using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Business;
using Newtonsoft.Json;

namespace Finance.Controllers
{
    /// <summary>
    /// 销售开票
    /// </summary>
    public class InvoiceController : BaseController
    {
        private readonly IBusinessInvoiceService _businessInvoiceService;
        private readonly IRepository<BusinessInvoice> _repository;
        private readonly IRepository<BusinessInvoiceDetail> _businessInvoiceDetailrepository;
        public InvoiceController(IBusinessInvoiceService businessInvoiceService,
            IRepository<BusinessInvoice> repository,
            IRepository<BusinessInvoiceDetail> businessInvoiceDetailrepository)
        {
            _businessInvoiceService = businessInvoiceService;
            _repository = repository;
            _businessInvoiceDetailrepository = businessInvoiceDetailrepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Update(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            BusinessInvoiceView viewModel = new BusinessInvoiceView();
            viewModel.Id = id;
            viewModel.Transactor = entity.Transactor;
            viewModel.LinkManName = entity.LinkManName;
            viewModel.InvoiceTitle = entity.InvoiceTitle;
            viewModel.InvoiceType = entity.InvoiceType;
            viewModel.Address = entity.Address;
            viewModel.Bank = entity.Bank;
            viewModel.BankNum = entity.BankNum;
            viewModel.Company = entity.Company;
            viewModel.Phone = entity.Phone;
            viewModel.TaxNum = entity.TaxNum;
            viewModel.TotalMoney = entity.TotalMoney;
            viewModel.Remark = entity.Remark;
            viewModel.Status = entity.Status;
            //viewModel.MoneyStatus = entity.MoneyStatus;
            viewModel.InvoiceTime = entity.InvoiceTime ?? DateTime.Now;
            viewModel.InvoiceNum = entity.InvoiceNum;
            viewModel.PayTime = entity.PayTime;
            viewModel.BusinessInvoiceDetails = entity.BusinessInvoiceDetails;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(BusinessInvoiceView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var invoice = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            invoice.Status = viewModel.Status;
            //invoice.MoneyStatus = viewModel.MoneyStatus;
            invoice.InvoiceNum = viewModel.InvoiceNum;
            invoice.InvoiceTime = viewModel.InvoiceTime;
            invoice.PayTime = viewModel.PayTime;
            invoice.ReceivableNum = viewModel.ReceivableNum;
            invoice.Remark = viewModel.Remark;
            invoice.TotalMoney = viewModel.TotalMoney;
            invoice.TaxNum = viewModel.TaxNum;
            invoice.Company = viewModel.Company;
            invoice.ModifiedBy = CurrentManager.UserName;
            invoice.ModifiedById = CurrentManager.Id;
            invoice.ModifiedDate = DateTime.Now;
            _businessInvoiceService.Update(invoice);
            TempData["Msg"] = "开票成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.Receivableses.Any())
            {
                return Json(new { State = 0, Msg = "此发票已核销，无法删除" });
            }
            _businessInvoiceDetailrepository.Remove(entity.BusinessInvoiceDetails);
            _businessInvoiceService.Delete(entity);//物理删除
            return Json(new { State = 1, Msg = "撤销成功" });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult CancleWriteOff(string id)
        {
            _businessInvoiceService.CancleWriteOff(id);
            return Json(new { State = 1, Msg = "撤销核销成功" });
        }
    }
}