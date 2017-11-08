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
    public class MediaService : IMediaService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Media> _repository;
        public MediaService(IRepository<Media> repository, IDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }
        public IQueryable<Media> LoadEntitiesFilter(MediaView viewModel)
        {
            var allList = _repository.LoadEntities(d => true);
            if (!string.IsNullOrWhiteSpace(viewModel.AdPositionId))
            {
                allList = from m in allList
                          from p in m.MediaPrices
                          where p.AdPositionId==viewModel.AdPositionId
                          select m;
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
