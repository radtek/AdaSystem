using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.WorkFlow;
using Ada.Core.ViewModel.WorkFlow;

namespace Ada.Services.WorkFlow
{
    public class WorkFlowService : IWorkFlowService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<WorkFlowRecord> _repository;
        private readonly IRepository<WorkFlowRecordDetail> _detailRepository;

        public WorkFlowService(IDbContext dbContext,
            IRepository<WorkFlowRecord> repository,
            IRepository<WorkFlowRecordDetail> detailRepository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _detailRepository = detailRepository;
        }
        public WorkFlowRecord GetRecordByWfInstanceId(string wfInstanceId)
        {
            return _repository.LoadEntities(d => d.WfInstanceId == wfInstanceId).FirstOrDefault();
        }
        public WorkFlowRecord GetRecordById(string id)
        {
            return _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
        }
        public void UpdateRecord(WorkFlowRecord workFlowRecord)
        {
            _repository.Update(workFlowRecord);
            _dbContext.SaveChanges();
        }
        public void UpdateDetail(WorkFlowRecordDetail detail)
        {
            _detailRepository.Update(detail);
            _dbContext.SaveChanges();
        }
        public void DeleteRecord(string id)
        {
            string[] ids = { id };
            _repository.Remove(ids);
            _dbContext.SaveChanges();
        }
        public WorkFlowRecordDetail GetDetailById(string id)
        {
            return _detailRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
        }

        public IQueryable<WorkFlowRecord> LoadRecordsFilter(WorkFlowRecordView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.AddedById));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Title.Contains(viewModel.search));
            }

            if (viewModel.MyApprove == true)
            {
                allList = allList.Where(d =>d.Status==Consts.StateLock&&
                    d.WorkFlowRecordDetails.Any(p =>
                        p.Status == Consts.StateLock && p.ProcessById == viewModel.FlowTo));
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
    }
}
