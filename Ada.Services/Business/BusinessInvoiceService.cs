using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Business;

namespace Ada.Services.Business
{
    public class BusinessInvoiceService : IBusinessInvoiceService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<BusinessInvoice> _repository;
        private readonly IRepository<Receivables> _receivablesRepository;
        public BusinessInvoiceService(IDbContext dbContext,
            IRepository<BusinessInvoice> repository,
            IRepository<Receivables> receivablesRepository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _receivablesRepository = receivablesRepository;
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
                allList = allList.Where(d => d.Company.Contains(viewModel.search) || d.BusinessInvoiceDetails.Any(i => i.BusinessOrder.OrderNum == viewModel.search));
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

            if (viewModel.Status != null)
            {
                allList = allList.Where(d => d.Status == viewModel.Status);
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

        public bool WriteOff(IEnumerable<string> businessInvoicesIds, IEnumerable<string> receivaluesIds)
        {
            decimal? totalInvoices = 0;
            decimal? totalReceivalues = 0;
            bool isTotal = false;
            foreach (var businessInvoicesId in businessInvoicesIds)
            {
                var invoice = _repository.LoadEntities(d => d.Id == businessInvoicesId).FirstOrDefault();
                var temp = invoice.Company.Trim().ToLower() + invoice.InvoiceTitle.Trim().ToLower();
                foreach (var receivaluesId in receivaluesIds)
                {
                    var receivalues = _receivablesRepository.LoadEntities(d => d.Id == receivaluesId).FirstOrDefault();
                    var temp2 = receivalues.AccountName.Trim().ToLower() +
                                receivalues.SettleAccount.AccountName.Trim().ToLower();
                    if (temp!=temp2)
                    {
                        return false;
                    }
                    invoice.Receivableses.Add(receivalues);
                    invoice.PayTime = receivalues.BillDate;
                    if (!isTotal)
                    {
                        totalReceivalues += receivalues.Money;
                    }
                }
                isTotal = true;
                invoice.MoneyStatus = Consts.StateNormal;
                totalInvoices += invoice.TotalMoney;
            }
            //||totalReceivalues!=totalInvoices*0.96M  //
            if (totalReceivalues==totalInvoices|| totalReceivalues == totalInvoices * 0.96M)
            {
                _dbContext.SaveChanges();
                return true;
                
            }
            return false;
        }

        public void CancleWriteOff(string id)
        {

            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            foreach (var entityReceivablese in entity.Receivableses)
            {
                entityReceivablese.BusinessInvoices.Clear();
            }

            entity.MoneyStatus = Consts.StateLock;
            entity.PayTime = null;
            //entity.Receivableses.Clear();
            _dbContext.SaveChanges();
        }
    }
}
