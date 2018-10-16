using System.Linq;
using Ada.Core;
using Ada.Core.Domain.WorkFlow;
using Ada.Core.ViewModel.WorkFlow;

namespace Ada.Services.WorkFlow
{
   public class WorkFlowDefinitionService : IWorkFlowDefinitionService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<WorkFlowDefinition> _repository;
        public WorkFlowDefinitionService(IDbContext dbContext,
            IRepository<WorkFlowDefinition> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        
        public void Add(WorkFlowDefinition entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(WorkFlowDefinition entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(WorkFlowDefinition entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
        public IQueryable<WorkFlowDefinition> LoadEntitiesFilter(WorkFlowDefinitionView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Name.Contains(viewModel.search));
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
    }
}
