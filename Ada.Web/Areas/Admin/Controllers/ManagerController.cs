using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Admin.Controllers
{
    public class ManagerController : BaseController
    {
        // GET: Manager
        public ActionResult Index()
        {
            return View();
        }
    }
}