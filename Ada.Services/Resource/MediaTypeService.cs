using System;
using System.Collections.Generic;
using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;

namespace Ada.Services.Resource
{
    public class MediaTypeService : IMediaTypeService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<MediaType> _repository;
        private readonly IRepository<AdPosition> _adPositionrepository;
        public MediaTypeService(IDbContext dbContext,
            IRepository<MediaType> repository, IRepository<AdPosition> adPositionrepository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _adPositionrepository = adPositionrepository;
        }
        public IQueryable<MediaType> LoadEntitiesFilter(MediaTypeView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.TypeName.Contains(viewModel.search));
            }
            if (viewModel.IsComment!=null)
            {
                allList = allList.Where(d => d.IsComment==viewModel.IsComment);
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
        public void Add(MediaType entity, List<string> adPositions = null)
        {
            _repository.Add(entity);
            if (adPositions != null)
            {
                foreach (var adPosition in adPositions)
                {
                    AdPosition temp = new AdPosition
                    {
                        Id = IdBuilder.CreateIdNum(),
                        AddedDate = DateTime.Now,
                        Name = adPosition
                    };
                    entity.AdPositions.Add(temp);
                }
            }
            _dbContext.SaveChanges();
        }

        public void Update(MediaType entity, List<string> adPositions = null)
        {
            _repository.Update(entity);
            var list = _adPositionrepository.LoadEntities(d => d.MediaTypeId == entity.Id);
            foreach (var adPosition in list)
            {
                _adPositionrepository.Remove(adPosition);
            }
            if (adPositions != null)
            {
                foreach (var adPosition in adPositions)
                {
                    AdPosition temp = new AdPosition
                    {
                        Id = IdBuilder.CreateIdNum(),
                        AddedDate = DateTime.Now,
                        Name = adPosition,
                        MediaTypeId = entity.Id
                    };
                    _adPositionrepository.Add(temp);
                }
            }
            _dbContext.SaveChanges();
        }

        public void Delete(MediaType entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }

        public MediaType GetMediaTypeByCallIndex(string callIndex)
        {
            return _repository.LoadEntities(d => d.CallIndex.Equals(callIndex, StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefault();
           
        }
    }
}
