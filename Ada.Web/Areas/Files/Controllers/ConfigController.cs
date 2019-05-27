using System.Web.Mvc;
using Ada.Framework.Filter;
using Ada.Services.Setting;
using Files.Models;
using Newtonsoft.Json;

namespace Files.Controllers
{
    public class ConfigController : BaseController
    {
        private readonly ISettingService _settingService;

        public ConfigController(ISettingService settingService)
        {
            _settingService = settingService;
        }
        public ActionResult Index()
        {
            FileConfig entity = _settingService.GetSetting<FileConfig>();
            return View(entity);
        }
        [HttpPost]
        public ActionResult Index(FileConfig entity)
        {
            var setting = new Ada.Core.Domain.Admin.Setting
            {
                SettingName = typeof(FileConfig).Name,
                Content = JsonConvert.SerializeObject(entity)
            };
            _settingService.AddOrUpdate(setting);
            TempData["Msg"] = "保存成功";
            return View(entity);

        }
    }
}