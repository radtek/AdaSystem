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
    public class OrganizationController : BaseController
    {
        private readonly IOrganizationService _organizationService;
        private readonly IRepository<Organization> _repository;
        public OrganizationController(IOrganizationService organizationService,
            IRepository<Organization> repository)
        {
            _organizationService = organizationService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            var entities = _repository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis).ToList();
            ViewBag.Trees = GetTree(null, entities);
            return View();
        }
        private List<TreeView> GetTree(string parentId, List<Organization> entities)
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
                    Text = item.OrganizationName
                });
            });
            return treeViews;
        }
        public ActionResult GetEntity(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return Json(new OrganizationView()
            {
                Id = entity.Id,
                OrganizationName = entity.OrganizationName,
                ParentId = entity.ParentId,
                Taxis = entity.Taxis

            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrUpdate(OrganizationView viewModel)
        {
            var entity = new Organization()
            {
                OrganizationName = viewModel.OrganizationName,
                Taxis = viewModel.Taxis
            };
            if (!string.IsNullOrWhiteSpace(viewModel.ParentId))
            {
                entity.ParentId = viewModel.ParentId;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Id))
            {
                entity.Id = viewModel.Id;
                entity.ModifiedBy = CurrentManager.UserName;
                entity.ModifiedById = CurrentManager.Id;
                entity.ModifiedDate = DateTime.Now;
                _organizationService.Update(entity);
                TempData["Msg"] = "更新成功";
            }
            else
            {
                entity.Id = IdBuilder.CreateIdNum();
                entity.AddedBy = CurrentManager.UserName;
                entity.AddedById = CurrentManager.Id;
                entity.AddedDate = DateTime.Now;
                _organizationService.Add(entity);
                TempData["Msg"] = "添加成功";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _organizationService.Delete(entity);
            TempData["Msg"] = "删除成功";
            return RedirectToAction("Index");
        }
    }
}