using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Business;

namespace DataReport.Controllers
{
    /// <summary>
    /// 提成明细（核销明细）
    /// </summary>
    public class WriteOffController : BaseController
    {
        private readonly IBusinessWriteOffService _businessWriteOffService;
        public WriteOffController(IBusinessWriteOffService businessWriteOffService)
        {
            _businessWriteOffService = businessWriteOffService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessWriteOffDetailView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _businessWriteOffService.LoadEntitiesFiltersPage(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d=>new BusinessWriteOffDetailView
                {
                   Id = d.Id,
                    BusinessMoney= d.SellMoney,
                    Commission= d.Commission,
                    PurchaseMoney= d.CostMoney,
                    Profit=  d.Profit,
                    OrderId= d.BusinessOrderId,
                    //d.OrderId,
                    //d.LinkManName,
                    PublishDate= d.PublishDate,
                    WriteOffDate= d.BusinessWriteOff.WriteOffDate,
                    ReturnDays= d.MoneyBackDay,
                    Transactor= d.BusinessWriteOff.Transactor,
                    Percentage= d.Percentage,
                    TotalCommission= viewModel.TotalCommission,
                    TotalBusinessMoney= viewModel.TotalBusinessMoney,
                    TotalProfit= viewModel.TotalProfit,
                    TotalPurchaseMoney=viewModel.TotalPurchaseMoney

                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}