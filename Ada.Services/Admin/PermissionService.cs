using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;

namespace Ada.Services.Admin
{
    public class PermissionService : IPermissionService
    {
        private readonly IRepository<Core.Domain.Admin.Action> _actionRepository;
        private readonly IRepository<ManagerAction> _managerActionRepository;
        private readonly IRepository<Manager> _managerRepository;
        public PermissionService(IRepository<Core.Domain.Admin.Action> actionRepository,
            IRepository<ManagerAction> managerActionRepository,
            IRepository<Manager> managerRepository)
        {
            _actionRepository = actionRepository;
            _managerActionRepository = managerActionRepository;
            _managerRepository = managerRepository;
        }
        public virtual bool Authorize(Core.Domain.Admin.Action action, string managerId)
        {
            //请求权限
            var currentUrlAction = _actionRepository.LoadEntities(a => a.Area.Equals(action.Area, StringComparison.CurrentCultureIgnoreCase) &&
                                                                       a.ControllerName.Equals(action.ControllerName, StringComparison.CurrentCultureIgnoreCase) &&
                                                                       a.MethodName.Equals(action.MethodName, StringComparison.CurrentCultureIgnoreCase) &&
                                                                       a.HttpMethod.Equals(action.HttpMethod, StringComparison.CurrentCultureIgnoreCase) &&
                                                                       a.IsDelete == false).FirstOrDefault();
            if (currentUrlAction==null)
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
                if (!tempUserAction.IsPass)
                {
                    return false;
                }
            }
            //TODO 3.校验（用户/部门）角色权限  这里只做了用户
            var manager = _managerRepository.LoadEntities(d => d.Id == managerId).FirstOrDefault();
            //获取当前用户所有角色对应的权限
            var tempRoleActions = from r in manager.Roles
                from a in r.Actions
                where a.Id == currentUrlAction.Id
                select a;
            if (!tempRoleActions.Any())
            {
                return false;
            }
            return true;
        }
    }
}
