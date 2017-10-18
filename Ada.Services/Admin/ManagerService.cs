using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.ViewModel.Admin;

namespace Ada.Services.Admin
{
    public class ManagerService : IManagerService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Manager> _managerRepository;
        private readonly IRepository<ManagerAction> _managerActionRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<Role> _roleRepository;
        public ManagerService(IDbContext dbContext,
            IRepository<Manager> managerRepository,
            IRepository<ManagerAction> managerActionRepository,
            IRepository<Organization> organizationRepository,
            IRepository<Role> roleRepository)
        {
            _dbContext = dbContext;
            _managerRepository = managerRepository;
            _managerActionRepository = managerActionRepository;
            _organizationRepository = organizationRepository;
            _roleRepository = roleRepository;
        }
        public IQueryable<Manager> LoadEntitiesFilter(ManagerView viewModel)
        {
            var allList = _managerRepository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.UserName))
            {
                allList = allList.Where(d => d.UserName.Contains(viewModel.UserName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Phone))
            {
                allList = allList.Where(d => d.Phone.Contains(viewModel.Phone));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.OpenId))
            {
                allList = allList.Where(d => d.OpenId.Contains(viewModel.OpenId));
            }
            if (viewModel.Status != null)
            {
                allList = allList.Where(d => d.Status == viewModel.Status);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.RoleId))
            {
                allList = allList.Where(d => d.Roles.Select(r => r.Id).Contains(viewModel.RoleId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.OrganizationId))
            {
                allList = allList.Where(d => d.Organizations.Select(r => r.Id).Contains(viewModel.OrganizationId));
            }
            viewModel.total = allList.Count();
            int page = viewModel.page ?? 1;
            int rows = viewModel.rows ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return allList.OrderByDescending(d => d.Id).Skip((page - 1) * rows).Take(rows);
            }
            return allList.OrderBy(d => d.Id).Skip((page - 1) * rows).Take(rows);
        }
        /// <summary>
        /// 设置权限
        /// </summary>
        /// <param name="managerIds"></param>
        /// <param name="actionIds"></param>
        /// <returns></returns>
        public bool SetAction(List<string> managerIds, List<string> actionIds)
        {
            foreach (var managerId in managerIds)
            {
                //清除
                var managerActions = _managerActionRepository.LoadEntities(a => a.ManagerId == managerId);
                if (managerActions.Any())
                {
                    _managerActionRepository.Delete(managerActions);
                }
                //新增
                foreach (var ids in actionIds)
                {
                    if (!string.IsNullOrWhiteSpace(ids))
                    {
                        var arr = ids.Split('^');
                        var actionId = arr[0];
                        var isPass = bool.Parse(arr[1]);
                        ManagerAction managerAction = new ManagerAction
                        {
                            Id = IdBuilder.CreateIdNum(),
                            ActionInfoId = actionId,
                            ManagerId = managerId,
                            IsPass = isPass
                        };
                        _managerActionRepository.Add(managerAction);
                    }
                }
            }
            return _dbContext.SaveChanges() >= 0;
        }
        /// <summary>
        /// 设计机构
        /// </summary>
        /// <param name="managerIds"></param>
        /// <param name="organizationIdIds"></param>
        /// <returns></returns>
        public bool SetOrganization(List<string> managerIds, List<string> organizationIdIds)
        {
            foreach (var managerId in managerIds)
            {
                var manager = _managerRepository.LoadEntities(d => d.Id == managerId).FirstOrDefault();
                if (manager != null)
                {
                    manager.Organizations.Clear();
                    foreach (var organizationIdId in organizationIdIds)
                    {
                        var dep = _organizationRepository.LoadEntities(d => d.Id == organizationIdId).FirstOrDefault();
                        if (dep != null)
                        {
                            manager.Organizations.Add(dep);
                        }
                    }
                }
            }
            return _dbContext.SaveChanges() >= 0;
        }
        /// <summary>
        /// 设置角色
        /// </summary>
        /// <param name="managerIds"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public bool SetRole(List<string> managerIds, List<string> roleIds)
        {
            foreach (var managerId in managerIds)
            {
                var manager = _managerRepository.LoadEntities(d => d.Id == managerId).FirstOrDefault();
                if (manager != null)
                {
                    manager.Roles.Clear();
                    foreach (var roleId in roleIds)
                    {
                        var role = _roleRepository.LoadEntities(d => d.Id == roleId).FirstOrDefault();
                        if (role != null)
                        {
                            manager.Roles.Add(role);
                        }
                    }
                }
            }
            return _dbContext.SaveChanges() >= 0;
        }
    }
}
