using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.Infrastructure;
using Ada.Framework.Filter;
using log4net;

namespace Ada.Web.Controllers
{
    [UserException]
    public class DefaultController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Test()
        {
            return View();
        }
    }
}