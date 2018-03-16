using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Resource;

namespace Ada.Services.Resource
{
    public class MediaDevelopService : IMediaDevelopService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<MediaDevelop> _repository;
        
        public MediaDevelopService(IRepository<MediaDevelop> repository,
            IDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
           
        }

        public void Add(MediaDevelop entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }
        public void AddRange(IList<MediaDevelop> entities)
        {
            _repository.Add(entities);
            _dbContext.SaveChanges();
        }
        public void Delete(MediaDevelop entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<MediaDevelop> LoadEntitiesFilter(MediaDevelopView viewModel,bool isTransactor=false)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = isTransactor ? allList.Where(d => viewModel.Managers.Contains(d.TransactorId)) : allList.Where(d => viewModel.Managers.Contains(d.SubById));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeId))
            {
                allList = allList.Where(d => d.MediaTypeId == viewModel.MediaTypeId);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.search) || d.MediaID.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaName))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.MediaName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Content))
            {
                allList = allList.Where(d => d.Content.Contains(viewModel.Content));
            }
            if (viewModel.Status != null)
            {
                allList = allList.Where(d => d.Status == viewModel.Status);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Platform))
            {
                allList = allList.Where(d => d.Platform.Contains(viewModel.Platform));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaID))
            {
                allList = allList.Where(d => d.MediaID.Contains(viewModel.MediaID));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.Transactor.Contains(viewModel.Transactor));
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
       
        public void Update(MediaDevelop entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

       

    }
}
