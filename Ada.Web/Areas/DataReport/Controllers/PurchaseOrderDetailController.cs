using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.ViewModel.Purchase;
using Ada.Framework.Filter;
using Ada.Services.Purchase;

namespace DataReport.Controllers
{
    public class PurchaseOrderDetailController : BaseController
    {
        private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;
        
        public PurchaseOrderDetailController(IPurchaseOrderDetailService purchaseOrderDetailService)
        {
            _purchaseOrderDetailService = purchaseOrderDetailService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(PurchaseOrderDetailView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _purchaseOrderDetailService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new PurchaseOrderDetailView
                {
                    Id = d.Id,
                    Status = d.Status,
                    LinkManName = d.LinkManName,
                    LinkManId = d.LinkManId,
                    PurchaseOrderNum = d.PurchaseOrder.OrderNum,
                    BusinessBy = d.PurchaseOrder.BusinessBy,
                    MediaTypeName = d.MediaTypeName,
                    MediaName = d.MediaName,
                    MediaPriceId = d.MediaPrice.MediaId,
                    AdPositionName = d.AdPositionName,
                    CostMoney = d.CostMoney,
                    PurchaseMoney = d.PurchaseMoney,
                    Tax = d.Tax,
                    TaxMoney = d.TaxMoney,
                    DiscountMoney = d.DiscountMoney,
                    Money = d.Money,
                    PublishDate = d.PublishDate,
                    PublishLink = d.PublishLink,
                    Transactor = d.Transactor,
                    OrderDate = d.PurchaseOrder.OrderDate,
                    TotalMoney = viewModel.TotalMoney,
                    TotalPurchaseMoney = viewModel.TotalPurchaseMoney,
                    TotalTaxMoney = viewModel.TotalTaxMoney,
                    TotalDiscountMoney = viewModel.TotalDiscountMoney,
                    AuditStatus = d.AuditStatus
                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}