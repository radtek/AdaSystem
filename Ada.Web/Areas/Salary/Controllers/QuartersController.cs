using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Wages;
using Ada.Core.ViewModel.Wages;
using Ada.Framework.Filter;
using Ada.Services.Salary;

namespace Salary.Controllers
{
    /// <summary>
    /// 岗位
    /// </summary>
    public class QuartersController : BaseController
    {
        private readonly IQuartersService _service;
        private readonly IRepository<Quarters> _repository;
        public QuartersController(IQuartersService service, IRepository<Quarters> repository)
        {
            _service = service;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(QuartersView viewModel)
        {
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new QuartersView
                {
                    Id = d.Id,
                    Title = d.Title,
                    BaseSalary = d.BaseSalary,
                    Attendance = d.Attendance,
                    Allowance = d.Allowance,
                    Post = d.Post
                })
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add()
        {
            QuartersView viewModel = new QuartersView();
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(QuartersView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View();
            }
            Quarters entity = new Quarters();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedById = CurrentManager.Id;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedDate = DateTime.Now;
            entity.Title = viewModel.Title;
            entity.BaseSalary = viewModel.BaseSalary;
            entity.Allowance = viewModel.Allowance;
            entity.Commission = viewModel.Commission;
            entity.Attendance = viewModel.Attendance;
            entity.Post = viewModel.Post;
            entity.Training = viewModel.Training;
            _service.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            QuartersView entity = new QuartersView();
            entity.Id = item.Id;
            entity.Title = item.Title;
            entity.BaseSalary = item.BaseSalary;
            entity.Allowance = item.Allowance;
            entity.Commission = item.Commission;
            entity.Attendance = item.Attendance;
            entity.Post = item.Post;
            entity.Training = item.Training;
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(QuartersView viewModel)
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
            entity.BaseSalary = viewModel.BaseSalary;
            entity.Commission = viewModel.Commission;
            entity.Allowance = viewModel.Allowance;
            entity.Attendance = viewModel.Attendance;
            entity.Post = viewModel.Post;
            entity.Training = viewModel.Training;
            _service.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _service.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}