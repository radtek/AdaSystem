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
    public class BusinessInvoiceService : IBusinessInvoiceService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<BusinessInvoice> _repository;
        public BusinessInvoiceService(IDbContext dbContext,
            IRepository<BusinessInvoice> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(BusinessInvoice entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(BusinessInvoice entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<BusinessInvoice> LoadEntitiesFilter(BusinessInvoiceView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Company.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.Transactor.Contains(viewModel.Transactor));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Company))
            {
                allList = allList.Where(d => d.Company.Contains(viewModel.Company));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.TaxNum))
            {
                allList = allList.Where(d => d.TaxNum.Contains(viewModel.TaxNum));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.InvoiceTitle))
            {
                allList = allList.Where(d => d.InvoiceTitle.Contains(viewModel.InvoiceTitle));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.InvoiceNum))
            {
                allList = allList.Where(d => d.InvoiceNum.Contains(viewModel.InvoiceNum));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.InvoiceType))
            {
                allList = allList.Where(d => d.InvoiceType.Contains(viewModel.InvoiceType));
            }
            
            if (viewModel.Status!=null)
            {
                allList = allList.Where(d => d.Status==viewModel.Status);
            }
            if (viewModel.MoneyStatus != null)
            {
                allList = allList.Where(d => d.MoneyStatus == viewModel.MoneyStatus);
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

        public void Update(BusinessInvoice entity)
        {

            _repository.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
