using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Tools;
using Ada.Core.ViewModel.WeiXin;
using Ada.Services.Admin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.Open.QRConnect;
using WeiXin.Filters;
using WeiXin.Services;

namespace WeiXin.Controllers
{
    public class BindingController : Controller
    {
        private IWeiXinService _service;
        private readonly IManagerService _managerService;
        public BindingController(IWeiXinService service, IManagerService managerService)
        {
            _service = service;
            _managerService = managerService;


        }
        [WeiXinOAuth("/WeiXin/OAuth2")]
        public ActionResult Index()
        {
            var obj = Session["CurrentWeiXin"];
            var currentWxUser = SerializeHelper.DeserializeToObject<UserInfoJson>(obj.ToString());
            return View(currentWxUser);
        }

        public ActionResult Manger(string openid)
        {
            var account = _managerService.GetMangerByOpenId(openid);
            return View(account);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manger(string userName, string password)
        {
            var obj = Session["CurrentWeiXin"];
            if (obj == null)
            {
                return RedirectToAction("Index");
            }
            var currentWxUser = SerializeHelper.DeserializeToObject<UserInfoJson>(obj.ToString());
            //校验用户
            var isBinding = _managerService.BindingOpenId(userName.Trim(), password, currentWxUser.openid, out string msg,
                 currentWxUser.headimgurl);
            if (!isBinding)
            {
                ModelState.AddModelError("message", msg);
                return View();
            }
            return RedirectToAction("Manger", new { currentWxUser.openid });
        }

        
    }
}