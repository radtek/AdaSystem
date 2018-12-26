using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Ada.Services.Purchase;

namespace DataReport.Controllers
{
    /// <summary>
    /// 媒介业绩统计
    /// </summary>
    public class PurchaseController : BaseController
    {
        private readonly IManagerService _managerService;
        private readonly IPurchaseOrderDetailService _purchaseOrderDetailServic;
        public PurchaseController(IManagerService managerService, IPurchaseOrderDetailService purchaseOrderDetailServic)
        {
            _managerService = managerService;

            _purchaseOrderDetailServic = purchaseOrderDetailServic;
        }
        public ActionResult Index()
        {

            return View();
        }
        [HttpPost]
        
        public ActionResult Index(string start, string end)
        {
            if (string.IsNullOrWhiteSpace(start) || string.IsNullOrWhiteSpace(end))
            {
                ModelState.AddModelError("message", "请输入要统计的日期范围");
                return View();
            }
            ViewBag.Start = start;
            ViewBag.End = end;
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);
            var managers = _managerService.GetByOrganizationName("媒介部");
            var model = _purchaseOrderDetailServic.PurchasePerformance(managers.ToList(),startDate,endDate.AddDays(1));
            return View(model);
        }
    }
}