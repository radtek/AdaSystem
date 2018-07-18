using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
        private readonly IManagerService _managerService;

        public CommissionController(IBusinessWriteOffService businessWriteOffService, IManagerService managerService)
        {
            _businessWriteOffService = businessWriteOffService;
            _managerService = managerService;
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
            var allList = _businessWriteOffService.LoadEntitiesFilter(new BusinessWriteOffDetailView(){WriteOffDateStar = startDate,WriteOffDateEnd = endDate});
            //var managers = _managerService.GetByOrganizationName("业务部");
            //List<BusinessWriteOffDetailView> list=new List<BusinessWriteOffDetailView>();
            //foreach (var managerView in managers.ToList())
            //{
            //    BusinessWriteOffDetailView item=new BusinessWriteOffDetailView();
            //    item.Transactor = managerView.UserName;
            //    item.TotalCommission = allList.Where(d => d.TransactorId == managerView.Id).Sum(d => d.Commission);
            //    list.Add(item);
            //}

            var result = allList.GroupBy(d => d.Transactor).Select(d => new BusinessCommission
            {
                Transactor = d.Key,
                TotalCommission = d.Sum(t => t.Commission)
            });
            return View(result.OrderByDescending(d=>d.TotalCommission).ToList());
        }
    }
}