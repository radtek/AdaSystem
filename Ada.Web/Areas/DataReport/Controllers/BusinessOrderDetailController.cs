using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Business;

namespace DataReport.Controllers
{
    public class BusinessOrderDetailController : BaseController
    {
        private readonly IBusinessOrderDetailService _businessOrderDetailService;
        private readonly IRepository<PurchaseOrderDetail> _purchaseOrderDetailRepository;
        public BusinessOrderDetailController(IBusinessOrderDetailService businessOrderDetailService, IRepository<PurchaseOrderDetail> purchaseOrderDetailRepository)
        {
            _businessOrderDetailService = businessOrderDetailService;
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
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
                    TotalMoney = viewModel.TotalMoney,
                    TotalPurchaseMoney = viewModel.TotalPurchaseMoney,
                    TotalSellMoney = viewModel.TotalSellMoney
                })
            }, JsonRequestBehavior.AllowGet);
        }
        private PurchaseOrderDetail GetPurchaseOrderDetail(string id)
        {
            return _purchaseOrderDetailRepository.LoadEntities(d => d.BusinessOrderDetailId == id).FirstOrDefault();
        }
    }
}