using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Admin.Controllers
{
    public class ActionController : BaseController
    {
        // GET: Action
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update()
        {
            return Json(null);
        }
    }
}