using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Customer;
using Ada.Services.Cache;

namespace Ada.Web.Models
{
    public class UserController:Controller
    {
        private readonly ICacheService _cacheService;
        public UserController()
        {
            _cacheService = EngineContext.Current.Resolve<ICacheService>();
        }
        public LinkManView CurrentUser { get; set; }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //如果打了允许的标签就无须验证权限
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }
             var session = Request.Cookies["UserSession"];
            if (session == null)
            {
                //没登陆
                filterContext.Result =RedirectToAction("Index", "Login");
                return;
            }
            string sessionId = Request.Cookies["UserSession"].Value;
            var user = _cacheService.GetObject<LinkManView>(sessionId) as LinkManView;
            if (user == null)
            {
                //缓存失效
                filterContext.Result = RedirectToAction("Index", "Login");
                return;
            }
            CurrentUser = user;
            ViewBag.CurrentUser = CurrentUser;
            _cacheService.Put(sessionId,user,new TimeSpan(1,0,0,0));
        }
    }
}