using System;
using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Finance;

namespace Ada.Services.Finance
{
    public class ReceivablesService : IReceivablesService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Receivables> _repository;
        public ReceivablesService(IDbContext dbContext,
            IRepository<Receivables> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(Receivables entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(Receivables entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Receivables entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<Receivables> LoadEntitiesFilter(ReceivablesView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.AccountName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AccountName))
            {
                allList = allList.Where(d => d.AccountName.Contains(viewModel.AccountName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AccountNum))
            {
                allList = allList.Where(d => d.AccountNum.Contains(viewModel.AccountNum));
            }
            if (viewModel.IsWriteOff!=null)
            {
                allList = viewModel.IsWriteOff.Value ? allList.Where(d => d.BusinessInvoices.Any()) : allList.Where(d => !d.BusinessInvoices.Any());
            }
            if (!string.IsNullOrWhiteSpace(viewModel.BillNum))
            {
                allList = allList.Where(d => d.BillNum.Contains(viewModel.BillNum));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.PayeeBy))
            {
                allList = allList.Where(d => d.BusinessPayees.Any(p=>p.Transactor.Contains(viewModel.PayeeBy)));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.IncomeExpendName))
            {
                allList = allList.Where(d => d.IncomeExpend.SubjectName.Contains(viewModel.IncomeExpendName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.SettleAccountName))
            {
                allList = allList.Where(d => d.SettleAccount.SettleName.Contains(viewModel.SettleAccountName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.ReceivablesType))
            {
                allList = allList.Where(d => d.ReceivablesType==viewModel.ReceivablesType);
            }
            if (viewModel.Status != null)
            {
                allList = viewModel.Status.Value ? allList.Where(d => d.BalanceMoney == 0) : allList.Where(d => d.BalanceMoney <= d.Money && d.BalanceMoney > 0);
            }
            if (viewModel.BalanceMoneyMin != null)
            {
                allList = allList.Where(d => d.BalanceMoney >= viewModel.BalanceMoneyMin);
            }
            if (viewModel.BalanceMoneyMax != null)
            {
                allList = allList.Where(d => d.BalanceMoney <= viewModel.BalanceMoneyMax);
            }
            if (viewModel.BillDateStart != null)
            {
                allList = allList.Where(d => d.BillDate >= viewModel.BillDateStart);
            }
            if (viewModel.BillDateEnd != null)
            {
                var endDate = viewModel.BillDateEnd.Value.AddDays(1);
                allList = allList.Where(d => d.BillDate < endDate);
            }
            viewModel.total = allList.Count();
            viewModel.TotalMoney = allList.Sum(d => d.Money);
            viewModel.TotalTaxMoney = allList.Sum(d => d.TaxMoney);
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
