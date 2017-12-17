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
   public class ActionService:IActionService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Action> _repository;
        public ActionService(IDbContext dbContext,
            IRepository<Action> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(Action action)
        {

            _repository.Add(action);
            _dbContext.SaveChanges();
        }

        public void Update(Action action)
        {
            _repository.Update(action);
            _dbContext.SaveChanges();
        }

        public void Delete(Action action)
        {
            _repository.Delete(action);
            _dbContext.SaveChanges();
        }
    }
}
