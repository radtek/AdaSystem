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
    public class RoleController : BaseController
    {
        private readonly IRepository<Action> _actionRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRoleService _roleService;
        public RoleController(IRepository<Action> actionRepository, IRoleService roleService, IRepository<Role> roleRepository)
        {
            _actionRepository = actionRepository;
            _roleRepository = roleRepository;
            _roleService = roleService;
        }
        // GET: Role
        public ActionResult Index()
        {
            var actions = _actionRepository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis).ToList();
            ViewBag.Actions = GetTree(null, actions);
            var roles = _roleRepository.LoadEntities(d => d.IsDelete == false).ToList();
            ViewBag.Roles = roles.Select(d => new RoleView()
            {
                Id = d.Id,
                RoleName = d.RoleName,
                RoleType = d.RoleType,
                RoleGrade = d.RoleGrade,
                DataRange = d.DataRange
            }).OrderBy(d=>d.RoleGrade);
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
            var role = _roleRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return Json(new RoleView()
            {
                Id = role.Id,
                RoleName = role.RoleName,
                RoleType = role.RoleType,
                RoleGrade = role.RoleGrade,
                DataRange = role.DataRange,
                ActionIds = string.Join(",", role.Actions.Select(d => d.Id))

            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        
        public ActionResult AddOrUpdate(RoleView viewModel)
        {
            if (!string.IsNullOrWhiteSpace(viewModel.Id))
            {
                var role = _roleRepository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
                role.RoleName = viewModel.RoleName;
                role.RoleType = viewModel.RoleType;
                role.DataRange = viewModel.DataRange;
                role.RoleGrade = viewModel.RoleGrade;

                role.ModifiedBy = CurrentManager.UserName;
                role.ModifiedById = CurrentManager.Id;
                role.ModifiedDate = DateTime.Now;
                _roleService.Update(role, viewModel.ActionIds);
                TempData["Msg"] = "更新成功";
            }
            else
            {
                var role = new Role
                {
                    RoleName = viewModel.RoleName,
                    RoleType = viewModel.RoleType,
                    DataRange = viewModel.DataRange,
                    RoleGrade = viewModel.RoleGrade,
                    Id = IdBuilder.CreateIdNum(),
                    AddedBy = CurrentManager.UserName,
                    AddedById = CurrentManager.Id,
                    AddedDate = DateTime.Now
                };
                _roleService.Add(role, viewModel.ActionIds);
                TempData["Msg"] = "添加成功";
            }
            ClearCacheByManagers("Menu");
            return RedirectToAction("Index");
        }
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            var role = _roleRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            role.DeletedBy = CurrentManager.UserName;
            role.DeletedById = CurrentManager.Id;
            role.DeletedDate = DateTime.Now;
            _roleService.Delete(role);
            TempData["Msg"] = "删除成功";
            ClearCacheByManagers("Menu");
            return RedirectToAction("Index");
        }

    }
}