using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;
using Ada.Services.Setting;
using Message.Models;
using Newtonsoft.Json;

namespace Message.Controllers
{
    public class AliYunController : BaseController
    {
        private readonly ISettingService _settingService;
        private readonly IAliYunSmsChannel _aliYunSmsChannel;
        public AliYunController(ISettingService settingService, IAliYunSmsChannel aliYunSmsChannel)
        {
            _settingService = settingService;
            _aliYunSmsChannel = aliYunSmsChannel;
        }
        public ActionResult Index()
        {
            AliYunSet entity = _settingService.GetSetting<AliYunSet>();
            return View(entity);
        }
        [HttpPost]
        
        public ActionResult Index(AliYunSet entity)
        {
            var setting = new Ada.Core.Domain.Admin.Setting
            {
                SettingName = typeof(AliYunSet).Name,
                Content = JsonConvert.SerializeObject(entity)
            };
            _settingService.AddOrUpdate(setting);
            TempData["Msg"] = "保存成功";
            return View(entity);

        }
        [HttpPost]
        
        public ActionResult TestSend(AliYunMessage aliYunMessage)
        {
            _aliYunSmsChannel.Process(new Dictionary<string, object> {
                {"PhoneNumbers", aliYunMessage.PhoneNumbers},
                {"TemplateCode", aliYunMessage.TemplateCode},
                {"TemplateParam", aliYunMessage.TemplateParam}
            });
            return Json(new{State=1,Msg="提交成功"});
        }
    }
}