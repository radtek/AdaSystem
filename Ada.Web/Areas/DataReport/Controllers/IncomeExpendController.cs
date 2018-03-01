using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Domain;
using Ada.Core.ViewModel.Finance;
using Ada.Framework.Filter;
using Ada.Services.Finance;
using Newtonsoft.Json.Linq;

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
                    SubjectType = d.IncomeExpend.SubjectType,
                    Employe = d.Expense.Employe

                })
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Export(ReceiptExpenditureView viewModel)
        {
            viewModel.limit = 5000;
            var result = _expenseDetailService.LoadEntitiesFilter(viewModel).ToList();
            JArray jObjects = new JArray();
            foreach (var item in result)
            {
                var jo = new JObject();
                jo.Add("单据日期", item.Expense.BillDate);
                jo.Add("业务员", item.Expense.Employe == "请输入关键字" ? "" : item.Expense.Employe);
                jo.Add("收支项目", item.IncomeExpend.SubjectName);
                jo.Add("结算账户", item.SettleAccount.SettleName);
                if (item.IncomeExpend.SubjectType == Consts.StateNormal)
                {
                    jo.Add("收入金额", item.Money);
                }
                if (item.IncomeExpend.SubjectType == Consts.StateLock)
                {
                    jo.Add("支出金额", item.Money);
                }
                
                
                jObjects.Add(jo);
            }
            return File(ExportData(jObjects.ToString()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
        }
    }
}