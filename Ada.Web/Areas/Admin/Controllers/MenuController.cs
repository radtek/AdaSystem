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
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;
        private readonly IRepository<Menu> _repository;
        private readonly IRepository<Ada.Core.Domain.Admin.Action> _actionRepository;
        public MenuController(IMenuService menuService,
            IRepository<Menu> repository,
            IRepository<Ada.Core.Domain.Admin.Action> actionRepository)
        {
            _menuService = menuService;
            _repository = repository;
            _actionRepository = actionRepository;
        }
        public ActionResult Index()
        {
            var entities = _repository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis).ToList();
            ViewBag.Trees = GetTree(null, entities);
            //获取是菜单的Action
            var actions = _actionRepository.LoadEntities(d => d.IsDelete == false && d.IsMenu == true).OrderBy(d => d.Taxis).ToList();
            ViewBag.Actions = actions.Select(d => new SelectListItem()
            {
                Text = d.ActionName,
                Value = d.Id
            }).ToList();
            return View();
        }
        private List<TreeView> GetTree(string parentId, List<Menu> entities)
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
                    Text = item.MenuName
                });
            });
            return treeViews;
        }
        public ActionResult GetEntity(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return Json(new MenuView()
            {
                Id = entity.Id,
                Name = entity.MenuName,
                ParentId = entity.ParentId,
                Taxis = entity.Taxis,
                ActionId = entity.ActionId,
                IsVisable = entity.IsVisable,
                IconCls = entity.IconCls,
                IsBlank = entity.IsBlank

            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        
        public ActionResult AddOrUpdate(MenuView viewModel)
        {

            if (!string.IsNullOrWhiteSpace(viewModel.Id))
            {
                var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
                entity.MenuName = viewModel.Name;
                entity.Taxis = viewModel.Taxis;
                entity.ActionId = viewModel.ActionId;
                entity.IsVisable = viewModel.IsVisable;
                entity.IsBlank = viewModel.IsBlank;
                entity.IconCls = viewModel.IconCls;
                entity.ParentId = viewModel.ParentId;
                entity.ModifiedById = CurrentManager.Id;
                entity.ModifiedBy = CurrentManager.UserName;
                entity.ModifiedDate = DateTime.Now;
                _menuService.Update(entity);
                TempData["Msg"] = "更新成功";
            }
            else
            {
                var entity = new Menu()
                {
                    MenuName = viewModel.Name,
                    Taxis = viewModel.Taxis,
                    ActionId = viewModel.ActionId,
                    IsVisable = viewModel.IsVisable,
                    ParentId = viewModel.ParentId,
                    Id = IdBuilder.CreateIdNum(),
                    AddedBy = CurrentManager.UserName,
                    AddedById = CurrentManager.Id,
                    AddedDate = DateTime.Now,
                    IconCls = viewModel.IconCls,
                    IsBlank = viewModel.IsBlank

                };
                _menuService.Add(entity);
                TempData["Msg"] = "添加成功";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.TreePath.Contains(id)).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _menuService.Delete(entity);
            TempData["Msg"] = "删除成功";
            return RedirectToAction("Index");
        }
    }
}