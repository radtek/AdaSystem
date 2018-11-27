using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;
using Microsoft.AspNet.SignalR;
using WebSocket.Hubs;
using WebSocket.Models;

namespace WebSocket.Controllers
{
    public class PushController : BaseController
    {
        // GET: Test
        public ActionResult Index()
        {
            return View(new HubView());
        }
        [HttpPost,AdaValidateAntiForgeryToken]
        public ActionResult Index(HubView hubView)
        {
            hubView.From = CurrentManager.UserName;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<PushHub>();
            hubContext.Clients.All.recive(hubView);
            return Json(new {State = 1, Msg = "推送成功"});
        }
    }
}