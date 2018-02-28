using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Purchase.Controllers
{
    public class OrderDetailSpecController : BaseController
    {
        // GET: OrderDetailSpec
        public ActionResult Index()
        {
            return View();
        }
    }
}