using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Finance;
using Ada.Core.Domain.Purchase;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Resource;
using Ada.Core.ViewModel.Statistics;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Ada.Services.Business;

namespace Dashboards.Controllers
{
    public class BossController : BaseController
    {
        private readonly IRepository<BusinessOrderDetail> _businessRepository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseRepository;
        private readonly IRepository<Receivables> _receivablesRepository;
        private readonly IRepository<BillPaymentDetail> _billPaymentDetailRepository;
        private readonly IRepository<ExpenseDetail> _expenseDetailDetailRepository;
        private readonly IRepository<MediaPrice> _mediaRepository;
        private readonly IRepository<OrderDetailComment> _orderDetailCommentRepository;
        private readonly IRepository<MediaComment> _mediaCommentRepository;
        private readonly IBusinessOrderDetailService _businessOrderDetailService;

        public BossController(IRepository<BusinessOrderDetail> businessRepository,
            IRepository<PurchaseOrderDetail> purchaseRepository,
            IBusinessOrderDetailService businessOrderDetailService,
            IRepository<Receivables> receivablesRepository,
            IRepository<BillPaymentDetail> billPaymentDetailRepository,
            IRepository<ExpenseDetail> expenseDetailDetailRepository,
            IRepository<MediaPrice> mediaRepository,
            IRepository<OrderDetailComment> orderDetailCommentRepository,
            IRepository<MediaComment> mediaCommentRepository

            )
        {
            _businessRepository = businessRepository;
            _purchaseRepository = purchaseRepository;
            _receivablesRepository = receivablesRepository;
            _billPaymentDetailRepository = billPaymentDetailRepository;
            _expenseDetailDetailRepository = expenseDetailDetailRepository;
            _mediaRepository = mediaRepository;
            _orderDetailCommentRepository = orderDetailCommentRepository;
            _mediaCommentRepository = mediaCommentRepository;
            _businessOrderDetailService = businessOrderDetailService;
        }
        public ActionResult Index()
        {
            WeiGuangTotal total = new WeiGuangTotal();
            var business = _businessRepository
                .LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false && d.Status != 0);
            //订单数校验
            total.BusinessCount = business.Count();
            total.PurchaseCount = _purchaseRepository.LoadEntities(d => d.IsDelete == false).Count();
            total.OrderStatus = total.BusinessCount == total.PurchaseCount;
            //核销金额校验

            total.BusinessSellMoney = business.Sum(d => d.SellMoney);
            total.BusinessVerificationMoney = business.Sum(d => d.VerificationMoney);
            total.BusinessConfirmVerificationMoney = business.Sum(d => d.ConfirmVerificationMoney);
            total.MoneyStatus = total.BusinessSellMoney ==
                                total.BusinessConfirmVerificationMoney + total.BusinessVerificationMoney;
            //账户总额
            //收入
            var receivables = _receivablesRepository
                  .LoadEntities(d => d.IsDelete == false && d.IncomeExpend.SubjectType == Consts.StateNormal)
                  .Sum(d => d.Money);
            var income = _expenseDetailDetailRepository
                .LoadEntities(d => d.IsDelete == false && d.IncomeExpend.SubjectType == Consts.StateNormal && d.Expense.IsDelete == false)
                .Sum(d => d.Money);
            //支出
            var expend = _expenseDetailDetailRepository
                .LoadEntities(d => d.IsDelete == false && d.IncomeExpend.SubjectType == Consts.StateLock && d.Expense.IsDelete == false)
                .Sum(d => d.Money);
            var billpay = _billPaymentDetailRepository
                .LoadEntities(d => d.IsDelete == false && d.IncomeExpend.SubjectType == Consts.StateLock && d.BillPayment.IsDelete == false)
                .Sum(d => d.Money);
            total.Income = receivables + income;
            total.Expend = expend + billpay;
            //订单 资源评价
            total.OrderComments = _orderDetailCommentRepository.LoadEntities(d => d.IsDelete == false).GroupBy(d => d.Transactor).Select(d =>
                  new Comment
                  {
                      Transactor = d.Key,
                      Count = d.Count()
                  }).OrderByDescending(d => d.Count).ToList();
            total.MediaComments = _mediaCommentRepository.LoadEntities(d => d.IsDelete == false).GroupBy(d => d.Transactor).Select(d =>
                new Comment
                {
                    Transactor = d.Key,
                    Count = d.Count()
                }).OrderByDescending(d => d.Count).ToList();
            //媒介订单采购成本与实际采购的统计
            total.PurchaseOrderTotals = GetPurchaseOrderTotal();
            //改价统计
            total.PriceChanges = _businessRepository.LoadEntities(d => d.RequestSellMoney >= 0)
                .GroupBy(d => d.BusinessOrder.Transactor).Select(d => new SelectListItem()
                {
                    Text = d.Key,
                    Value = (d.Sum(r => r.RequestSellMoney) - d.Sum(r => r.SellMoney)) + ""
                }).ToList();
            return View(total);
        }

        public ActionResult GetPurchaseOrder(string date)
        {
            return Json(GetPurchaseOrderTotal(date), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMediaTypeBusinessTop(string index,string date)
        {
            BusinessOrderDetailView quare = new BusinessOrderDetailView();
            if (!string.IsNullOrWhiteSpace(date))
            {
                if (DateTime.TryParse(date, out var dateTime))
                {
                    quare.PublishDateStart = new DateTime(dateTime.Year, dateTime.Month, 1);
                    quare.PublishDateEnd = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
                }
            }
            quare.MediaTypeId = index;
            var result = _businessOrderDetailService.BusinessPerformanceGroupByUser(quare).OrderByDescending(d => d.TotalProfitMoney).Take(20).ToList();
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        private List<PurchaseOrderTotal> GetPurchaseOrderTotal(string date = null)
        {
            var orders = _purchaseRepository
                .LoadEntities(d =>
                    d.MediaPrice.Media.MediaType.IsComment == true && d.IsDelete == false &&
                    d.Status == Consts.PurchaseStatusSuccess);
            if (!string.IsNullOrWhiteSpace(date))
            {
                var dateTime = DateTime.Parse(date);
                var start = new DateTime(dateTime.Year, dateTime.Month, 1);
                var end = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month)).AddDays(1);
                orders = orders.Where(d => d.PublishDate >= start && d.PublishDate < end);
            }

            return orders.GroupBy(d => d.Transactor).Select(d => new PurchaseOrderTotal
            {
                Transactor = d.Key,
                PurchaseMoney = d.Sum(p => p.PurchaseMoney),
                CostMoney = d.Sum(p => p.CostMoney),
                Diff = d.Sum(p => p.CostMoney) - d.Sum(p => p.PurchaseMoney)
            }).ToList();
        }
    }
}