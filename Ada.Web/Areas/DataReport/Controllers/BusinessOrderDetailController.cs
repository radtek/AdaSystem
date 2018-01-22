using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Filter;
using Ada.Services.Business;
using Ada.Services.Setting;
using Newtonsoft.Json.Linq;

namespace DataReport.Controllers
{
    public class BusinessOrderDetailController : BaseController
    {
        private readonly IBusinessOrderDetailService _businessOrderDetailService;
        private readonly IRepository<PurchaseOrderDetail> _purchaseOrderDetailRepository;
        private readonly ISettingService _settingService;
        public BusinessOrderDetailController(IBusinessOrderDetailService businessOrderDetailService,
            IRepository<PurchaseOrderDetail> purchaseOrderDetailRepository,
            ISettingService settingService)
        {
            _businessOrderDetailService = businessOrderDetailService;
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
            _settingService = settingService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessOrderDetailView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _businessOrderDetailService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
               
                viewModel.total,
                rows = result.Select(d => new BusinessOrderDetailView
                {
                    Id = d.Id,
                    BusinessOrderId = d.BusinessOrderId,
                    VerificationMoney = d.VerificationMoney,
                    MediaName = d.MediaName,
                    MediaTypeName = d.MediaTypeName,
                    AdPositionName = d.AdPositionName,
                    Money = d.Money,
                    TaxMoney = d.TaxMoney,
                    Remark = d.Remark,
                    MediaPriceId = d.MediaPrice.MediaId,
                    Tax = d.Tax,
                    SellMoney = d.SellMoney,
                    VerificationStatus = d.VerificationStatus,
                    Status = d.Status,
                    Transactor = d.BusinessOrder.Transactor,
                    LinkManName = d.BusinessOrder.LinkManName,
                    LinkManId = d.BusinessOrder.LinkManId,
                    OrderDate = d.BusinessOrder.OrderDate,
                    OrderNum = d.BusinessOrder.OrderNum,
                    OrderRemark = d.BusinessOrder.Remark,
                    PurchaseStatus = GetPurchaseOrderDetail(d.Id)?.Status,
                    PurchaseMoney = GetPurchaseOrderDetail(d.Id)?.PurchaseMoney,
                    PublishDate = GetPurchaseOrderDetail(d.Id)?.PublishDate,
                    PublishLink = GetPurchaseOrderDetail(d.Id)?.PublishLink,
                    MediaByPurchase = GetPurchaseOrderDetail(d.Id)?.Transactor,
                    TotalMoney = viewModel.TotalMoney,
                    TotalPurchaseMoney = viewModel.TotalPurchaseMoney,
                    TotalSellMoney = viewModel.TotalSellMoney
                })
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Export(BusinessOrderDetailView viewModel)
        {
            viewModel.Managers = PremissionData();
            var setting = _settingService.GetSetting<WeiGuang>();
            viewModel.limit = setting.BusinessOrderExportRows;
            var business = _businessOrderDetailService.LoadEntitiesFilter(viewModel);
            var purchase = _purchaseOrderDetailRepository.LoadEntities(d => d.IsDelete == false);
            var result = from b in business
                from p in purchase
                where b.Id == p.BusinessOrderDetailId
                select new BusinessOrderDetailView()
                {
                    OrderDate = b.BusinessOrder.OrderDate,
                    OrderNum = b.BusinessOrder.OrderNum,
                    OrderRemark = b.BusinessOrder.Remark,
                    LinkManName = b.BusinessOrder.LinkManName,
                    MediaTypeName = b.MediaTypeName,
                    MediaName = b.MediaName,
                    AdPositionName = b.AdPositionName,
                    SellMoney = b.SellMoney,
                    PurchaseMoney = p.PurchaseMoney,
                    PublishDate = p.PublishDate,
                    PublishLink = p.PublishLink,
                    MediaByPurchase = p.Transactor,
                    Transactor = b.BusinessOrder.Transactor
                };
            JArray jObjects = new JArray();
            foreach (var item in result)
            {
                var jo = new JObject();
                jo.Add("订单日期", item.OrderDate);
                jo.Add("订单编号", item.OrderNum);
                jo.Add("项目摘要", item.OrderRemark);
                jo.Add("客户名称", item.LinkManName);
                jo.Add("媒体类型", item.MediaTypeName);
                jo.Add("媒体名称", item.MediaName);
                jo.Add("广告位", item.AdPositionName);
                jo.Add("无税金额", item.SellMoney);
                jo.Add("采购成本", item.PurchaseMoney);
                jo.Add("出刊日期", item.PublishDate);
                jo.Add("出刊链接", item.PublishLink);
                jo.Add("经办媒介", item.MediaByPurchase);
                jo.Add("销售人员", item.Transactor);
                jObjects.Add(jo);
            }
            return File(ExportData(jObjects.ToString()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
        }
        private PurchaseOrderDetail GetPurchaseOrderDetail(string id)
        {
            return _purchaseOrderDetailRepository.LoadEntities(d => d.BusinessOrderDetailId == id).FirstOrDefault();
        }
    }
}