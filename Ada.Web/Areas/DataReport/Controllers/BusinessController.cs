using System;
using System.Linq;
using System.Web.Mvc;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Ada.Services.Business;

namespace DataReport.Controllers
{
    /// <summary>
    /// 销售业绩
    /// </summary>
    public class BusinessController : BaseController
    {
        private readonly IManagerService _managerService;
        private readonly IBusinessOrderDetailService _businessOrderDetailService;
        public BusinessController(IManagerService managerService, IBusinessOrderDetailService businessOrderDetailService)
        {
            _managerService = managerService;

            _businessOrderDetailService = businessOrderDetailService;
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            var managers = _managerService.GetByOrganizationName("业务部");
            var view = new BusinessOrderDetailView();
            view.PublishDateStart = startDate;
            view.PublishDateEnd = endDate;
            var model = _businessOrderDetailService.BusinessPerformance(managers.ToList(), view);
            return View(model);
        }
    }
}