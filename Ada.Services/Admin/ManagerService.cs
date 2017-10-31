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
        /// 新增用户
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="roleIds"></param>
        /// <param name="organizationIds"></param>
        public void Add(Manager entity, List<string> roleIds = null, List<string> organizationIds = null)
        {
            _managerRepository.Add(entity);
            if (roleIds!=null)
            {
                
                foreach (var id in roleIds)
                {
                    var role = _roleRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
                    if (role!=null)
                    {
                        entity.Roles.Add(role);
                    }
                }
            }
            if (organizationIds!=null)
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
            _dbContext.SaveChanges();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="roleIds"></param>
        /// <param name="organizationIds"></param>
        public void Update(Manager entity, List<string> roleIds = null, List<string> organizationIds = null)
        {
            _managerRepository.Update(entity);
            entity.Roles.Clear();
            entity.Organizations.Clear();
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
                    _managerActionRepository.Remove(managerActions);
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
        
    }
}
