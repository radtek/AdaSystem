using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Filter;
using Ada.Services.Setting;
using Newtonsoft.Json;


namespace Setting.Controllers
{
    public class SiteController : BaseController
    {
        private readonly ISettingService _settingService;
        private const string SettingName = "Site";
        public SiteController(ISettingService settingService)
        {
            _settingService = settingService;
        }
        public ActionResult Index()
        {
            Site site = _settingService.GetSetting<Site>(SettingName);
            return View(site);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Site site)
        {
            var setting = new Ada.Core.Domain.Admin.Setting
            {
                SettingName = SettingName,
                Content = JsonConvert.SerializeObject(site)
            };
            _settingService.AddOrUpdate(setting);
            TempData["Msg"] = "保存成功";
            return View(site);

        }
    }
}