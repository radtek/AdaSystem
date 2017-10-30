using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Action = Ada.Core.Domain.Admin.Action;

namespace Ada.Services.Admin
{
    public class RoleService : IRoleService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Action> _actionRepository;
        private readonly IRepository<Role> _roleRepository;
        public RoleService(IDbContext dbContext,
            IRepository<Action> actionRepository,
            IRepository<Role> roleRepository)
        {
            _dbContext = dbContext;
            _actionRepository = actionRepository;
            _roleRepository = roleRepository;
        }
        public void Add(Role role, string actionIds)
        {
            _roleRepository.Add(role);
            if (!string.IsNullOrWhiteSpace(actionIds))
            {
                var arry = actionIds.Split(',');
                foreach (var id in arry)
                {
                    var action = _actionRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
                    if (action!=null)
                    {
                        role.Actions.Add(action);
                    }
                }
            }
            _dbContext.SaveChanges();
        }

        public void Delete(Role role)
        {
            _roleRepository.Delete(role);
            _dbContext.SaveChanges();
        }

        public void Update(Role role, string actionIds)
        {
            _roleRepository.Update(role);
            if (!string.IsNullOrWhiteSpace(actionIds))
            {
                role.Actions.Clear();
                var arry = actionIds.Split(',');
                foreach (var id in arry)
                {
                    var action = _actionRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
                    if (action != null)
                    {
                        role.Actions.Add(action);
                    }
                }
            }
            _dbContext.SaveChanges();
        }
    }
}
