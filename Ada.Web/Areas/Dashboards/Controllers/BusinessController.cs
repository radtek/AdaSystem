using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Statistics;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Ada.Services.Business;

namespace Dashboards.Controllers
{
    public class BusinessController : BaseController
    {
        private readonly IRepository<BusinessOrderDetail> _businessRepository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseRepository;
        private readonly IBusinessOrderDetailService _businessOrderDetailService;
        public BusinessController(IRepository<BusinessOrderDetail> businessRepository,
            IRepository<PurchaseOrderDetail> purchaseRepository,
            IBusinessOrderDetailService businessOrderDetailService)
        {
            _businessRepository = businessRepository;
            _purchaseRepository = purchaseRepository;
            _businessOrderDetailService = businessOrderDetailService;
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
                business = business.Where(d => d.BusinessOrder.TransactorId == userId);
            }
            var purchase = _purchaseRepository.LoadEntities(d => d.IsDelete == false);
            var temp = business.Where(d => d.Status != 0);
            viewModel.OrderCount = temp.Count();
            viewModel.SellMoney = temp.Sum(d => d.SellMoney);
            viewModel.ConfirmVerificationMoney = temp.Sum(d => d.ConfirmVerificationMoney);
            viewModel.VerificationMoney = temp.Sum(d => d.VerificationMoney);
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
            BusinessOrderDetailView quare = new BusinessOrderDetailView();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                quare.TransactorId = userId;
            }

            viewModel.BusinessPerformances = _businessOrderDetailService.BusinessPerformanceGroupByDate(quare)
                .OrderBy(d => d.Month).Take(12).ToList();
            return View(viewModel);
        }
        /// <summary>
        /// 业绩排行
        /// </summary>
        /// <param name="o"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public ActionResult GetBusinessPerformance(string o,string t=null)
        {
            var date = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(t))
            {
                if (DateTime.TryParse(t,out var dateTime))
                {
                    date = dateTime;
                }
            }
           
            BusinessOrderDetailView quare = new BusinessOrderDetailView();
            quare.PublishDateStart = new DateTime(date.Year, date.Month, 1);
            quare.PublishDateEnd = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            quare.OrganizationName = o;
            var result = _businessOrderDetailService.BusinessPerformanceGroupByUser(quare).OrderByDescending(d => d.TotalProfitMoney);
            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBusinessIncome()
        {
            var premission = PremissionData();
            var userId = string.Empty;
            if (premission.Count > 0)
            {
                userId = CurrentManager.Id;
            }
            //List<BusinessTotal> list = new List<BusinessTotal>();
            ////当前月后面11个月
            //for (int i = 0; i < 12; i++)
            //{
            //    BusinessTotal total = new BusinessTotal();
            //    BusinessOrderDetailView quare = new BusinessOrderDetailView();
            //    if (i == 0)
            //    {
            //        quare.PublishDateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //        quare.PublishDateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            //        total.Month = DateTime.Now.Month + "月";
            //    }
            //    else
            //    {
            //        quare.PublishDateStart = new DateTime(DateTime.Now.AddMonths(-i).Year, DateTime.Now.AddMonths(-i).Month, 1);
            //        quare.PublishDateEnd = new DateTime(DateTime.Now.AddMonths(-i).Year, DateTime.Now.AddMonths(-i).Month, DateTime.DaysInMonth(DateTime.Now.AddMonths(-i).Year, DateTime.Now.AddMonths(-i).Month));
            //        total.Month = DateTime.Now.AddMonths(-i).Month + "月";
            //    }
            //    total.Sort = i;

            //    //total.BusinessOrderDetailView = string.IsNullOrWhiteSpace(userId) ? _businessOrderDetailService.BusinessPerformance(quare) : _businessOrderDetailService.BusinessPerformance(userId, quare);
            //    list.Add(total);
            //}
            BusinessOrderDetailView quare = new BusinessOrderDetailView();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                quare.TransactorId = userId;
            }

            var result = _businessOrderDetailService.BusinessPerformanceGroupByDate(quare);
            return Json(result.OrderBy(d=>d.Month).Take(12).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}