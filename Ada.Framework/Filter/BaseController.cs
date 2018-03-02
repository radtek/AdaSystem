using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Admin;
using Ada.Framework.Caching;
using Ada.Framework.Services;
using Ada.Services.Admin;
using Autofac;
using ClosedXML.Excel;
using log4net;
using Newtonsoft.Json;
using Action = Ada.Core.Domain.Admin.Action;

namespace Ada.Framework.Filter
{
    public abstract class BaseController : Controller
    {

        private readonly IPermissionService _permissionService;
        private readonly IRepository<Manager> _repository;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        protected BaseController()
        {
            _permissionService = EngineContext.Current.Resolve<IPermissionService>();
            _repository = EngineContext.Current.Resolve<IRepository<Manager>>();
            _cacheManager = EngineContext.Current.ContainerManager.Scope().Resolve<ICacheManager>(new TypedParameter(typeof(Type), GetType().BaseType));
            _signals = EngineContext.Current.Resolve<ISignals>();
        }

        public ILog Log { get; set; }
        public ManagerView CurrentManager { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //如果打了允许的标签就无须验证权限
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false)
                && filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false))
            {
                return;
            }
            var obj = Session["LoginManager"];
            bool isAjax = Request.IsAjaxRequest();
            //校验是否登陆
            if (obj == null)
            {
                //没登陆
                filterContext.Result = isAjax
                    ? Json(new { State = 0, Msg = "您尚未登陆或登陆超时，请重新登陆！" })
                    : (ActionResult)RedirectToAction("Index", "Login", new { area = "" });
                return;
            }
            CurrentManager = SerializeHelper.DeserializeToObject<ManagerView>(obj.ToString());
            var manager = _repository.LoadEntities(d => d.Id == CurrentManager.Id).FirstOrDefault();
            //当前用户
            CurrentManager.Image = manager.Image;//更新头像
            ViewBag.CurrentManager = CurrentManager;
            //用户登录日志
            var loginLogs = _cacheManager.Get("LoginLog" + CurrentManager.Id, c =>
             {
                 c.Monitor(_signals.When("LoginLog" + CurrentManager.Id + ".Changed"));
                 return manager.ManagerLoginLogs.OrderByDescending(d => d.Id).Take(5).ToList();
             });
            ViewBag.CurrentManagerLoginLog = loginLogs;
            //ViewBag.CurrentManagerLoginLog = manager.ManagerLoginLogs.OrderByDescending(d=>d.Id).Take(5).ToList();
            //用户菜单
            var menus = _cacheManager.Get("Menu" + CurrentManager.Id + CurrentManager.RoleId, c =>
                {
                    c.Monitor(_signals.When("Menu" + CurrentManager.Id + CurrentManager.RoleId + ".Changed"));
                    return _permissionService.AuthorizeMenu(CurrentManager.Id, CurrentManager.RoleId);
                });
            ViewBag.Menus = menus;
            //ViewBag.Menus = _permissionService.AuthorizeMenu(CurrentManager.Id);
            //////后门，用于调试
            //if (CurrentManager.UserName == "adaxiong")
            //{
            //    return;
            //}
            var actionRecord =
                new Action
                {
                    Area = filterContext.RouteData.DataTokens["area"]?.ToString() ?? string.Empty,
                    ControllerName = filterContext.RouteData.Values["controller"].ToString(),
                    MethodName = filterContext.RouteData.Values["action"].ToString(),
                    HttpMethod = Request.HttpMethod
                };
            //校验权限
            var hasPermission = _permissionService.Authorize(actionRecord, CurrentManager.Id, CurrentManager.RoleId);
            //1.根据获取的URL地址与请求的方式查询权限表。
            if (!hasPermission)
            {
                //写个用户日志
                Log.Warn(
                    $"用户：{CurrentManager.UserName}(IP[{Utils.GetIpAddress()}]),请求[{actionRecord.Area + "/" + actionRecord.ControllerName + "/" + actionRecord.MethodName}]时,出现无权限访问情况！]");
                filterContext.Result = Error(isAjax, new HttpResultView() { HttpCode = 401, Msg = "您无此功能的操作权限！请联系管理员处理" });
            }



        }
        private ActionResult Error(bool isAjax, HttpResultView httpResultView)
        {
            if (isAjax)
            {
                return Json(new { State = 0, Msg = httpResultView.Msg });
            }
            ViewResult result = new ViewResult
            {
                ViewName = "Error", //错误页
                ViewData = new ViewDataDictionary(httpResultView),       //指定模型
            };
            return result;
        }
        /// <summary>
        /// 清除菜单缓存
        /// </summary>
        /// <param name="key"></param>
        public void ClearCacheByManagers(string key)
        {
            var managers = _repository.LoadEntities(d => d.IsDelete == false).ToList();
            foreach (var manager in managers)
            {
                foreach (var managerRole in manager.Roles)
                {
                    _signals.Trigger(key + manager.Id + managerRole.Id + ".Changed");
                }

            }

        }
        /// <summary>
        /// 获取数据范围
        /// </summary>
        public List<string> PremissionData()
        {
            return _permissionService.AuthorizeData(CurrentManager.Id, CurrentManager.RoleId);
        }
        /// <summary>
        /// 获取数据范围
        /// </summary>
        public bool IsPremission(Action action)
        {
            return _permissionService.Authorize(action, CurrentManager.Id);
        }
        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public byte[] ExportData(string jsonStr)
        {
            var dt = JsonConvert.DeserializeObject<DataTable>(jsonStr);
            byte[] bytes;
            using (var workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dt, "江西微广");
                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    bytes = ms.ToArray();
                }
            }
            return bytes;
        }
    }
}
