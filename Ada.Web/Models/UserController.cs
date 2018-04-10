using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Customer;

namespace Ada.Web.Models
{
    public class UserController:Controller
    {
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
            var obj = Session["User"];
            if (obj == null)
            {
                //没登陆
                filterContext.Result =RedirectToAction("Index", "Login");
                return;
            }
            CurrentUser= SerializeHelper.DeserializeToObject<LinkManView>(obj.ToString());
        }
    }
}