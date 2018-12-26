using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Admin;
using Ada.Framework.Caching;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Action = Ada.Core.Domain.Admin.Action;

namespace Admin.Controllers
{
    public class ActionController : BaseController
    {
        private readonly IActionService _actionService;
        private readonly IRepository<Action> _repository;
        public ActionController(IActionService actionService,
            IRepository<Action> repository,ICacheManager cacheManager)
        {
            _actionService = actionService;
            _repository = repository;
        }
        // GET: Action
        public ActionResult Index()
        {
            var actions = _repository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis).ToList();
            ViewBag.Actions = GetTree(null, actions);
            return View();
        }

        private List<TreeView> GetTree(string parentId, List<Action> actions)
        {
            var newlist = actions.Where(d => d.ParentId == parentId).ToList();
            List<TreeView> treeViews = new List<TreeView>();
            newlist.ForEach(item =>
            {
                treeViews.Add(new TreeView
                {
                    Id = item.Id,
                    Children = GetTree(item.Id, actions),
                    ParentId = item.ParentId,
                    Text = item.ActionName
                });
            });
            return treeViews;
        }
        public ActionResult GetEntity(string id)
        {
            var action = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return Json(new ActionView()
            {
                Id = action.Id,
                ActionName = action.ActionName,
                Area = action.Area,
                ControllerName = action.ControllerName,
                HttpMethod = action.HttpMethod,
                IconCls = action.IconCls,
                LinkUrl = action.LinkUrl,
                MethodName = action.MethodName,
                ParentId = action.ParentId,
                Taxis = action.Taxis,
                IsMenu = action.IsMenu,
                IsButton = action.IsButton

            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        
        public ActionResult AddOrUpdate(ActionView actionView)
        {

            if (!string.IsNullOrWhiteSpace(actionView.Id))
            {
                var action = _repository.LoadEntities(d => d.Id == actionView.Id).FirstOrDefault();
                action.ActionName = actionView.ActionName;
                action.Area = actionView.Area;
                action.ControllerName = actionView.ControllerName;
                action.HttpMethod = actionView.HttpMethod;
                action.IconCls = actionView.IconCls;
                action.MethodName = actionView.MethodName;
                action.LinkUrl = actionView.LinkUrl;
                action.Taxis = actionView.Taxis;
                action.IsButton = actionView.IsButton;
                action.IsMenu = actionView.IsMenu;
                action.ParentId = actionView.ParentId;
                action.ModifiedBy = CurrentManager.UserName;
                action.ModifiedById = CurrentManager.Id;
                action.ModifiedDate = DateTime.Now;
                _actionService.Update(action);
                TempData["Msg"] = "更新成功";
            }
            else
            {
                var action = new Action
                {
                    ActionName = actionView.ActionName,
                    Area = actionView.Area,
                    ControllerName = actionView.ControllerName,
                    HttpMethod = actionView.HttpMethod,
                    IconCls = actionView.IconCls,
                    MethodName = actionView.MethodName,
                    LinkUrl = actionView.LinkUrl,
                    Taxis = actionView.Taxis,
                    IsButton = actionView.IsButton,
                    IsMenu = actionView.IsMenu,
                    ParentId = actionView.ParentId,
                    Id = IdBuilder.CreateIdNum(),
                    AddedBy = CurrentManager.UserName,
                    AddedById = CurrentManager.Id,
                    AddedDate = DateTime.Now
                };
                bool isCurd = false;
                if (string.IsNullOrWhiteSpace(action.ParentId))
                {
                    if (actionView.IsCURD==true)
                    {
                        isCurd = true;
                    }
                }
                _actionService.Add(action, isCurd);
                TempData["Msg"] = "添加成功";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            var action = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            action.DeletedBy = CurrentManager.UserName;
            action.DeletedById = CurrentManager.Id;
            action.DeletedDate = DateTime.Now;
            _actionService.Delete(action);
            TempData["Msg"] = "删除成功";
            return RedirectToAction("Index");
        }

    }
}