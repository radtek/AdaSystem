using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.WeiXin;
using Ada.Core.ViewModel.WeiXin;

namespace Ada.Services.WeiXin
{
   public class WeiXinAccountService: IWeiXinAccountService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<WeiXinAccount> _repository;
        public WeiXinAccountService(IDbContext dbContext,
            IRepository<WeiXinAccount> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(WeiXinAccount entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(WeiXinAccount entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(WeiXinAccount entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<WeiXinAccount> LoadEntitiesFilter(WeiXinAccountView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Name.Contains(viewModel.search)||d.SourceId.Contains(viewModel.search));
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
