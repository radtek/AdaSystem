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

namespace Business.Controllers
{
    /// <summary>
    /// 销售发票
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
        public ActionResult GetList(BusinessInvoiceView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _businessInvoiceService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessInvoiceView
                {
                    Id = d.Id,
                    LinkManName = d.LinkManName,
                    Transactor = d.Transactor,
                    AddedDate = d.AddedDate,
                    TotalMoney = d.TotalMoney,
                    Status = d.Status,
                    MoneyStatus = d.MoneyStatus,
                    InvoiceTitle = d.InvoiceTitle,
                    InvoiceType = d.InvoiceType,
                    Company = d.Company,
                    TaxNum = d.InvoiceTitle + d.Company,
                    InvoiceTime = d.InvoiceTime,
                    InvoiceNum = d.InvoiceNum,
                    PayTime = d.PayTime,
                    TaxMoney = d.BusinessInvoiceDetails.Sum(o => o.BusinessOrder.BusinessOrderDetails.Sum(b => b.TaxMoney))

                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OrderDetails()
        {
            return PartialView("OrderDetails");
        }
        public ActionResult InvoiceDetails(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("InvoiceDetails", entity);
        }
        public ActionResult Add()
        {
            BusinessInvoiceView viewModel = new BusinessInvoiceView();
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
            viewModel.TotalMoney = 0;
            viewModel.AuditStatus = Consts.StateLock;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(BusinessInvoiceView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var details = JsonConvert.DeserializeObject<List<BusinessInvoiceDetail>>(viewModel.OrderDetails);
            if (details.Count <= 0)
            {
                ModelState.AddModelError("message", "开票订单明细不能为空");
                return View(viewModel);
            }
            BusinessInvoice invoice = new BusinessInvoice();
            invoice.Id = IdBuilder.CreateIdNum();
            invoice.Transactor = viewModel.Transactor;
            invoice.TransactorId = viewModel.TransactorId;
            invoice.Status = Consts.StateLock;//待开票
            invoice.MoneyStatus = Consts.StateLock;//未到款
            invoice.LinkManName = viewModel.LinkManName;
            invoice.LinkManId = viewModel.LinkManId;
            invoice.InvoiceTitle = viewModel.InvoiceTitle;
            invoice.InvoiceType = viewModel.InvoiceType;
            invoice.Address = viewModel.Address;
            invoice.Bank = viewModel.Bank;
            invoice.BankNum = viewModel.BankNum;
            invoice.Company = !string.IsNullOrWhiteSpace(viewModel.Company) ? viewModel.Company.Trim().Replace(" ","") : viewModel.Company;
            invoice.Phone = viewModel.Phone;
            invoice.TaxNum = viewModel.TaxNum;
            invoice.Remark = viewModel.Remark;
            //发票明细
            foreach (var item in details)
            {
                item.Id = IdBuilder.CreateIdNum();
                item.AddedBy = CurrentManager.UserName;
                item.AddedById = CurrentManager.Id;
                item.AddedDate = DateTime.Now;
                invoice.BusinessInvoiceDetails.Add(item);

            }
            invoice.TotalMoney = details.Sum(d => d.InvoiceMoney);
            invoice.AddedBy = CurrentManager.UserName;
            invoice.AddedById = CurrentManager.Id;
            invoice.AddedDate = DateTime.Now;
            invoice.AuditStatus = Consts.StateNormal;
            _businessInvoiceService.Add(invoice);
            TempData["Msg"] = "开票成功";
            return RedirectToAction("Index");
        }

        public ActionResult Update(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            BusinessInvoiceView viewModel = new BusinessInvoiceView();
            viewModel.Id = id;
            viewModel.Transactor = entity.Transactor;
            viewModel.TransactorId = entity.TransactorId;
            viewModel.LinkManName = entity.LinkManName;
            viewModel.LinkManId = entity.LinkManId;
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
            viewModel.AuditStatus = entity.AuditStatus;
            List<BusinessInvoiceDetail> details = entity.BusinessInvoiceDetails.ToList();
            viewModel.OrderDetails = JsonConvert.SerializeObject(details.Select(d => new
            {
                d.BusinessOrderId,
                d.BusinessOrder.Remark,
                d.BusinessOrder.OrderNum,
                d.OrderMoney,
                d.InvoiceMoney
            }));
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
            var orderdetails = JsonConvert.DeserializeObject<List<BusinessInvoiceDetail>>(viewModel.OrderDetails);
            if (orderdetails.Count <= 0)
            {
                ModelState.AddModelError("message", "开票订单明细不能为空");
                return View(viewModel);
            }

            var invoice = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            if (invoice.Status == Consts.StateNormal)
            {
                ModelState.AddModelError("message", "发票已开，无法修改，请联系财务进行处理");
                return View(viewModel);
            }
            invoice.Transactor = viewModel.Transactor;
            invoice.TransactorId = viewModel.TransactorId;
            invoice.LinkManName = viewModel.LinkManName;
            invoice.LinkManId = viewModel.LinkManId;
            invoice.InvoiceTitle = viewModel.InvoiceTitle;
            invoice.InvoiceType = viewModel.InvoiceType;
            invoice.Address = viewModel.Address;
            invoice.Bank = viewModel.Bank;
            invoice.BankNum = viewModel.BankNum;
            invoice.Company = !string.IsNullOrWhiteSpace(viewModel.Company) ? viewModel.Company.Trim().Replace(" ", "") : viewModel.Company;
            invoice.Phone = viewModel.Phone;
            invoice.TaxNum = viewModel.TaxNum;
            invoice.Remark = viewModel.Remark;
            invoice.ModifiedBy = CurrentManager.UserName;
            invoice.ModifiedById = CurrentManager.Id;
            invoice.ModifiedDate = DateTime.Now;
            //明细处理
            if (invoice.AuditStatus == Consts.StateLock)
            {
                _businessInvoiceDetailrepository.Remove(invoice.BusinessInvoiceDetails);
                foreach (var businessInvoiceDetail in orderdetails)
                {
                    businessInvoiceDetail.Id = IdBuilder.CreateIdNum();
                    businessInvoiceDetail.AddedBy = CurrentManager.UserName;
                    businessInvoiceDetail.AddedById = CurrentManager.Id;
                    businessInvoiceDetail.AddedDate = DateTime.Now;
                    invoice.BusinessInvoiceDetails.Add(businessInvoiceDetail);
                }
                invoice.TotalMoney = orderdetails.Sum(d => d.InvoiceMoney);
            }

            _businessInvoiceService.Update(invoice);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.Status == Consts.StateNormal)
            {
                return Json(new { State = 0, Msg = "发票已开，无法删除，请联系财务处理" });
            }
            _businessInvoiceDetailrepository.Remove(entity.BusinessInvoiceDetails);
            _businessInvoiceService.Delete(entity);//物理删除
            return Json(new { State = 1, Msg = "撤销成功" });
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Audit(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.AuditStatus == null || entity.AuditStatus == Consts.StateLock)
            {
                entity.AuditStatus = Consts.StateNormal;
            }
            else
            {
                entity.AuditStatus = Consts.StateLock;
            }
            _businessInvoiceService.Update(entity);
            TempData["Msg"] = "操作成功";
            return RedirectToAction("Update", new { id });
        }

        public ActionResult WriteOff()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WriteOff(InvoiceWriteOffView viewModel)
        {
            //校验数据
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "请选择核销款项或核销发票！");
                return View(viewModel);
            }
            var businessInvoicesIds = viewModel.BusinessInvoicesIds.Split(',').ToList().Distinct();
            var receivaluesIds = viewModel.ReceivalesIds.Split(',').ToList().Distinct();
            var result = _businessInvoiceService.WriteOff(businessInvoicesIds, receivaluesIds);
            if (!result)
            {
                ModelState.AddModelError("message", "核销信息或金额不一致！");
                return View(viewModel);
            }
            TempData["Msg"] = "核销成功";
            return View();
        }
        
    }
}