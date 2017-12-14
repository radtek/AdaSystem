using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Finance;

namespace Ada.Services.Finance
{
   public class BillPaymentService:IBillPaymentService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<BillPayment> _repository;
        public BillPaymentService(IDbContext dbContext,
            IRepository<BillPayment> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(BillPayment entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(BillPayment entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(BillPayment entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<BillPayment> LoadEntitiesFilter(BillPaymentView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.AccountName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AccountNum))
            {
                allList = allList.Where(d => d.AccountNum.Contains(viewModel.AccountNum));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.BillNum))
            {
                allList = allList.Where(d => d.BillNum.Contains(viewModel.BillNum));
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
        public BillPayment GetByRequestNum(string requestNum)
        {
           return _repository.LoadEntities(d=>d.RequestNum==requestNum).FirstOrDefault();
        }
    }
}
