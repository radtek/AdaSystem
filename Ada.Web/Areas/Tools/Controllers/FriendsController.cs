using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Tools.Controllers
{
    public class FriendsController : BaseController
    {
        // GET: Friends
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Preview()
        {
            return View();
        }
    }
}