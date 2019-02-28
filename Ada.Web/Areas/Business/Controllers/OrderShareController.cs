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

namespace Business.Controllers
{
    public class OrderShareController : BaseController
    {
        private readonly IBusinessOrderService _service;
        private readonly IRepository<BusinessOrder> _repository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseOrderDetailRepository;
        public OrderShareController(IBusinessOrderService service,
            IRepository<BusinessOrder> repository,
            IRepository<PurchaseOrderDetail> purchaseOrderDetailRepository)
        {
            _service = service;
            _repository = repository;
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
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
    }
}