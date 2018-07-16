using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Customer;
using Ada.Framework.Filter;
using Ada.Services.Cache;
using Ada.Services.Customer;
using ClosedXML.Excel;
using Newtonsoft.Json;

namespace Ada.Web.Models
{
    [UserException]
    public class UserController:Controller
    {
        private readonly ICacheService _cacheService;
        private readonly ILinkManService _linkManService;
        public UserController()
        {
            _cacheService = EngineContext.Current.Resolve<ICacheService>();
            _linkManService = EngineContext.Current.Resolve<ILinkManService>();
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
            //校验是否开通
            var temp = _linkManService.CheackUser(user.LoginName);
            if (temp==null)
            {
                //账户暂未开通
                //清缓存 清COOKIE
                _cacheService.Remove(sessionId);
                session.Expires = DateTime.Now.AddDays(-999);
                filterContext.Result = RedirectToAction("Index", "Login");
                return;
            }
            CurrentUser = user;
            ViewBag.CurrentUser = CurrentUser;
            _cacheService.Put(sessionId,user,new TimeSpan(1,0,0,0));
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public string ExportData(string jsonStr,string sheetName= "我的数据")
        {
            var dt = JsonConvert.DeserializeObject<DataTable>(jsonStr);
            var fileName = "UserExport_" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx";
            var fullPath = Server.MapPath("~/upload/" + fileName);
            using (var workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dt, sheetName);
                workbook.SaveAs(fullPath);
            }
            return fileName;
        }

        public string ExportData(IDictionary<string,string> dic)
        {
           
            var fileName = "UserExport_" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx";
            var fullPath = Server.MapPath("~/upload/" + fileName);
            using (var workbook = new XLWorkbook())
            {
                foreach (KeyValuePair<string, string> keyValuePair in dic)
                {
                    var dt = JsonConvert.DeserializeObject<DataTable>(keyValuePair.Value);
                    workbook.Worksheets.Add(dt, keyValuePair.Key);
                }
                workbook.SaveAs(fullPath);
            }
            return fileName;
        }
    }
}