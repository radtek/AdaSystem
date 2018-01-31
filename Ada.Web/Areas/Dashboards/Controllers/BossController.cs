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
        private readonly IRepository<MediaType> _mediaTypeDetailRepository;

        public BossController(IRepository<BusinessOrderDetail> businessRepository,
            IRepository<PurchaseOrderDetail> purchaseRepository,
            IBusinessOrderDetailService businessOrderDetailService,
            IRepository<Receivables> receivablesRepository,
            IRepository<BillPaymentDetail> billPaymentDetailRepository,
            IRepository<ExpenseDetail> expenseDetailDetailRepository,
            IRepository<MediaType> mediaTypeDetailRepository)
        {
            _businessRepository = businessRepository;
            _purchaseRepository = purchaseRepository;
            _receivablesRepository = receivablesRepository;
            _billPaymentDetailRepository = billPaymentDetailRepository;
            _expenseDetailDetailRepository = expenseDetailDetailRepository;
            _mediaTypeDetailRepository = mediaTypeDetailRepository;
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
            ////资源数
            //var types = _mediaTypeDetailRepository.LoadEntities(d => d.IsDelete == false).ToList();
            //List<MediaTypeView> mediaTypeViews = new List<MediaTypeView>();
            //foreach (var mediaType in types)
            //{
            //    MediaTypeView item = new MediaTypeView();
            //    item.Image = mediaType.Image;
            //    item.TypeName = mediaType.TypeName;
            //    item.Taxis = mediaType.Medias.Count(d => d.IsDelete == false);
            //    mediaTypeViews.Add(item);
            //}

            //total.MediaTypes = mediaTypeViews;
            return View(total);
        }
    }
}