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
    public class MediaTagService : IMediaTagService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<MediaTag> _repository;
        public MediaTagService(IRepository<MediaTag> repository, IDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        public void Add(MediaTag entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(MediaTag entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<MediaTag> LoadEntitiesFilter(MediaTagView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.TagName.Contains(viewModel.search));
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

        public void Update(MediaTag entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public List<MediaTagView> GetTags()
        {
            return _repository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis)
                 .Select(d => new MediaTagView() { Id = d.Id, TagName = d.TagName }).ToList();
        }
        public List<MediaTagView> GetTags(string typeId)
        {
            return _repository.LoadEntities(d => d.IsDelete == false && d.Medias.Any(m => m.IsDelete == false && m.MediaTypeId == typeId)).OrderBy(d => d.Taxis)
                .Select(d => new MediaTagView() { Id = d.Id, TagName = d.TagName }).ToList();
        }
    }
}
