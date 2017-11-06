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
   public class FieldTypeService : IFieldTypeService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<FieldType> _repository;
        public FieldTypeService(IDbContext dbContext,
            IRepository<FieldType> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(FieldType entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(FieldType entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(FieldType entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }
    }
}
