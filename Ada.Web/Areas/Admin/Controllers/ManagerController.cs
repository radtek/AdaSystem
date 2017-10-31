using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Admin;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Action = Ada.Core.Domain.Admin.Action;

namespace Admin.Controllers
{
    public class ManagerController : BaseController
    {
        private readonly IRepository<Action> _actionRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<Manager> _managerRepository;
        private readonly IManagerService _managerService;
        private readonly IRepository<Organization> _organizationRepository;

        public ManagerController(IManagerService managerService,
            IRepository<Manager> managerRepository,
            IRepository<Role> roleRepository,
            IRepository<Action> actionRepository,
            IRepository<Organization> organizationRepository)
        {
            _actionRepository = actionRepository;
            _roleRepository = roleRepository;
            _managerRepository = managerRepository;
            _managerService = managerService;
            _organizationRepository = organizationRepository;
        }
        public ActionResult Index()
        {
            var managers = _managerRepository.LoadEntities(d => d.IsDelete == false).ToList();
            if (managers.Count > 0)
            {
                ViewBag.Managers = managers.Select(d => new ManagerView()
                {
                    Id = d.Id,
                    UserName = d.UserName,
                    Status = d.Status,
                    RealName = d.RealName,
                    AddDate = d.AddedDate?.ToString("yyyy年MM月dd日") ?? "",
                    Roles = d.Roles.Count > 0 ? string.Join(",", d.Roles.Select(r => r.RoleName)) : ""
                });
            }
            var roles = _roleRepository.LoadEntities(d => d.IsDelete == false).ToList();
            ViewBag.Roles = roles.Select(d => new RoleView() { Id = d.Id, RoleName = d.RoleName });
            var organizations = _organizationRepository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis)
                .ToList();
            ViewBag.Organizations = GetOrganizationTree(null, organizations);
            return View();
        }
        private List<TreeView> GetOrganizationTree(string parentId, List<Organization> list)
        {
            var newlist = list.Where(d => d.ParentId == parentId).ToList();
            List<TreeView> treeViews = new List<TreeView>();
            newlist.ForEach(item =>
            {
                treeViews.Add(new TreeView
                {
                    Id = item.Id,
                    Children = GetOrganizationTree(item.Id, list),
                    ParentId = item.ParentId,
                    Text = item.OrganizationName
                });
            });
            return treeViews;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrUpdate(ManagerView viewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = "请核对输入的用户信息是否正确";
                return RedirectToAction("Index");
            }
            var organizationIds = string.IsNullOrWhiteSpace(viewModel.OrganizationIds)
                ? null
                : viewModel.OrganizationIds.Split(',').ToList();
            if (!string.IsNullOrWhiteSpace(viewModel.Id))
            {
                //校验唯一性
                var temp = _managerRepository
                    .LoadEntities(d => d.UserName.Equals(viewModel.UserName, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false && d.Id != viewModel.Id)
                    .FirstOrDefault();
                if (temp != null)
                {
                    TempData["Warning"] = "用户名：" + viewModel.UserName + "，已被占用！";
                    return RedirectToAction("Index");
                }
                var manager = _managerRepository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
                manager.UserName = viewModel.UserName;
                manager.RealName = viewModel.RealName;
                manager.Phone = viewModel.Phone;
                manager.Status = viewModel.Status;
                manager.Password = Encrypt.Encode(viewModel.Password);
                manager.ModifiedBy = CurrentManager.Id;
                manager.ModifiedDate = DateTime.Now;

                _managerService.Update(manager, viewModel.RoleIds, organizationIds);
                TempData["Msg"] = "更新成功";
            }
            else
            {
                //校验唯一性
                var temp = _managerRepository
                      .LoadEntities(d => d.UserName.Equals(viewModel.UserName, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false)
                      .FirstOrDefault();
                if (temp != null)
                {
                    TempData["Warning"] = "用户名：" + viewModel.UserName + "，已被占用！";
                    return RedirectToAction("Index");
                }
                var manager = new Manager()
                {
                    UserName = viewModel.UserName,
                    RealName = viewModel.RealName,
                    Phone = viewModel.Phone,
                    Status = viewModel.Status,
                    Password = Encrypt.Encode(viewModel.Password),
                    Id = IdBuilder.CreateIdNum(),
                    AddedBy = CurrentManager.Id,
                    AddedDate = DateTime.Now
                };
                _managerService.Add(manager, viewModel.RoleIds, organizationIds);
                TempData["Msg"] = "添加成功";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var manager = _managerRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            manager.DeletedBy = CurrentManager.Id;
            manager.DeletedDate = DateTime.Now;
            _managerService.Delete(manager);
            TempData["Msg"] = "删除成功";
            return RedirectToAction("Index");
        }
    }
}