using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Finance.Controllers
{
    public class BusinessPayController : BaseController
    {
        // GET: BusinessPay
        public ActionResult Index()
        {
            return View();
        }
    }
}