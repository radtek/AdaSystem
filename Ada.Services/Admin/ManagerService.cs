using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain;
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
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.Id));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.UserName.Contains(viewModel.search));
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

            viewModel.total = allList.Count();
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return allList.OrderByDescending(d => d.Id).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.Id).Skip(offset).Take(rows);
        }

        /// <summary>
        /// 新增或更新用户
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isAdd"></param>
        /// <param name="roleIds"></param>
        /// <param name="organizationIds"></param>
        /// <param name="actionIds"></param>
        public void AddOrUpdate(Manager entity, bool isAdd = true, List<string> roleIds = null, List<string> organizationIds = null, List<string> actionIds = null)
        {
            if (isAdd)
            {
                _managerRepository.Add(entity);
            }
            else
            {
                _managerRepository.Update(entity);
                //清除
                entity.Roles.Clear();
                entity.Organizations.Clear();
                var managerActions = _managerActionRepository.LoadEntities(a => a.ManagerId == entity.Id);
                if (managerActions.Any())
                {
                    _managerActionRepository.Remove(managerActions);
                }
            }

            if (roleIds != null)
            {

                foreach (var id in roleIds)
                {
                    var role = _roleRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
                    if (role != null)
                    {
                        entity.Roles.Add(role);
                    }
                }
            }
            if (organizationIds != null)
            {

                foreach (var id in organizationIds)
                {
                    var organization = _organizationRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
                    if (organization != null)
                    {
                        entity.Organizations.Add(organization);
                    }
                }
            }
            if (actionIds != null)
            {
                foreach (var ids in actionIds)
                {
                    if (!string.IsNullOrWhiteSpace(ids))
                    {
                        var arr = ids.Split('^');
                        var actionId = arr[0];
                        var isPass = bool.Parse(arr[1]);
                        ManagerAction managerAction = new ManagerAction()
                        {
                            Id = IdBuilder.CreateIdNum(),
                            ActionInfoId = actionId,
                            ManagerId = entity.Id,
                            IsPass = isPass
                        };
                        _managerActionRepository.Add(managerAction);
                    }
                }
            }
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Manager entity)
        {
            _managerRepository.Delete(entity);
            _dbContext.SaveChanges();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        public void Edit(Manager entity)
        {
            _managerRepository.Update(entity);
            _dbContext.SaveChanges();
        }

        public IEnumerable<ManagerView> GetByOrganizationName(string name)
        {
            var organization = _organizationRepository.LoadEntities(d => d.IsDelete == false && d.OrganizationName == name).FirstOrDefault();
            var allManagers = _managerRepository.LoadEntities(d => d.Status == Consts.StateNormal && d.IsDelete == false);
            var managers = from m in allManagers
                from o in m.Organizations
                where o.TreePath.Contains(organization.Id)
                select new ManagerView()
                {
                    Id = m.Id,
                    UserName = m.UserName
                };
            return managers;
        }
    }
}
