using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Purchase;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Filter;
using Ada.Services.Purchase;
using Ada.Services.Setting;
using Newtonsoft.Json.Linq;

namespace DataReport.Controllers
{
    public class PurchaseOrderDetailController : BaseController
    {
        private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;
        private readonly ISettingService _settingService;
        public PurchaseOrderDetailController(IPurchaseOrderDetailService purchaseOrderDetailService, ISettingService settingService)
        {
            _purchaseOrderDetailService = purchaseOrderDetailService;
            _settingService = settingService;
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
                    BusinessOrderId = d.PurchaseOrder.BusinessOrderId,
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
                    AuditStatus = d.AuditStatus,
                    IsPayment = d.PurchasePaymentOrderDetails.Any()
                })
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Export(PurchaseOrderDetailView viewModel)
        {
            viewModel.Managers = PremissionData();
            var setting = _settingService.GetSetting<WeiGuang>();
            viewModel.limit = setting.BusinessOrderExportRows;
            var purchase = _purchaseOrderDetailService.LoadEntitiesFilter(viewModel);
            var result = purchase.Select(d => new 
            {
                d.PurchaseOrder.OrderDate,
                d.PurchaseOrder.OrderNum,
                d.MediaName,
                d.LinkManName,
                d.AdPositionName,
                d.PurchaseMoney,
                d.PublishDate,
                d.PublishLink,
                d.Transactor,
                d.MediaTitle,
                d.Money
            });
                         
            JArray jObjects = new JArray();
            foreach (var item in result)
            {
                var jo = new JObject();
                jo.Add("订单日期", item.OrderDate);
                jo.Add("订单编号", item.OrderNum);
                jo.Add("供应商", item.LinkManName);
                jo.Add("媒体名称", item.MediaName);
                jo.Add("广告位", item.AdPositionName);
                jo.Add("采购金额（无税）", item.PurchaseMoney);
                jo.Add("采购金额（含税）", item.Money);
                jo.Add("稿件标题", item.MediaTitle);
                jo.Add("出刊日期", item.PublishDate);
                jo.Add("出刊链接", item.PublishLink);
                jo.Add("经办媒介", item.Transactor);
                jObjects.Add(jo);
            }
            return File(ExportData(jObjects.ToString()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
        }
    }
}