using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.ViewModel.Admin;
using Ada.Framework.Filter;
using Ada.Services.Admin;

namespace Admin.Controllers
{
    public class SystemLogController : BaseController
    {
        private readonly ISystemLogService _systemLogService;
        public SystemLogController(ISystemLogService systemLogService)
        {
            _systemLogService = systemLogService;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(SystemLogView viewModel)
        {
            var result = _systemLogService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new SystemLogView
                {
                    Id = d.Id.ToString(),
                    Level = d.Level,
                    Logger = d.Logger,
                    Message = d.Message,
                    Exception = d.Exception,
                    Date = d.Date.Value.ToString("yyyy-MM-dd HH:mm:ss")
                })
            },JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        
        public ActionResult Delete()
        {
            var ids = Request["Ids"].Split(',');
            _systemLogService.Delete(ids);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}