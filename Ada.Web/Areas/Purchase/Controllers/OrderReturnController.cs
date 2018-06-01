using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Purchase.Controllers
{
    /// <summary>
    /// 采购退款
    /// </summary>
    public class OrderReturnController : BaseController
    {
        // GET: OrderReturn
        public ActionResult Index()
        {
            return View();
        }
    }
}