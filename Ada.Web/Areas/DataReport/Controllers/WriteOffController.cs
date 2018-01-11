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
            var result = _businessWriteOffService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d=>new
                {
                    d.Id,
                    d.BusinessMoney,
                    d.Commission,
                    d.PurchaseMoney,
                    d.Profit,
                    d.OrderNum,
                    d.OrderId,
                    d.LinkManName,
                    d.PublishDate,
                    d.WriteOffDate,
                    d.ReturnDays,
                    d.Transactor,
                    d.Percentage,
                    viewModel.TotalCommission,
                    viewModel.TotalBusinessMoney,
                    viewModel.TotalProfit,
                    viewModel.TotalPurchaseMoney

                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}