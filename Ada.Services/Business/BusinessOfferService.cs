using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.ViewModel.Business;

namespace Ada.Services.Business
{
    public class BusinessOfferService : IBusinessOfferService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<BusinessOffer> _repository;
        public BusinessOfferService(IDbContext dbContext,
            IRepository<BusinessOffer> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }


        public IQueryable<BusinessOffer> LoadEntitiesFilter(BusinessOfferView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.LinkMan.Name.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManId))
            {
                allList = allList.Where(d => d.LinkManId == viewModel.LinkManId);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.Transactor.Contains(viewModel.Transactor));
            }
            if (viewModel.Status!=null)
            {
                allList = allList.Where(d => d.Status==viewModel.Status);
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
        public void Add(BusinessOffer entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(BusinessOffer entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(BusinessOffer entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }
    }
}
