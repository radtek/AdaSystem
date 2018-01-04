using System;
using System.Collections.Generic;
using System.Linq;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Finance;

namespace Ada.Services.Finance
{
    public class SettleAccountService : ISettleAccountService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<SettleAccount> _repository;
        public SettleAccountService(IDbContext dbContext,
            IRepository<SettleAccount> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(SettleAccount entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(SettleAccount entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(SettleAccount entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }
        public IQueryable<SettleAccount> LoadEntitiesFilter()
        {
            return _repository.LoadEntities(d => d.IsDelete == false);
        }

        /// <summary>
        /// 收支统计
        /// </summary>
        public IList<SettleAccountView> BalanceStatistics(DateTime start, DateTime end)
        {
            var accounts = _repository.LoadEntities(d => d.IsDelete == false).ToList();
            IList<SettleAccountView> settleAccountViews = new List<SettleAccountView>();
            foreach (var settleAccount in accounts)
            {
                SettleAccountView settleAccountView = new SettleAccountView();
                settleAccountView.SettleName = settleAccount.SettleName;
                //主营业收入
                //其他收入
                var incomeSum = settleAccount.ExpenseDetails
                    .Where(d => d.IncomeExpend.SubjectType == Consts.StateNormal && d.IsDelete == false && d.Expense.IsDelete == false && d.Expense.BillDate >= start && d.Expense.BillDate < end)
                    .Sum(d => d.Money);
                var receivablesSum = settleAccount.Receivableses
                    .Where(d => d.IncomeExpend.SubjectType == Consts.StateNormal && d.IsDelete == false && d.BillDate >= start && d.BillDate < end)
                    .Sum(d => d.Money);
                settleAccountView.Income = incomeSum + receivablesSum;
                //主营业支出
                //其他支出
                var billSum = settleAccount.BillPaymentDetails
                    .Where(d => d.IncomeExpend.SubjectType == Consts.StateLock && d.IsDelete == false && d.BillPayment.IsDelete == false && d.BillPayment.BillDate >= start && d.BillPayment.BillDate < end)
                    .Sum(d => d.Money);
                var expendSum = settleAccount.ExpenseDetails
                    .Where(d => d.IncomeExpend.SubjectType == Consts.StateLock && d.IsDelete == false && d.Expense.IsDelete == false && d.Expense.BillDate >= start && d.Expense.BillDate < end)
                    .Sum(d => d.Money);
                settleAccountView.Expend = billSum + expendSum;
                settleAccountView.TotalMoney =
                    settleAccount.Money + settleAccountView.Income - settleAccountView.Expend;
                settleAccountView.Money = settleAccount.Money;
                settleAccountViews.Add(settleAccountView);
            }

            return settleAccountViews;
        }
    }
}
