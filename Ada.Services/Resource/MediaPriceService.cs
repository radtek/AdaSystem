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
    public class MediaPriceService : IMediaPriceService
    {

        private readonly IRepository<MediaPrice> _repository;
        public MediaPriceService(IRepository<MediaPrice> repository)
        {
            _repository = repository;
        }
        public IQueryable<MediaPrice> LoadEntitiesFilter(MediaView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeIndex))
            {
                allList = allList.Where(d => d.Media.MediaType.CallIndex == viewModel.MediaTypeIndex);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Media.MediaName.Contains(viewModel.search));
            }
            allList = allList.Distinct();
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
