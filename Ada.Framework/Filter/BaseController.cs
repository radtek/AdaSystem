using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Admin;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Caching;
using Ada.Framework.UploadFile;
using Ada.Services.Admin;
using Ada.Services.Setting;
using Autofac;
using ClosedXML.Excel;
using log4net;
using MvcThrottle;
using Newtonsoft.Json;
using Action = Ada.Core.Domain.Admin.Action;

namespace Ada.Framework.Filter
{
    [AdminAntiForgery]
    public abstract class BaseController : Controller
    {

        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IRepository<Manager> _repository;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        protected BaseController()
        {
            _permissionService = EngineContext.Current.Resolve<IPermissionService>();
            _repository = EngineContext.Current.Resolve<IRepository<Manager>>();
            _cacheManager = EngineContext.Current.ContainerManager.Scope().Resolve<ICacheManager>(new TypedParameter(typeof(Type), GetType().BaseType));
            _signals = EngineContext.Current.Resolve<ISignals>();
            _settingService= EngineContext.Current.Resolve<ISettingService>();
        }

        public ILog Log { get; set; }
        public ManagerView CurrentManager { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //压缩
            string format = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
            if (!string.IsNullOrWhiteSpace(format))
            {
                if (format.Contains("gzip"))
                {
                    filterContext.HttpContext.Response.AddHeader("Content-Encoding","GZIP");
                    filterContext.HttpContext.Response.Filter=new GZipStream(filterContext.HttpContext.Response.Filter,CompressionMode.Compress);
                }
            }
            //如果打了允许的标签就无须验证权限
            ViewBag.TopMessage = TopMessage();
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }
            var obj = Session["LoginManager"];
            bool isAjax = Request.IsAjaxRequest();
            //校验是否登陆
            if (obj == null)
            {
                //没登陆
                Session["LastUrl"] = filterContext.HttpContext.Request.Url;
                filterContext.Result = isAjax
                    ? Json(new { State = 0, Msg = "您尚未登陆或登陆超时，请重新登陆！" }, JsonRequestBehavior.AllowGet)
                    : (ActionResult)RedirectToAction("Index", "Login", new { area = "Admin" });
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
                //Log.Warn(
                //    $"用户：{CurrentManager.UserName}(IP[{Utils.GetIpAddress()}]),请求[{actionRecord.Area + "/" + actionRecord.ControllerName + "/" + actionRecord.MethodName}]时,出现无权限访问情况！]");
                filterContext.Result = Error(isAjax, new HttpResultView() { HttpCode = 401, Msg = "您无此功能的操作权限！请联系管理员处理" });
            }



        }
        private ActionResult Error(bool isAjax, HttpResultView httpResultView)
        {
            if (isAjax)
            {
                return Json(new { State = 0, Msg = httpResultView.Msg }, JsonRequestBehavior.AllowGet);
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
        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public string ExportFile(string jsonStr, string sheetName = "江西微广")
        {
            var dt = JsonConvert.DeserializeObject<DataTable>(jsonStr);
            var fileName = "MangerExport_" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx";
            var fullPath = Server.MapPath("~/upload/" + fileName);
            using (var workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dt, sheetName);
                workbook.SaveAs(fullPath);
            }
            return fileName;
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="name">file字段名称</param>
        /// <returns>服务器路径</returns>
        public string UploadImageFile(string name)
        {
            UEditorModel uploadConfig = new UEditorModel()
            {
                AllowExtensions = UEditorConfig.GetStringList("imageAllowFiles"),
                PathFormat = UEditorConfig.GetString("imagePathFormat"),
                SizeLimit = UEditorConfig.GetInt("imageMaxSize"),
                UploadFieldName = name
            };
            var file = Request.Files[name];
            if (file == null) return null;
            try
            {
                if (string.IsNullOrWhiteSpace(file.FileName))
                {
                    return null;
                }
                var uploadFileName = file.FileName;
                var fileExtension = Path.GetExtension(uploadFileName).ToLower();
                if (!uploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension))
                {
                    throw new ApplicationException("请上传正确的图片格式文件");
                    //ModelState.AddModelError("message", "请上传正确的图片格式文件");
                }
                if (!(file.ContentLength < uploadConfig.SizeLimit))
                {
                    throw new ApplicationException("上传的图片最大只能为：" + uploadConfig.SizeLimit + "B");
                    //ModelState.AddModelError("message", "上传的图片最大只能为：" + uploadConfig.SizeLimit + "B");
                }
                var uploadFileBytes = new byte[file.ContentLength];
                file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);
                var savePath = PathFormatter.Format(uploadFileName, uploadConfig.PathFormat);
                var localPath = Server.MapPath(savePath);
                if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                }
                System.IO.File.WriteAllBytes(localPath, uploadFileBytes);
                return savePath;

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        private List<string> TopMessage()
        {
            List<string> msgs = new List<string>();
            //生日
            var dateNow = DateTime.Now.Date.Month;
            var birthdays = _repository.LoadEntities(d => d.Status == Consts.StateNormal && d.Birthday != null).ToList();
            ChineseLunisolarCalendar cc = new ChineseLunisolarCalendar();
            List<string> managerBirthdays = new List<string>();
            foreach (var birthday in birthdays)
            {
                var month = birthday.Birthday.Value.Month;
                if (birthday.IsLunar == true)
                {
                    var temp = cc.ToDateTime(DateTime.Now.Year, birthday.Birthday.Value.Month,
                        birthday.Birthday.Value.Day, 0, 0, 0, 0);
                    month = temp.Month;
                }
                if (dateNow == month)
                {
                    managerBirthdays.Add(birthday.UserName);
                }
            }
            if (managerBirthdays.Any())
            {
                msgs.Add("在这美好的" + DateTime.Now.Date.Month + "月，微广全体同事祝 <span class='text-danger'>" + string.Join(" ", managerBirthdays) + "</span> 生日快乐！");
            }
            //值日
            var zrSet = _settingService.GetSetting<WeiGuang>().WeiXinHolder;
            if (!string.IsNullOrWhiteSpace(zrSet))
            {
                msgs.Add("本月公司自运营号负责人：<span class='text-danger'>"+zrSet+"</span>");
            }

            return msgs;
        }
    }
}
