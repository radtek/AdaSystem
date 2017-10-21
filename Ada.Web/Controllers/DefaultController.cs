using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Admin;

namespace Ada.Web.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}