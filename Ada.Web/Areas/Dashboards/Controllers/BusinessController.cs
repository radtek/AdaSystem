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
            //var temp = business.Where(d => d.Status != 0);
            viewModel.OrderCount = string.IsNullOrWhiteSpace(userId) ? purchase.Count(d => d.Status != Consts.PurchaseStatusFail) : purchase.Count(d => d.Status != Consts.PurchaseStatusFail && d.PurchaseOrder.BusinessById == userId);
            var moneyTotal = _businessOrderDetailService.BusinessPerformanceGroupByUser(new BusinessOrderDetailView());
            if (!string.IsNullOrWhiteSpace(userId))
            {
                moneyTotal = moneyTotal.Where(d => d.Transactor == CurrentManager.UserName);
            }
            viewModel.SellMoney = moneyTotal.Sum(d => d.TotalSellMoney);
            viewModel.ConfirmVerificationMoney = moneyTotal.Sum(d => d.TotalConfirmVerificationMoney);
            viewModel.VerificationMoney = moneyTotal.Sum(d => d.TotalVerificationMoney);
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
                .OrderBy(d => d.Month).ToList();
            viewModel.BusinessPerformancesByUser = _businessOrderDetailService
                .BusinessPerformanceGroupByUser(new BusinessOrderDetailView()).OrderByDescending(d => d.TotalVerificationMoney)
                .ToList();
            //未核销金额 联系人分组
            viewModel.VerificationInfos = business
                .Where(d => d.Status == Consts.StateOK && d.VerificationStatus == Consts.StateLock)
                .GroupBy(d => d.BusinessOrder.LinkMan).Select(d => new VerificationInfo
                {
                    CompanyName = d.Key.Commpany.Name,
                    LinkManName = d.Key.Name,
                    LinkManId = d.Key.Id,
                    VerificationMoney = d.Sum(o => o.VerificationMoney)
                }).OrderByDescending(d => d.VerificationMoney).Take(50).ToList();
            return View(viewModel);
        }
        /// <summary>
        /// 业绩排行
        /// </summary>
        /// <param name="o"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public ActionResult GetBusinessPerformance(string o, string t = null)
        {
            BusinessOrderDetailView quare = new BusinessOrderDetailView();
            //var date = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(t))
            {
                if (DateTime.TryParse(t, out var dateTime))
                {
                    quare.PublishDateStart = new DateTime(dateTime.Year, dateTime.Month, 1);
                    quare.PublishDateEnd = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
                }

            }
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
            BusinessOrderDetailView quare = new BusinessOrderDetailView();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                quare.TransactorId = userId;
            }

            var result = _businessOrderDetailService.BusinessPerformanceGroupByDate(quare);
            return Json(result.OrderBy(d => d.Month).Take(12).ToList(), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProfitMargin(string date)
        {
            var now = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(date))
            {
                if (DateTime.TryParse(date, out var dateTime))
                {
                    now = dateTime;
                }
            }
            BusinessOrderDetailView quare = new BusinessOrderDetailView();
            quare.PublishDateStart = new DateTime(now.Year, now.Month, 1);
            quare.PublishDateEnd = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
            var result = _businessOrderDetailService.BusinessPerformanceGroupByMediaType(quare);
            return Json(result.Select(d => new { name = d.MediaTypeName , value = d.TotalProfitMoney }).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}