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
    public class BusinessPaymentService : IBusinessPaymentService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<BusinessPayment> _repository;
        public BusinessPaymentService(IDbContext dbContext,
            IRepository<BusinessPayment> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(BusinessPayment entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(BusinessPayment entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<BusinessPayment> LoadEntitiesFilter(BusinessPaymentView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.AccountName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.ApplicationNum))
            {
                allList = allList.Where(d => d.ApplicationNum.Contains(viewModel.ApplicationNum));
            }
            if (viewModel.AuditStatus!=null)
            {
                allList = allList.Where(d => d.AuditStatus==viewModel.AuditStatus);
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

        public void Update(BusinessPayment entity)
        {

            _repository.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
