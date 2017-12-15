using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Purchase;

namespace Ada.Services.Purchase
{
    public class PurchasePaymentDetailService : IPurchasePaymentDetailService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<PurchasePaymentDetail> _repository;

        public PurchasePaymentDetailService(IDbContext dbContext,
            IRepository<PurchasePaymentDetail> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public IQueryable<PurchasePaymentDetail> LoadEntitiesFilter(PurchasePaymentDetailView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            //if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            //{
            //    allList = allList.Where(d => viewModel.Managers.Contains(d.PurchasePayment.TransactorId));
            //}
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.PurchasePayment.LinkManName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.PurchasePayment.Transactor.Contains(viewModel.Transactor));
            }
            if (viewModel.AuditStatus!=null)
            {
                allList = allList.Where(d => d.AuditStatus == viewModel.AuditStatus);
            }
            if (viewModel.Status != null)
            {
                allList = allList.Where(d => d.Status == viewModel.Status);
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


        public void Update(PurchasePaymentDetail entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(PurchasePaymentDetail entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}
