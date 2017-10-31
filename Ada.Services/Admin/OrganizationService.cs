using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Action = System.Action;

namespace Ada.Services.Admin
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Organization> _repository;
        public OrganizationService(IDbContext dbContext,
            IRepository<Organization> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(Organization entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(Organization entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Organization entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }
    }
}
