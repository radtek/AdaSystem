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
    public class BusinessOrderDetailController : BaseController
    {
        private readonly IBusinessOrderDetailService _businessOrderDetailService;
        public BusinessOrderDetailController(IBusinessOrderDetailService businessOrderDetailService)
        {
            _businessOrderDetailService = businessOrderDetailService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessOrderDetailView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _businessOrderDetailService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessOrderDetailView
                {
                    Id = d.Id,
                    VerificationMoney = d.VerificationMoney,
                    MediaName = d.MediaName,
                    MediaTypeName = d.MediaTypeName,
                    AdPositionName = d.AdPositionName,
                    Money = d.Money,
                    TaxMoney = d.TaxMoney,
                    Tax = d.Tax,
                    SellMoney = d.SellMoney,
                    

                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}