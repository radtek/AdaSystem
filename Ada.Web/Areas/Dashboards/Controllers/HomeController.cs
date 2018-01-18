using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Finance;
using Ada.Core.Domain.Purchase;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Admin;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Resource;
using Ada.Core.ViewModel.Statistics;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Ada.Services.Business;
using Action = Ada.Core.Domain.Admin.Action;

namespace Dashboards.Controllers
{
    public class HomeController : BaseController
    {
        
        public ActionResult Index()
        {
            var area = ControllerContext.RouteData.DataTokens["area"]?.ToString() ?? string.Empty;
            var methodName = ControllerContext.RouteData.Values["action"].ToString();
            var bossAction =
                new Action
                {
                    Area = area,
                    ControllerName = "Boss",
                    MethodName = methodName,
                    HttpMethod = Request.HttpMethod
                };
            var bosspremission = IsPremission(bossAction);
            if (bosspremission)
            {
                return RedirectToAction("Index", "Boss");
            }

            var businessAction =
                new Action
                {
                    Area = area,
                    ControllerName = "Business",
                    MethodName = methodName,
                    HttpMethod = Request.HttpMethod
                };
            var bpremission = IsPremission(businessAction);
            if (bpremission)
            {
                return RedirectToAction("Index", "Business");
            }
            var purchaseAction =
                new Action
                {
                    Area = area,
                    ControllerName = "Purchase",
                    MethodName = methodName,
                    HttpMethod = Request.HttpMethod
                };
            var ppremission = IsPremission(purchaseAction);
            if (ppremission)
            {
                return RedirectToAction("Index", "Purchase");
            }
            return View();

        }





    }
}