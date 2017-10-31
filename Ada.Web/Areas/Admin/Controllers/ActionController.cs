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
using Action = Ada.Core.Domain.Admin.Action;

namespace Admin.Controllers
{
    public class ActionController : BaseController
    {
        private readonly IActionService _actionService;
        private readonly IRepository<Action> _repository;
        public ActionController(IActionService actionService,
            IRepository<Action> repository)
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
                Taxis = action.Taxis

            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrUpdate(ActionView actionView)
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
                Taxis = actionView.Taxis
            };
            if (!string.IsNullOrWhiteSpace(actionView.ParentId))
            {
                action.ParentId = actionView.ParentId;
            }
            if (!string.IsNullOrWhiteSpace(actionView.Id))
            {
                action.Id = actionView.Id;
                action.ModifiedBy = CurrentManager.Id;
                action.ModifiedDate = DateTime.Now;
                _actionService.Update(action);
                TempData["Msg"] = "更新成功";
            }
            else
            {
                action.Id = IdBuilder.CreateIdNum();
                action.AddedBy = CurrentManager.Id;
                action.AddedDate = DateTime.Now;
                _actionService.Add(action);
                TempData["Msg"] = "添加成功";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var action = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            action.DeletedBy = CurrentManager.Id;
            action.DeletedDate = DateTime.Now;
            _actionService.Delete(action);
            TempData["Msg"] = "删除成功";
            return RedirectToAction("Index");
        }

    }
}