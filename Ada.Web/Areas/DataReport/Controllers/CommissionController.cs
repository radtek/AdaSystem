using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Ada.Services.Business;
using DataReport.Models;

namespace DataReport.Controllers
{
    /// <summary>
    /// 提成统计
    /// </summary>
    public class CommissionController : BaseController
    {
        private readonly IBusinessWriteOffService _businessWriteOffService;

        public CommissionController(IBusinessWriteOffService businessWriteOffService)
        {
            _businessWriteOffService = businessWriteOffService;
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
            var allList = _businessWriteOffService.LoadEntitiesFilters(new BusinessWriteOffDetailView(){WriteOffDateStar = startDate,WriteOffDateEnd = endDate});
            var result = allList.GroupBy(d => d.BusinessWriteOff.Transactor).Select(d => new BusinessCommission
            {
                Transactor = d.Key,
                TotalCommission = d.Sum(t => t.Commission)
            }).OrderByDescending(d => d.TotalCommission);
            return View(result.ToList());
        }
    }
}