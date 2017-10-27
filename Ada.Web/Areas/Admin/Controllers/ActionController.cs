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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ActionView actionView)
        {
            var action = new Action
            {
                Id = IdBuilder.CreateIdNum(),
                AddedBy = CurrentManager.Id,
                AddedDate = DateTime.Now,
                ActionName = actionView.ActionName,
                Area = actionView.Area,
                ControllerName = actionView.ControllerName,
                HttpMethod = actionView.HttpMethod,
                IconCls = actionView.IconCls,
                MethodName = actionView.MethodName,
                LinkUrl = actionView.LinkUrl
            };
            if (!string.IsNullOrWhiteSpace(actionView.ParentId))
            {
                action.ParentId = actionView.ParentId;
            }
            _actionService.Add(action);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            _actionService.Delete(id);
            TempData["Msg"] = "删除成功";
            return RedirectToAction("Index");
        }

    }
}