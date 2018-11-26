using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;
using WebSocket.Hubs;

namespace WebSocket.Controllers
{
    public class PushController : BaseController
    {
        // GET: Test
        public ActionResult Index()
        {
            return View(PushHub.OnLineUsers);
        }
    }
}