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
    /// 应收统计
    /// </summary>
    public class ReceivableController : BaseController
    {
        private readonly IReceivablesService _receivablesService;
        public ReceivableController(IReceivablesService receivablesService)
        {
            _receivablesService = receivablesService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(ReceivablesView viewModel)
        {
            var result = _receivablesService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {

                viewModel.total,
                rows = result.Select(d => new ReceivablesView
                {
                    Id = d.Id,
                    BillDate = d.BillDate,
                    BillNum = d.BillNum,
                    Money = d.Money,
                    TaxMoney = d.TaxMoney,
                    SettleAccountName = d.SettleAccount.SettleName,
                    IncomeExpendName = d.IncomeExpend.SubjectName,
                    TotalMoney = viewModel.TotalMoney,
                    TotalTaxMoney = viewModel.TotalTaxMoney


                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}