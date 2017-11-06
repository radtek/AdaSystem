using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Admin;
using Ada.Framework.Filter;
using Ada.Services.Admin;

namespace Admin.Controllers
{
    public class FieldController : BaseController
    {
        private readonly IFieldService _fieldService;
        private readonly IRepository<Field> _fieldRepository;
        private readonly IFieldTypeService _fieldTypeService;
        private readonly IRepository<FieldType> _fieldTypeRepository;
        public FieldController(IFieldService fieldService,
            IRepository<Field> fieldRepository,
            IFieldTypeService fieldTypeService,
            IRepository<FieldType> fieldTypeRepository)
        {
            _fieldService = fieldService;
            _fieldRepository = fieldRepository;
            _fieldTypeService = fieldTypeService;
            _fieldTypeRepository = fieldTypeRepository;
        }
        public ActionResult Index()
        {
            var entities = _fieldTypeRepository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis).ToList();
            ViewBag.Trees = GetTree(null, entities);
            return View();
        }
        private List<TreeView> GetTree(string parentId, List<FieldType> entities)
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
                    Text = item.TypeName
                });
            });
            return treeViews;
        }
        public ActionResult GetEntityType(string id)
        {
            var entity = _fieldTypeRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return Json(new FieldTypeView()
            {
                TypeId = entity.Id,
                TypeName = entity.TypeName,
                ParentId = entity.ParentId,
                CallIndex = entity.CallIndex

            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrUpdateType(FieldTypeView viewModel)
        {
            var entity = new FieldType()
            {
                TypeName = viewModel.TypeName,
                CallIndex = viewModel.CallIndex,
                ParentId = viewModel.ParentId
            };
           
            if (!string.IsNullOrWhiteSpace(viewModel.TypeId))
            {
                entity.Id = viewModel.TypeId;
                entity.ModifiedBy = CurrentManager.UserName;
                entity.ModifiedById = CurrentManager.Id;
                entity.ModifiedDate = DateTime.Now;
                _fieldTypeService.Update(entity);
                TempData["Msg"] = "更新成功";
            }
            else
            {
                entity.Id = IdBuilder.CreateIdNum();
                entity.AddedBy = CurrentManager.UserName;
                entity.AddedById = CurrentManager.Id;
                entity.AddedDate = DateTime.Now;
                _fieldTypeService.Add(entity);
                TempData["Msg"] = "添加成功";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteType(string id)
        {
            var entity = _fieldTypeRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _fieldTypeService.Delete(entity);
            TempData["Msg"] = "删除成功";
            return RedirectToAction("Index");
        }


        public ActionResult GetEntityField(string id)
        {
            var entity = _fieldRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return Json(new FieldView()
            {
                Id = entity.Id,
                Text = entity.Text,
                Value = entity.Value,
                Taxis = entity.Taxis,
                FieldTypeId = entity.FieldTypeId

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetList(FieldView viewModel)
        {
            var result = _fieldService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new FieldView
                {
                    Id = d.Id,
                    Text = d.Text,
                    Value = d.Value,
                    Taxis = d.Taxis
                })
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult AddOrUpdateField(FieldView viewModel)
        {

            var msg = string.Empty;
            if (!string.IsNullOrWhiteSpace(viewModel.Id))
            {
                var entity = _fieldRepository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
                entity.Id = viewModel.Id;
                entity.Text = viewModel.Text;
                entity.Value = viewModel.Value;
                entity.Taxis = viewModel.Taxis;
                entity.FieldTypeId = viewModel.FieldTypeId;
                entity.ModifiedBy = CurrentManager.UserName;
                entity.ModifiedById = CurrentManager.Id;
                entity.ModifiedDate = DateTime.Now;
                _fieldService.Update(entity);
                msg = "更新成功";
            }
            else
            {
                var entity = new Field
                {
                    Text = viewModel.Text,
                    Taxis = viewModel.Taxis,
                    Value = viewModel.Value,
                    FieldTypeId = viewModel.FieldTypeId,
                    Id = IdBuilder.CreateIdNum(),
                    AddedBy = CurrentManager.UserName,
                    AddedById = CurrentManager.Id,
                    AddedDate = DateTime.Now
                };
                _fieldService.Add(entity);
                msg = "添加成功";
            }
            return Json(new { State = 1, Msg = msg });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult DeleteField(string id)
        {
            var entity = _fieldRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _fieldService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}