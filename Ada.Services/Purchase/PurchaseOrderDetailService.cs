using System;
using System.Collections.Generic;
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
    public class PurchaseOrderDetailService : IPurchaseOrderDetailService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<PurchaseOrderDetail> _repository;
        public PurchaseOrderDetailService(IDbContext dbContext,
            IRepository<PurchaseOrderDetail> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public IQueryable<PurchaseOrderDetail> LoadEntitiesFilter(PurchaseOrderDetailView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaName))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.MediaName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeName))
            {
                allList = allList.Where(d => d.MediaTypeName.Contains(viewModel.MediaTypeName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.BusinessBy))
            {
                allList = allList.Where(d => d.PurchaseOrder.BusinessBy.Contains(viewModel.BusinessBy));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManName))
            {
                allList = allList.Where(d => d.LinkManName.Contains(viewModel.LinkManName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManId))
            {
                allList = allList.Where(d => d.LinkManId.Contains(viewModel.LinkManId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.Transactor.Contains(viewModel.Transactor));
            }
            if (viewModel.Status != null)
            {
                allList = allList.Where(d => d.Status == viewModel.Status);
            }
            if (viewModel.AuditStatus != null)
            {
                allList = allList.Where(d => d.AuditStatus == viewModel.AuditStatus);
            }
            if (viewModel.IsPayment==false)//过滤没有请款的
            {
                allList = allList.Where(d => d.PurchasePaymentOrderDetails.Count==0);
            }
            if (viewModel.PublishDateStart != null)
            {
                allList = allList.Where(d => d.PublishDate >= viewModel.PublishDateStart);
            }
            if (viewModel.PublishDateEnd != null)
            {
                var endDate = viewModel.PublishDateEnd.Value.AddDays(1);
                allList = allList.Where(d => d.PublishDate < endDate);
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

        public void Add(PurchaseOrderDetail entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(PurchaseOrderDetail entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(PurchaseOrderDetail entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}
