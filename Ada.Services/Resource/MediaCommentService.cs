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
    public class MediaCommentService : IMediaCommentService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<MediaComment> _repository;
        public MediaCommentService(
            IDbContext dbContext,
            IRepository<MediaComment> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }

        public void Delete(string[] ids)
        {
            _repository.Remove(ids);
            _dbContext.SaveChanges();
        }

        public IQueryable<MediaComment> LoadEntitiesFilter(MediaCommentView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //var isInclud = false;
            //if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            //{
            //    allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
            //}
            
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Media.MediaName.Contains(viewModel.search) || d.Media.MediaID.Contains(viewModel.search)&&d.Transactor.Contains(viewModel.search));
            }

            if (viewModel.Score!=null)
            {
                allList = allList.Where(d => d.Score == viewModel.Score);
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
