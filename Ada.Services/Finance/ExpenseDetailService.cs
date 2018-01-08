using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Finance;

namespace Ada.Services.Finance
{
   public class ExpenseDetailService: IExpenseDetailService
    {
        private readonly IRepository<ExpenseDetail> _repository;
        public ExpenseDetailService(IRepository<ExpenseDetail> repository)
        {
            _repository = repository;
        }
        public IQueryable<ExpenseDetail> LoadEntitiesFilter(ReceiptExpenditureView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false && d.Expense.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.SettleAccountName))
            {
                allList = allList.Where(d => d.SettleAccount.SettleName.Contains(viewModel.SettleAccountName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.SettleAccount.SettleName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.IncomeExpendName))
            {
                allList = allList.Where(d => d.IncomeExpend.SubjectName.Contains(viewModel.IncomeExpendName));
            }
            if (viewModel.BillDateStart!=null)
            {
                allList = allList.Where(d => d.Expense.BillDate >= viewModel.BillDateStart);
            }
            if (viewModel.BillDateEnd != null)
            {
                var endDate = viewModel.BillDateEnd.Value.AddDays(1);
                allList = allList.Where(d => d.Expense.BillDate < endDate);
            }
            viewModel.total = allList.Count();
            viewModel.TotalExpenditureMoney =
                allList.Where(d => d.IncomeExpend.SubjectType == Consts.StateLock).Sum(d => d.Money);
            viewModel.TotalReceiptMoney =
                allList.Where(d => d.IncomeExpend.SubjectType == Consts.StateNormal).Sum(d => d.Money);
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
