using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.ViewModel.WorkLog;
using Ada.Framework.Filter;
using Ada.Services.WorkLog;

namespace WorkLog.Controllers
{
    public class DailyController : BaseController
    {
        private readonly IWorkLogService _workLogService;
        private readonly IRepository<Ada.Core.Domain.Log.WorkLog> _repository;
        public DailyController(IRepository<Ada.Core.Domain.Log.WorkLog> repository, IWorkLogService workLogService)
        {
            _workLogService = workLogService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(WorkLogView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _workLogService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new WorkLogView
                {
                    Id = d.Id,
                    Content = d.Content,
                    Title = d.Title,
                    Manager = d.Manager,
                    Date = d.Date,
                    Transactor = d.Transactor,
                    Director = d.Director,
                    Boss = d.Boss
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(WorkLogView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            Ada.Core.Domain.Log.WorkLog entity = new Ada.Core.Domain.Log.WorkLog();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedById = CurrentManager.Id;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedDate = DateTime.Now;
            entity.Title = viewModel.Title;
            entity.Content = viewModel.Content;
            entity.Manager = viewModel.Manager;
            entity.Boss = viewModel.Boss;
            entity.Director = viewModel.Director;
            entity.Date = DateTime.Now;
            entity.Transactor = CurrentManager.UserName;
            entity.TransactorId = CurrentManager.Id;
            _workLogService.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            WorkLogView entity = new WorkLogView();
            entity.Id = item.Id;
            entity.Title = item.Title;
            entity.Content = item.Content;
            entity.Boss = item.Boss;
            entity.Director = item.Director;
            entity.Manager = item.Manager;
            entity.Transactor = item.Transactor;
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(WorkLogView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.Title = viewModel.Title;
            entity.Content = viewModel.Content;
            entity.Manager = viewModel.Manager;
            entity.Boss = viewModel.Boss;
            entity.Director = viewModel.Director;
            _workLogService.Update(entity);
            TempData["Msg"] = "操作成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _workLogService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}