using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Domain;
using Ada.Core.ViewModel.Finance;
using Ada.Framework.Filter;
using Ada.Services.Finance;

namespace DataReport.Controllers
{
    /// <summary>
    /// 应付统计
    /// </summary>
    public class PayableController : BaseController
    {
        private readonly IBillPaymentDetailService _billPaymentDetailService;

        public PayableController(IBillPaymentDetailService billPaymentDetailService)
        {
            _billPaymentDetailService = billPaymentDetailService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(ReceiptExpenditureView viewModel)
        {
            var result = _billPaymentDetailService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {

                viewModel.total,
                rows = result.Select(d => new ReceiptExpenditureView
                {
                    Id = d.BillPaymentId,
                    BillDate = d.BillPayment.BillDate,
                    BillNum = d.BillPayment.BillNum,
                    ExpenditureMoney = d.Money,
                    SettleAccountName = d.SettleAccount.SettleName,
                    IncomeExpendName = d.IncomeExpend.SubjectName,
                    TotalExpenditureMoney = viewModel.TotalExpenditureMoney,
                    SubjectType = d.IncomeExpend.SubjectType

                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}