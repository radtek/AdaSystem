using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Filter;
using Ada.Services.Setting;
using Newtonsoft.Json;

namespace Setting.Controllers
{
    public class WeiGuangController : BaseController
    {
        private readonly ISettingService _settingService;
        public WeiGuangController(ISettingService settingService)
        {
            _settingService = settingService;
        }
        public ActionResult Index()
        {
            WeiGuang entity = _settingService.GetSetting<WeiGuang>();
            return View(entity);
        }
        [HttpPost]
        
        public ActionResult Index(WeiGuang entity)
        {
            var setting = new Ada.Core.Domain.Admin.Setting
            {
                SettingName = typeof(WeiGuang).Name,
                Content = JsonConvert.SerializeObject(entity)
            };
            _settingService.AddOrUpdate(setting);
            TempData["Msg"] = "保存成功";
            return View(entity);

        }
    }
}