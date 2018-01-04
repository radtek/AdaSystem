using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Finance;
using Ada.Framework.Filter;
using Ada.Services.Finance;

namespace DataReport.Controllers
{
    public class SettleAccountController : BaseController
    {
        private readonly ISettleAccountService _settleAccountService;
        public SettleAccountController(ISettleAccountService settleAccountService)
        {
            _settleAccountService = settleAccountService;
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string start,string end)
        {
            if (string.IsNullOrWhiteSpace(start)|| string.IsNullOrWhiteSpace(end))
            {
                ModelState.AddModelError("message", "请输入要统计的日期范围");
                return View();
            }
            ViewBag.Start = start;
            ViewBag.End = end;
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end).AddDays(1);
            var settleAccountViews = _settleAccountService.BalanceStatistics(startDate, endDate);
            return View(settleAccountViews);
        }
    }
}