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
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeIndex))
            {
                allList = allList.Where(d => d.MediaType.CallIndex == viewModel.MediaTypeIndex);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeId))
            {
                allList = allList.Where(d => d.MediaTypeId == viewModel.MediaTypeId);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaID))
            {
                allList = allList.Where(d => d.MediaID.Contains(viewModel.MediaID));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManId))
            {
                allList = allList.Where(d => d.LinkManId == viewModel.LinkManId);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManName))
            {
                allList = allList.Where(d => d.LinkMan.Name.Contains(viewModel.LinkManName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.Transactor == viewModel.Transactor);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaNames))
            {
                viewModel.MediaNames = viewModel.MediaNames.Trim().Replace("\r\n", ",").Replace("，", ",").Replace(" ",",");
                var mediaNames = viewModel.MediaNames.Split(',').ToList();
                allList = allList.Where(d => mediaNames.Contains(d.MediaName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaIDs))
            {
                viewModel.MediaIDs = viewModel.MediaIDs.Trim().Replace("\r\n", ",").Replace("，", ",").Replace(" ", ",");
                var mediaIDs = viewModel.MediaIDs.Split(',').ToList();
                allList = allList.Where(d => mediaIDs.Contains(d.MediaID));
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
                return allList.OrderByDescending(d => d.LinkManId).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.LinkManId).Skip(offset).Take(rows);
        }

        public void Update(Media entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
