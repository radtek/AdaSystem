using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;

namespace Ada.Services.Purchase
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<PurchaseOrder> _repository;
        public PurchaseOrderService(IDbContext dbContext,
            IRepository<PurchaseOrder> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }


        public void Add(PurchaseOrder entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(PurchaseOrder entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(PurchaseOrder entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }
    }
}
