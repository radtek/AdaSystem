using Ada.Core;
using Ada.Core.Domain.Content;

namespace Ada.Services.Content
{
   public class ColumnService : IColumnService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Column> _repository;
        public ColumnService(IDbContext dbContext,
            IRepository<Column> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(Column entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(Column entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Column entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}
