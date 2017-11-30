using System;
using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Finance;

namespace Ada.Services.Finance
{
    public class SettleAccountService : ISettleAccountService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<SettleAccount> _repository;
        public SettleAccountService(IDbContext dbContext,
            IRepository<SettleAccount> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(SettleAccount entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(SettleAccount entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(SettleAccount entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }
        public IQueryable<SettleAccount> LoadEntitiesFilter()
        {
            return _repository.LoadEntities(d => d.IsDelete == false);
        }
    }
}
