using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;

namespace Ada.Services.Resource
{
    public class MediaGroupService : IMediaGroupService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<MediaGroup> _repository;
        public MediaGroupService(IRepository<MediaGroup> repository, IDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        public void Add(MediaGroup entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(MediaGroup entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<MediaGroup> LoadEntitiesFilter(MediaGroupView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete==false);
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.GroupName.Contains(viewModel.search));
            }
            viewModel.total = allList.Count();
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return allList.OrderByDescending(d => d.Taxis).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.Taxis).Skip(offset).Take(rows);
        }

        public void Update(MediaGroup entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        
    }
}
