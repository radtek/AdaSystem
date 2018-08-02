using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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
    public class MediaArticleService : IMediaArticleService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<MediaArticle> _repository;
        public MediaArticleService(
            IDbContext dbContext,
            IRepository<MediaArticle> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }

        public void Delete(string[] ids)
        {
            _repository.Remove(ids);
            _dbContext.SaveChanges();
        }

        public IQueryable<MediaArticle> LoadEntitiesFilter(MediaArticleView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(viewModel.MediaId))
            {
                allList = allList.Where(d => d.MediaId==viewModel.MediaId);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Title.Contains(viewModel.search));
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
        public void Add(MediaArticle entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }



    }
}
