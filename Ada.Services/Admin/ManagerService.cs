using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Log;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
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
                allList = viewModel.Status==0 ? allList.Where(d => d.Status == viewModel.Status||d.Status==null) : allList.Where(d => d.Status == viewModel.Status);
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
        /// <summary>
        /// 根据微信ID获取用户信息
        /// </summary>
        /// <param name="openId"></param>
        public Manager GetMangerByOpenId(string openId)
        {
            return _managerRepository.LoadEntities(d => d.OpenId == openId || d.UnionId == openId && d.IsDelete == false && d.Status == Consts.StateNormal).FirstOrDefault();

        }
        /// <summary>
        /// 根据微信ID获取用户信息
        /// </summary>
        /// <param name="phone"></param>
        public Manager GetMangerByPhone(string phone)
        {
            return _managerRepository.LoadEntities(d => d.Phone == phone && d.IsDelete == false && d.Status == Consts.StateNormal).FirstOrDefault();

        }
        public ManagerView Login(LoginModel loginModel)
        {
            Manager manager;
            if (!string.IsNullOrWhiteSpace(loginModel.OpenId))
            {
                manager =
                    _managerRepository.LoadEntities(u => (u.OpenId == loginModel.OpenId || u.UnionId == loginModel.OpenId) && u.Status == Consts.StateNormal && u.IsDelete == false).FirstOrDefault();
            }
            else if (!string.IsNullOrWhiteSpace(loginModel.Phone))
            {
                manager =
                    _managerRepository.LoadEntities(u => u.Phone == loginModel.Phone && u.Status == Consts.StateNormal && u.IsDelete == false).FirstOrDefault();
            }
            else
            {
                loginModel.Password = Encrypt.Encode(loginModel.Password);
                manager =
                    _managerRepository.LoadEntities(u => u.UserName == loginModel.LoginName && u.Password == loginModel.Password && u.Status == Consts.StateNormal && u.IsDelete == false).FirstOrDefault();
            }
            if (manager == null)
            {
                loginModel.Message = !string.IsNullOrWhiteSpace(loginModel.OpenId) ? "用户未绑定微信" : "用户不存在或者密码有误";
                return null;
            }
            if (manager.Roles.Count == 0)
            {
                loginModel.Message = "未分配角色，请联系管理员";
                return null;
            }
            //根据角色级别排序，获取最高的那个
            var role = manager.Roles.OrderBy(d => d.RoleGrade).FirstOrDefault();
            //记录日志
            manager.ManagerLoginLogs.Add(new ManagerLoginLog()
            {
                Id = IdBuilder.CreateIdNum(),
                IpAddress = Utils.GetIpAddress(),
                LoginTime = loginModel.LoginLog.LoginTime,
                WebInfo = loginModel.LoginLog.UserAgent,
                Remark = "成功"
            });
            _dbContext.SaveChanges();
            loginModel.IsSuccess = true;
            return new ManagerView()
            {
                Id = manager.Id,
                Phone = manager.Phone,
                RealName = manager.RealName,
                Image = manager.Image,
                UserName = manager.UserName,
                RoleId = role.Id,
                RoleName = role.RoleName,
                RoleList = manager.Roles.Select(d => new RoleView() { Id = d.Id, RoleName = d.RoleName }),
                Roles = manager.Roles.Count > 0 ? string.Join(",", manager.Roles.Select(d => d.RoleName)) : "",
                OrganizationIds = string.Join("/", manager.Organizations.Select(d => d.Id)),
                Organizations = string.Join("-", manager.Organizations.Where(d => d.ParentId != null).Select(d => d.OrganizationName))
            };
        }
        public bool BindingOpenId(string loginName, string pwd, string openid, out string errmsg, string image = null)
        {
            errmsg = "绑定成功！";
            pwd = Encrypt.Encode(pwd);
            var manager =
                _managerRepository.LoadEntities(u => u.UserName == loginName && u.Password == pwd && u.Status == Consts.StateNormal && u.IsDelete == false).FirstOrDefault();
            if (manager == null)
            {
                errmsg = "用户不存在或者密码有误！";
                return false;
            }
            if (manager.Roles.Count == 0)
            {
                errmsg = "此用户未分配角色，请联系管理员";
                return false;
            }
            manager.OpenId = openid.Trim();
            if (string.IsNullOrWhiteSpace(manager.Image))
            {
                if (!string.IsNullOrWhiteSpace(image))
                {
                    manager.Image = image;
                }
            }
            _dbContext.SaveChanges();
            return true;

        }
        public ManagerView BindingLogin(LoginModel loginModel)
        {
            loginModel.Password = Encrypt.Encode(loginModel.Password);
            var manager =
                _managerRepository.LoadEntities(u => u.UserName == loginModel.LoginName && u.Password == loginModel.Password && u.Status == Consts.StateNormal && u.IsDelete == false).FirstOrDefault();

            if (manager == null)
            {
                loginModel.Message = "用户不存在或者密码有误";
                return null;
            }
            if (manager.Roles.Count == 0)
            {
                loginModel.Message = "未分配角色，请联系管理员";
                return null;
            }
            if (!string.IsNullOrWhiteSpace(manager.UnionId))
            {
                loginModel.Message = "此账户已绑定了微信，无需重复绑定，如需解绑，请联系管理员！";
                return null;
            }
            //根据角色级别排序，获取最高的那个
            var role = manager.Roles.OrderBy(d => d.RoleGrade).FirstOrDefault();
            //记录日志
            manager.ManagerLoginLogs.Add(new ManagerLoginLog()
            {
                Id = IdBuilder.CreateIdNum(),
                IpAddress = Utils.GetIpAddress(),
                LoginTime = loginModel.LoginLog.LoginTime,
                WebInfo = loginModel.LoginLog.UserAgent,
                Remark = "成功"
            });
            manager.UnionId = loginModel.OpenId;
            _dbContext.SaveChanges();
            loginModel.IsSuccess = true;
            return new ManagerView()
            {
                Id = manager.Id,
                Phone = manager.Phone,
                RealName = manager.RealName,
                Image = manager.Image,
                UserName = manager.UserName,
                RoleId = role.Id,
                RoleName = role.RoleName,
                RoleList = manager.Roles.Select(d => new RoleView() { Id = d.Id, RoleName = d.RoleName }),
                Roles = manager.Roles.Count > 0 ? string.Join(",", manager.Roles.Select(d => d.RoleName)) : "",
                OrganizationIds = string.Join("/", manager.Organizations.Select(d => d.Id)),
                Organizations = string.Join("-", manager.Organizations.Where(d => d.ParentId != null).Select(d => d.OrganizationName))
            };
        }

        public void UnBinding(string id)
        {
            var manager = _managerRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            manager.UnionId = null;
            manager.OpenId = null;
            manager.Status = null;
            
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
                               UserName = m.UserName,
                               OpenId = m.OpenId
                           };
            return managers;
        }
    }
}
