using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Finance;
using Ada.Framework.Filter;
using Ada.Services.Finance;

namespace Finance.Controllers
{
    public class IncomeExpendController : BaseController
    {
        private readonly IIncomeExpendService _incomeExpendService;
        private readonly IRepository<IncomeExpend> _repository;
        public IncomeExpendController(IIncomeExpendService incomeExpendService, IRepository<IncomeExpend> repository)
        {
            _incomeExpendService = incomeExpendService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList()
        {
            var result = _repository.LoadEntities(d => d.IsDelete == false).OrderBy(d=>d.Taxis).ToList();
            return Json(new
            {
                total = result.Count,
                rows = result.Select(d => new IncomeExpendView
                {
                    Id = d.Id,
                    SubjectName = d.SubjectName,
                    SubjectNum = d.SubjectNum,
                    SubjectType = d.SubjectType,
                    IsMain = d.IsMain,
                    ParentId = d.ParentId,
                    Taxis = d.Taxis
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(IncomeExpendView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }

            IncomeExpend entity = new IncomeExpend();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedById = CurrentManager.Id;
            entity.AddedDate = DateTime.Now;
            entity.SubjectName = viewModel.SubjectName;
            entity.SubjectNum = viewModel.SubjectNum;
            entity.SubjectType = viewModel.SubjectType;
            entity.IsMain = viewModel.IsMain;
            entity.ParentId = viewModel.ParentId;
            entity.Taxis = viewModel.Taxis;
            _incomeExpendService.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            IncomeExpendView viewModel = new IncomeExpendView();
            viewModel.Id = id;
            viewModel.SubjectName = entity.SubjectName;
            viewModel.SubjectNum = entity.SubjectNum;
            viewModel.SubjectType = entity.SubjectType;
            viewModel.IsMain = entity.IsMain;
            viewModel.ParentId = entity.ParentId;
            viewModel.Taxis = entity.Taxis;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(IncomeExpendView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }

            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedDate = DateTime.Now;
            entity.SubjectName = viewModel.SubjectName;
            entity.SubjectNum = viewModel.SubjectNum;
            entity.SubjectType = viewModel.SubjectType;
            entity.IsMain = viewModel.IsMain;
            entity.ParentId = viewModel.ParentId;
            entity.Taxis = viewModel.Taxis;
            _incomeExpendService.Update(entity);
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
            _incomeExpendService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}