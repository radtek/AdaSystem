using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Domain.Business;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Filter;
using Ada.Services.Business;
using Ada.Services.Setting;
using Salary.Models;

namespace Salary.Controllers
{
    public class PercentageController : BaseController
    {
        private readonly IBusinessWriteOffService _businessWriteOffService;
        private readonly ISettingService _settingService;

        public PercentageController(IBusinessWriteOffService businessWriteOffService, ISettingService settingService)
        {
            _businessWriteOffService = businessWriteOffService;
            _settingService = settingService;
        }
        public ActionResult Index()
        {
            return View(new PercentageView());
        }
        [HttpPost]
        public ActionResult Index(PercentageView view)
        {
            var rangeDate = view.DateRange.Split('至');
            var start = DateTime.Parse(rangeDate[0]);
            var end = DateTime.Parse(rangeDate[1]).AddDays(1);
            Expression<Func<BusinessWriteOffDetail, bool>> where = d => d.PublishDate >= start &&
                                                                        d.PublishDate < end &&
                                                                        view.MediaTypeIds.Contains(d.MediaTypeId);
            if (view.TransactorIds.Any())
            {
                where = d => d.PublishDate >= start &&
                             d.PublishDate < end &&
                             view.MediaTypeIds.Contains(d.MediaTypeId) &&
                             view.TransactorIds.Contains(d.BusinessWriteOff.TransactorId);
            }
            var setting = _settingService.GetSetting<WeiGuang>();
            var result = _businessWriteOffService.UpdateDetail(where,
                 d => new BusinessWriteOffDetail()
                 {
                     Remark = view.Title,
                     Percentage = d.MoneyBackDay >= setting.ReturnDays1 && d.MoneyBackDay <= setting.ReturnDays2 ? view.Percentage * 0.8M : d.MoneyBackDay > setting.ReturnDays2 ? 0 : view.Percentage,
                     Commission = d.Profit * (d.MoneyBackDay >= setting.ReturnDays1 && d.MoneyBackDay <= setting.ReturnDays2 ? view.Percentage * 0.8M : d.MoneyBackDay > setting.ReturnDays2 ? 0 : view.Percentage)
                 });
            return Json(new { State = 1, Msg = "成功更新" + result + "条提成明细" });
        }
    }
}