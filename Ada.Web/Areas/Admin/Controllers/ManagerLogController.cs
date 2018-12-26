using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Log;
using Ada.Core.ViewModel.Admin;
using Ada.Framework.Filter;
using Ada.Services.Admin;

namespace Admin.Controllers
{
    /// <summary>
    /// 用户登陆日志
    /// </summary>
    public class ManagerLogController : BaseController
    {
        // IManagerLoginLogService
        private readonly IManagerLoginLogService _managerLoginLogService;
        private readonly IRepository<ManagerLoginLog> _repository;

        public ManagerLogController(IManagerLoginLogService managerLoginLogService, IRepository<ManagerLoginLog> repository)
        {
            _managerLoginLogService = managerLoginLogService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(ManagerLoginLogView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _managerLoginLogService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new ManagerLoginLogView
                {
                    Id = d.Id,
                    LoginTime = d.LoginTime,
                    WebInfo = d.WebInfo,
                    UserName = d.Manager.UserName,
                    Remark = d.Remark,
                    Ip = d.IpAddress,
                    Image = d.Manager.Image

                    
                })
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _managerLoginLogService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}