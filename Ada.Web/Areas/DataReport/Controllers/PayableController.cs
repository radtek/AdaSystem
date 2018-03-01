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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Export(ReceiptExpenditureView viewModel)
        {
            viewModel.limit = 5000;
            var result = _billPaymentDetailService.LoadEntitiesFilter(viewModel).ToList();
            JArray jObjects = new JArray();
            foreach (var item in result)
            {
                var jo = new JObject();
                jo.Add("单据日期", item.BillPayment.BillDate);
                jo.Add("支出项目", item.IncomeExpend.SubjectName);
                jo.Add("结算账户", item.SettleAccount.SettleName);
                jo.Add("支付金额", item.Money);
                jObjects.Add(jo);
            }
            return File(ExportData(jObjects.ToString()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
        }
    }
}