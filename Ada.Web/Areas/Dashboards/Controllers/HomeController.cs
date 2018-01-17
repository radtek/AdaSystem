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
            var business =
                _businessRepository.LoadEntities(d => d.BusinessOrder.IsDelete == false && d.IsDelete == false);
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
                              where b.Id == p.BusinessOrderDetailId && p.Status == Consts.PurchaseStatusSuccess&&b.Status==Consts.StateNormal
                              select b).Count();
            var start = DateTime.Now.Date;
            var end = start.AddDays(1);
            viewModel.Today = business.Count(d => d.PrePublishDate > start && d.PrePublishDate < end);
            var start1 = DateTime.Now.Date.AddDays(1);
            var end1 = start1.AddDays(1);
            viewModel.Tomorrow = business.Count(d => d.PrePublishDate > start1 && d.PrePublishDate < end1);
            ////业务一二部 本月
            //var managers1 = _managerService.GetByOrganizationName("业务一部");
            //var managers2 = _managerService.GetByOrganizationName("业务二部");
            //BusinessOrderDetailView quare=new BusinessOrderDetailView();
            //quare.PublishDateStart= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //quare.PublishDateEnd= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            //viewModel.Business1 = _businessOrderDetailService.BusinessPerformance(managers1.ToList(), quare);
            //viewModel.Business2 = _businessOrderDetailService.BusinessPerformance(managers2.ToList(), quare);
            return View(viewModel);
        }

        public ActionResult GetBusinessPerformance(string o)
        {
            var managers = _managerService.GetByOrganizationName(o);
            BusinessOrderDetailView quare = new BusinessOrderDetailView();
            quare.PublishDateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            quare.PublishDateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            var result = _businessOrderDetailService.BusinessPerformance(managers.ToList(), quare).OrderByDescending(d=>d.TotalProfitMoney).Take(10);
            return Json(result, JsonRequestBehavior.AllowGet);
        }



    }
}