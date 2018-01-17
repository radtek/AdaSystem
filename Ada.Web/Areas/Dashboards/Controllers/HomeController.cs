using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Admin;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Ada.Services.Business;

namespace Dashboards.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IRepository<BusinessOrderDetail> _businessRepository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseRepository;
        private readonly IManagerService _managerService;
        private readonly IBusinessOrderDetailService _businessOrderDetailService;
        public HomeController(IRepository<BusinessOrderDetail> businessRepository,
            IRepository<PurchaseOrderDetail> purchaseRepository,
            IBusinessOrderDetailService businessOrderDetailService,
            IManagerService managerService)
        {
            _businessRepository = businessRepository;
            _purchaseRepository = purchaseRepository;
            _businessOrderDetailService = businessOrderDetailService;
            _managerService = managerService;
        }
        public ActionResult Index()
        {
            //订单数
            BusinessTotal viewModel = new BusinessTotal();
            var premission = PremissionData();
            var userId = string.Empty;
            if (premission.Count > 0)
            {
                userId = CurrentManager.Id;
            }
            var business =
                _businessRepository.LoadEntities(d => d.BusinessOrder.IsDelete == false && d.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(userId))
            {
                business = business.Where(d => d.BusinessOrder.TransactorId==userId);
            }
            var purchase = _purchaseRepository.LoadEntities(d => d.IsDelete == false);
            viewModel.Waiting = business.Count(d => d.Status == Consts.StateLock);

            viewModel.Doing = (from b in business
                               from p in purchase
                               where b.Id == p.BusinessOrderDetailId && p.Status == Consts.PurchaseStatusTodo
                               select b).Count();
            viewModel.Confirm = (from b in business
                                 from p in purchase
                                 where b.Id == p.BusinessOrderDetailId && p.Status == Consts.PurchaseStatusConfirm
                                 select b).Count();
            viewModel.Done = (from b in business
                              from p in purchase
                              where b.Id == p.BusinessOrderDetailId && p.Status == Consts.PurchaseStatusSuccess && b.Status == Consts.StateNormal
                              select b).Count();
            var start = DateTime.Now.Date;
            var end = start.AddDays(1);
            viewModel.Today = business.Count(d => d.PrePublishDate >= start && d.PrePublishDate < end);
            var start1 = DateTime.Now.Date.AddDays(1);
            var end1 = start1.AddDays(1);
            viewModel.Tomorrow = business.Count(d => d.PrePublishDate >= start1 && d.PrePublishDate < end1);
            return View(viewModel);
        }

        public ActionResult GetBusinessPerformance(string o)
        {
            var managers = _managerService.GetByOrganizationName(o);
            BusinessOrderDetailView quare = new BusinessOrderDetailView();
            quare.PublishDateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            quare.PublishDateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            var result = _businessOrderDetailService.BusinessPerformance(managers.ToList(), quare).OrderByDescending(d => d.TotalProfitMoney).Take(10);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBusinessIncome()
        {
            var premission = PremissionData();
            var userId = string.Empty;
            if (premission.Count>0)
            {
                userId = CurrentManager.Id;
            }
            List<BusinessTotal> list = new List<BusinessTotal>();
            //当前月后面11个月
            for (int i = 0; i < 12; i++)
            {
                BusinessTotal total = new BusinessTotal();
                BusinessOrderDetailView quare = new BusinessOrderDetailView();
                if (i == 0)
                {
                    quare.PublishDateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    quare.PublishDateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    total.Month = DateTime.Now.Month + "月";
                }
                else
                {
                    quare.PublishDateStart = new DateTime(DateTime.Now.AddMonths(-i).Year, DateTime.Now.AddMonths(-i).Month, 1);
                    quare.PublishDateEnd = new DateTime(DateTime.Now.AddMonths(-i).Year, DateTime.Now.AddMonths(-i).Month, DateTime.DaysInMonth(DateTime.Now.AddMonths(-i).Year, DateTime.Now.AddMonths(-i).Month));
                    total.Month = DateTime.Now.AddMonths(-i).Month + "月";
                }
                total.Sort = i;
                total.BusinessOrderDetailView = string.IsNullOrWhiteSpace(userId) ? _businessOrderDetailService.BusinessPerformance(quare) : _businessOrderDetailService.BusinessPerformance(userId,quare);
                list.Add(total);
            }
            return Json(list.OrderByDescending(d => d.Sort), JsonRequestBehavior.AllowGet);
        }

    }
}