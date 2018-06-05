using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.ViewModel.Purchase;
using Ada.Framework.Filter;
using Ada.Services.Purchase;

namespace Purchase.Controllers
{
    /// <summary>
    /// 采购退款
    /// </summary>
    public class OrderReturnController : BaseController
    {
        private readonly IPurchaseReturnOrderService _service;

        public OrderReturnController(IPurchaseReturnOrderService service)
        {
            _service = service;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(PurchaseReturnOrderView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _service.LoadEntitiesFilter(viewModel).AsNoTracking().ToList();
            return Json(new 
            {
                viewModel.total,
                rows = result.Select(d => new PurchaseReturnOrderView
                {
                    Id = d.Id,
                    //Status = d.Status,
                    LinkManName = d.LinkManName,
                    Transactor = d.Transactor,
                    

                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}