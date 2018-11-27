using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;
using WebSocket.Hubs;

namespace WebSocket.Controllers
{
    public class ChatRoomController : BaseController
    {
        // GET: ChatRoom
        public ActionResult Index()
        {
            return View(ChatHub.OnLineUsers);
        }
    }
}