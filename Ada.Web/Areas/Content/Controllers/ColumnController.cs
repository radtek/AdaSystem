using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Content;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Content;
using Ada.Framework.Filter;
using Ada.Services.Content;

namespace Content.Controllers
{
    public class ColumnController : BaseController
    {
        private readonly IColumnService _service;
        private readonly IRepository<Column> _repository;

        public ColumnController(IColumnService service, IRepository<Column> repository)
        {
            _service = service;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList()
        {
            var result = _repository.LoadEntities(d => d.IsDelete == false).OrderBy(d=>d.Taxis).ToList();
            return Json(result.Select(d => new
            {
                d.Id,
                d.ParentId,
                d.Title,
                d.Articles.Count
            }), JsonRequestBehavior.AllowGet);
        }
        private List<TreeView> GetTree(string parentId, List<Column> entities)
        {
            var newlist = entities.Where(d => d.ParentId == parentId).ToList();
            List<TreeView> treeViews = new List<TreeView>();
            newlist.ForEach(item =>
            {
                treeViews.Add(new TreeView
                {
                    Id = item.Id,
                    Children = GetTree(item.Id, entities),
                    ParentId = item.ParentId,
                    Text = item.Title
                });
            });
            return treeViews;
        }
        public ActionResult Add()
        {
            var entities = _repository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis).ToList();
            ViewBag.Trees = GetTree(null, entities);
            return View(new ColumnView() { Taxis = 99 });
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ColumnView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            IDictionary idc = new Dictionary<string, string>();
            foreach (var filesAllKey in Request.Files.AllKeys)
            {
                var path = UploadImageFile(filesAllKey);
                idc.Add(filesAllKey, path);
            }
            Column column = new Column
            {
                Id = IdBuilder.CreateIdNum(),
                AddedById = CurrentManager.Id,
                AddedBy = CurrentManager.UserName,
                AddedDate = DateTime.Now,
                Content = viewModel.Content,
                Title = viewModel.Title,
                ParentId = viewModel.ParentId,
                CallIndex = viewModel.CallIndex,
                CoverPic = idc["CoverPic"]?.ToString(),
                Taxis = viewModel.Taxis,
                Url = viewModel.Url,
                Remark = viewModel.Remark
            };
            _service.Add(column);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            ColumnView item = new ColumnView
            {
                Id = entity.Id,
                Title = entity.Title,
                ParentId = entity.ParentId,
                Content = entity.Content,
                CallIndex = entity.CallIndex,
                CoverPic = entity.CoverPic,
                Taxis = entity.Taxis,
                Url = entity.Url,
                Remark = entity.Remark
            };
            var entities = _repository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis).ToList();
            ViewBag.Trees = GetTree(null, entities);
            return View(item);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Update(ColumnView viewModel)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            IDictionary idc = new Dictionary<string, string>();
            foreach (var filesAllKey in Request.Files.AllKeys)
            {
                var path = UploadImageFile(filesAllKey);
                idc.Add(filesAllKey, path);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.Title = viewModel.Title;
            entity.ParentId = viewModel.ParentId;
            entity.Content = viewModel.Content;
            entity.CallIndex = viewModel.CallIndex;
            entity.CoverPic = idc["CoverPic"] == null ? entity.CoverPic : idc["CoverPic"]?.ToString();
            entity.Taxis = viewModel.Taxis;
            entity.Url = viewModel.Url;
            entity.Remark = viewModel.Remark;
            _service.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _service.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}