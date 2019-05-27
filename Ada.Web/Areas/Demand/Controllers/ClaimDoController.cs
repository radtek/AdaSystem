using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Demand.Controllers
{
    public class ClaimDoController : BaseController
    {
        // GET: ClaimDo
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Self()
        {
            return View();
        }
    }
}