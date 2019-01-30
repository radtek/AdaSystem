using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Vote.Controllers
{
    public class RecordController : BaseController
    {
        // GET: Record
        public ActionResult Index()
        {
            return View();
        }
    }
}