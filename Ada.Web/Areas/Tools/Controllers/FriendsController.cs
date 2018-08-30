using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;
using Tools.Models;

namespace Tools.Controllers
{
    public class FriendsController : BaseController
    {
        // GET: Friends
        public ActionResult Index()
        {
            FriendsSet view=new FriendsSet();
            return View(view);
        }
        [AllowAnonymous]
        public ActionResult Preview()
        {
            return View();
        }
    }
}