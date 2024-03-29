﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.ViewModel.Admin;

namespace Ada.Services.Admin
{
    public class PermissionService : IPermissionService
    {
        private readonly IRepository<Core.Domain.Admin.Action> _actionRepository;
        private readonly IRepository<ManagerAction> _managerActionRepository;
        private readonly IRepository<Manager> _managerRepository;
        private readonly IRepository<Menu> _menuRepository;
        public PermissionService(IRepository<Core.Domain.Admin.Action> actionRepository,
            IRepository<ManagerAction> managerActionRepository,
            IRepository<Manager> managerRepository,
            IRepository<Menu> menuRepository)
        {
            _actionRepository = actionRepository;
            _managerActionRepository = managerActionRepository;
            _managerRepository = managerRepository;
            _menuRepository = menuRepository;
        }
        /// <summary>
        /// 请求权限
        /// </summary>
        /// <returns></returns>
        public virtual bool Authorize(Core.Domain.Admin.Action action, string managerId, string roleId = "")
        {
            //请求权限
            var currentUrlAction = _actionRepository.LoadEntities(a => a.Area.Equals(action.Area, StringComparison.CurrentCultureIgnoreCase) &&
                                                                       a.ControllerName.Equals(action.ControllerName, StringComparison.CurrentCultureIgnoreCase) &&
                                                                       a.MethodName.Equals(action.MethodName, StringComparison.CurrentCultureIgnoreCase) &&
                                                                       a.HttpMethod.Equals(action.HttpMethod, StringComparison.CurrentCultureIgnoreCase) &&
                                                                       a.IsDelete == false).FirstOrDefault();
            if (currentUrlAction == null)
            {
                return false;
            }
            //2.校验用户特殊权限
            var managerActions = from a in _managerActionRepository.LoadEntities(u => u.IsDelete == false)
                                 where a.ActionInfoId == currentUrlAction.Id && a.ManagerId == managerId
                                 select a;
            var tempUserAction = managerActions.FirstOrDefault();
            if (tempUserAction != null)
            {
                return tempUserAction.IsPass;
            }
            //TODO 3.校验（用户/部门）角色权限  这里只做了用户
            var manager = _managerRepository.LoadEntities(d => d.Id == managerId).FirstOrDefault();
            //获取当前用户所有角色对应的权限
            IEnumerable<Core.Domain.Admin.Action> tempRoleActions;
            if (string.IsNullOrWhiteSpace(roleId))
            {
                tempRoleActions = from r in manager.Roles
                                  from a in r.Actions
                                  where a.Id == currentUrlAction.Id
                                  select a;
            }
            else
            {
                var role = manager.Roles.FirstOrDefault(d => d.Id == roleId);
                tempRoleActions = from a in role.Actions
                                  where a.Id == currentUrlAction.Id
                                  select a;
            }

            return tempRoleActions.Any();
        }
        /// <summary>
        /// 菜单权限（当前用户）
        /// </summary>
        /// <returns></returns>
        public List<MenuView> AuthorizeMenu(string managerId, string roleId = "")
        {
            var allMenus = _menuRepository.LoadEntities(d => d.IsDelete == false && d.IsVisable != true).ToList();
            var manager = _managerRepository.LoadEntities(d => d.Id == managerId).FirstOrDefault();
            //拿到了角色对应的权限的id
            List<string> allRoleActionIds;
            if (string.IsNullOrWhiteSpace(roleId))
            {
                allRoleActionIds = (from r in manager.Roles
                                    from a in r.Actions
                                    where a.IsDelete == false && r.IsDelete == false
                                    select a.Id).ToList();
            }
            else
            {
                var role = manager.Roles.FirstOrDefault(d => d.Id == roleId);
                allRoleActionIds = (from a in role.Actions
                                    where a.IsDelete == false && role.IsDelete == false
                                    select a.Id).ToList();
            }

            //拿到了当前用户所有的允许的特殊权限
            var allUserActionIsPass = (from r in manager.ManagerActions
                                       where r.IsPass
                                       select r.ActionInfoId).ToList();
            //合并起来的所有的都被允许的。
            allRoleActionIds.AddRange(allUserActionIsPass);
            //去掉不允许的
            var allNotPassUserActions = (from r in manager.ManagerActions
                                         where r.IsPass == false
                                         select r.ActionInfoId).ToList();
            var result = (from a in allRoleActionIds
                          where !allNotPassUserActions.Contains(a)
                          select a).ToList();
            //拿到当前用户所有的权限Id
            result = result.Distinct().ToList();
            //获取权限对应的菜单数据
            var allMenuData = (from m in allMenus
                               where result.Contains(m.ActionId)
                               select m).ToList();
            List<Menu> list = new List<Menu>();
            foreach (var menuInfo in allMenuData)
            {
                var meunIds = menuInfo.TreePath.TrimStart('/').TrimEnd('/').Split('/');
                foreach (var meunId in meunIds)
                {
                    var temp = allMenus.FirstOrDefault(d => d.Id == meunId);
                    if (temp != null)
                    {
                        list.Add(temp);
                    }
                }
            }
            allMenuData.AddRange(list);
            allMenuData = allMenuData.Distinct().ToList();
            return allMenuData.Select(d => new MenuView()
            {
                Id = d.Id,
                Name = d.MenuName,
                ParentId = d.ParentId,
                Level = d.Level,
                IsLeaf = d.IsLeaf,
                IconCls = d.IconCls,
                TreePath = d.TreePath,
                Url = GetUrl(d.ActionId),
                Taxis = d.Taxis,
                IsBlank = d.IsBlank
            }).OrderBy(d => d.Taxis).ToList();
        }

        public List<string> AuthorizeData(string managerId, string roleId)
        {
            var manager = _managerRepository.LoadEntities(d => d.Id == managerId).FirstOrDefault();
            var allManager = _managerRepository.LoadEntities(d => d.IsDelete == false);
            //获取当前角色的数据范围  9:全部 0：个人 1：部门
            var role = manager.Roles.FirstOrDefault(d => d.Id == roleId);
            List<string> managerIds = new List<string>();
            if (role.DataRange == null)
            {
                managerIds.Add(managerId);
            }
            if (role.DataRange == 0)
            {
                managerIds.Add(managerId);
            }
            if (role.DataRange == 1)
            {
                foreach (var managerOrganization in manager.Organizations.Where(o => o.ParentId != null))
                {
                    var managerOrgId = managerOrganization.Id;
                    var managers = (from m in allManager
                                    from o in m.Organizations
                                    where o.TreePath.Contains(managerOrgId)
                                    select m).Select(d => d.Id).ToList();
                    managerIds.AddRange(managers);
                }
            }
            return managerIds;
        }


        private Url GetUrl(string actionId)
        {
            if (string.IsNullOrWhiteSpace(actionId))
            {
                return null;
            }
            var action = _actionRepository.LoadEntities(a => a.Id == actionId).FirstOrDefault();
            if (action == null) return null;
            return new Url()
            {
                Action = action.MethodName,
                Colltroller = action.ControllerName,
                Area = action.Area,
                LinkUrl = action.LinkUrl
            };
        }
    }
}
