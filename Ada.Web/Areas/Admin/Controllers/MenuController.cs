using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Admin.Controllers
{
    public class MenuController : BaseController
    {
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }
    }
}