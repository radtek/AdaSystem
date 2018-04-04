using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Resource.Controllers
{
    public class DouYinController : BaseController
    {
        // GET: DouYin
        public ActionResult Index()
        {
            return View();
        }
    }
}