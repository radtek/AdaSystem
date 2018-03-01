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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Export(ReceivablesView viewModel)
        {
            viewModel.limit = 5000;
            var result = _receivablesService.LoadEntitiesFilter(viewModel).ToList();
            JArray jObjects = new JArray();
            foreach (var item in result)
            {
                var jo = new JObject();
                jo.Add("单据日期", item.BillDate);
                jo.Add("收支项目", item.IncomeExpend.SubjectName);
                jo.Add("结算账户", item.SettleAccount.SettleName);
                jo.Add("收入金额", item.Money);
                jo.Add("税额", item.TaxMoney);
                jObjects.Add(jo);
            }
            return File(ExportData(jObjects.ToString()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
        }
    }
}