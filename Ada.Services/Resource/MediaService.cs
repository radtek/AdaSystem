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

        public void Add(Media entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Media entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<Media> LoadEntitiesFilter(MediaView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeIndex))
            {
                allList = allList.Where(d => d.MediaType.CallIndex == viewModel.MediaTypeIndex);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManName))
            {
                allList = allList.Where(d => d.LinkMan.Name.Contains(viewModel.LinkManName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AddedBy))
            {
                allList = allList.Where(d => d.AddedBy==viewModel.AddedBy);
            }
            if (viewModel.MediaTagIds != null)
            {
                allList = from m in allList
                          from t in m.MediaTags
                          where viewModel.MediaTagIds.Contains(t.Id)
                          select m;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AdPositionName))
            {
                allList = from m in allList
                          from p in m.MediaPrices
                          where p.AdPositionName == viewModel.AdPositionName
                          select m;
                if (viewModel.PriceStart != null)
                {
                    allList = from m in allList
                              from p in m.MediaPrices
                              where p.PurchasePrice >= viewModel.PriceStart && p.AdPositionName == viewModel.AdPositionName
                              select m;
                }
                if (viewModel.PriceEnd != null)
                {
                    allList = from m in allList
                              from p in m.MediaPrices
                              where p.PurchasePrice <= viewModel.PriceEnd && p.AdPositionName == viewModel.AdPositionName
                              select m;
                }

            }
            else
            {
                if (viewModel.PriceStart != null)
                {
                    allList = from m in allList
                              from p in m.MediaPrices
                              where p.PurchasePrice >= viewModel.PriceStart
                              select m;
                }
                if (viewModel.PriceEnd != null)
                {
                    allList = from m in allList
                              from p in m.MediaPrices
                              where p.PurchasePrice <= viewModel.PriceEnd
                              select m;
                }
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

        public void Update(Media entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
