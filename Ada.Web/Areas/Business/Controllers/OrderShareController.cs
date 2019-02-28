using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Business;
using Newtonsoft.Json.Linq;

namespace Business.Controllers
{
    public class OrderShareController : BaseController
    {
        private readonly IBusinessOrderService _service;
        private readonly IRepository<BusinessOrder> _repository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseOrderDetailRepository;
        private readonly IRepository<BusinessOrderDetail> _businessOrderDetailRepository;
        public OrderShareController(IBusinessOrderService service,
            IRepository<BusinessOrder> repository,
            IRepository<PurchaseOrderDetail> purchaseOrderDetailRepository,
            IRepository<BusinessOrderDetail> businessOrderDetailRepository)
        {
            _service = service;
            _repository = repository;
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
            _businessOrderDetailRepository = businessOrderDetailRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessOrderView viewModel)
        {
            viewModel.IsRecommend = true;
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessOrderView
                {

                    Transactor = d.Transactor,
                    OrderDate = d.OrderDate,
                    Id = d.Id,
                    Remark = d.Remark,
                    BusinessType = d.BusinessType
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            var details = item.BusinessOrderDetails.Select(d => new BusinessOrderDetailView()
            {
                MediaTitle = d.MediaTitle,
                MediaTypeName = d.MediaTypeName,
                MediaName = d.MediaName,
                MediaByPurchase = d.MediaByPurchase,
                AdPositionName = d.AdPositionName,
                PublishLink = GetPurchaseOrderDetail(d.Id)?.PublishLink,
                PublishDate = GetPurchaseOrderDetail(d.Id)?.PublishDate,
               
            });
            return PartialView("Detail", details);
        }
        private PurchaseOrderDetail GetPurchaseOrderDetail(string id)
        {
            return _purchaseOrderDetailRepository.LoadEntities(d => d.BusinessOrderDetailId == id).FirstOrDefault();
        }
        [HttpPost]
        public ActionResult Export(BusinessOrderView viewModel)
        {
            viewModel.limit = 100;
            viewModel.IsRecommend = true;
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            JArray jObjects = new JArray();
            foreach (var item in result)
            {
                foreach (var detail in item.BusinessOrderDetails)
                {
                    var purchase = GetPurchaseOrderDetail(detail.Id);
                    var jo = new JObject();
                    jo.Add("销售类型", item.BusinessType);
                    jo.Add("项目摘要", item.Remark);
                    jo.Add("项目日期", item.OrderDate);
                    jo.Add("销售人员", item.Transactor);
                    jo.Add("媒体类型", detail.MediaTypeName);
                    jo.Add("媒体名称", detail.MediaName);
                    jo.Add("广告位置", detail.AdPositionName);
                    jo.Add("出刊链接", purchase?.PublishLink);
                    jo.Add("出刊日期", purchase?.PublishDate);
                    jo.Add("经办媒介", detail.MediaByPurchase);
                    jObjects.Add(jo);
                }
            }
            return Json(new { State = 1, Msg = ExportFile(jObjects.ToString()) });
        }
    }
}