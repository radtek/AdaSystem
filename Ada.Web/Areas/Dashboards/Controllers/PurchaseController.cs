using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Dashboards.Controllers
{
    public class PurchaseController : BaseController
    {
        // GET: Purchase
        public ActionResult Index()
        {
            return View();
        }
    }
}