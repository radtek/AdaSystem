using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;

namespace Ada.Services.Resource
{
    public class MediaGroupService : IMediaGroupService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<MediaGroup> _repository;
        private readonly IRepository<Media> _mediaRepository;
        public MediaGroupService(IRepository<MediaGroup> repository, IRepository<Media> mediaRepository, IDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
            _mediaRepository = mediaRepository;
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
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.GroupName.Contains(viewModel.search));
            }
            if (viewModel.GroupType != null)
            {
                allList = allList.Where(d => d.GroupType == viewModel.GroupType);
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
        public void AddMedia(List<string> groupIds, Media media,short groupType)
        {
            var oldGroups = media.MediaGroups.Where(d => d.GroupType == groupType).ToList();
            //找出所有对应的媒体组，删除此媒体，再加入媒体组
            foreach (var mediaGroup in oldGroups)
            {
                mediaGroup.Medias.Remove(media);
            }
            //media.MediaGroups.Clear();
            foreach (var groupId in groupIds)
            {
                var group = _repository.LoadEntities(d => d.Id == groupId).FirstOrDefault();
                //var temp = _mediaRepository.LoadEntities(d => d.Id == media.Id).FirstOrDefault();
                group?.Medias.Add(media);
                //media.MediaGroups.Add(group);
            }
            _dbContext.SaveChanges();
        }
        public void RemoveMedia(string groupId, Media media)
        {
            var group = _repository.LoadEntities(d => d.Id == groupId).FirstOrDefault();
            if (group != null)
            {
                group.Medias.Remove(media);
                _dbContext.SaveChanges();
            }

        }
    }
}
