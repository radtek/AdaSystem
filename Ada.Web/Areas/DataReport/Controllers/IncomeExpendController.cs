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
    /// 其他收支统计
    /// </summary>
    public class IncomeExpendController : BaseController
    {
        private readonly IExpenseDetailService _expenseDetailService;
        public IncomeExpendController(IExpenseDetailService expenseDetailService)
        {
            _expenseDetailService = expenseDetailService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(ReceiptExpenditureView viewModel)
        {
            var result = _expenseDetailService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {

                viewModel.total,
                rows = result.Select(d => new ReceiptExpenditureView
                {
                    Id = d.ExpenseId,
                    BillDate = d.Expense.BillDate,
                    BillNum = d.Expense.BillNum,
                    ExpenditureMoney = d.IncomeExpend.SubjectType == Consts.StateLock ? d.Money : 0,
                    ReceiptMoney = d.IncomeExpend.SubjectType == Consts.StateNormal ? d.Money : 0,
                    SettleAccountName = d.SettleAccount.SettleName,
                    IncomeExpendName = d.IncomeExpend.SubjectName,
                    TotalReceiptMoney = viewModel.TotalReceiptMoney,
                    TotalExpenditureMoney = viewModel.TotalExpenditureMoney,
                    SubjectType = d.IncomeExpend.SubjectType

                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}