using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.Infrastructure;
using log4net;

namespace Ada.Web.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult Index()
        {
            throw new ApplicationException("测试");
            return View();
        }
    }
}