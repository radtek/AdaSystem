using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;
using Ada.Services.Setting;
using Newtonsoft.Json;
using Salary.Models;

namespace Salary.Controllers
{
    public class SettingController : BaseController
    {
        private readonly ISettingService _settingService;

        public SettingController(ISettingService settingService)
        {
            _settingService = settingService;
        }
        public ActionResult Index()
        {
            var entity = _settingService.GetSetting<SalarySet>();
            return View(entity);
        }
        [HttpPost]
        
        public ActionResult Index(SalarySet entity)
        {
            var setting = new Ada.Core.Domain.Admin.Setting
            {
                SettingName = typeof(SalarySet).Name,
                Content = JsonConvert.SerializeObject(entity)
            };
            _settingService.AddOrUpdate(setting);
            TempData["Msg"] = "保存成功";
            return View(entity);

        }
    }
}