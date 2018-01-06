using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Business;

namespace DataReport.Controllers
{
    public class BusinessOrderController : BaseController
    {
        private readonly IBusinessOrderService _businessOrderService;
        private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;

        public BusinessOrderController(IBusinessOrderService businessOrderService, IRepository<PurchaseOrder> purchaseOrderRepository)
        {
            _businessOrderService = businessOrderService;
            _purchaseOrderRepository = purchaseOrderRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessOrderView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _businessOrderService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessOrderView
                {
                    Id = d.Id,
                    OrderNum = d.OrderNum,
                    LinkManName = d.LinkManName,
                    LinkManId = d.LinkManId,
                    TotalMoney = d.BusinessOrderDetails.Where(a => a.Status == Consts.StateNormal).Sum(o => o.Money),
                    Transactor = d.Transactor,
                    Status = d.Status ?? Consts.StateLock,
                    OrderDate = d.OrderDate,
                    TotalSellMoney = d.BusinessOrderDetails.Where(a => a.Status == Consts.StateNormal).Sum(o => o.SellMoney),
                    TotalTaxMoney = d.BusinessOrderDetails.Where(a => a.Status == Consts.StateNormal).Sum(o => o.TaxMoney),
                    TotalPurchaseMoney = GetPurchaseMoney(d.Id),
                    Remark = d.Remark,
                    AllSellMoney = viewModel.AllSellMoney,
                    AllTaxMoney = viewModel.AllTaxMoney,
                    AllMoney = viewModel.AllMoney,
                    AllPurchaseMoney = viewModel.AllPurchaseMoney
                })
            }, JsonRequestBehavior.AllowGet);
        }

        private decimal? GetPurchaseMoney(string id)
        {
            var order = _purchaseOrderRepository.LoadEntities(d => d.BusinessOrderId == id).FirstOrDefault();
            return order != null ? order.PurchaseOrderDetails.Sum(d => d.PurchaseMoney) : 0;
        }
    }
}