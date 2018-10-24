using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.WorkFlow;

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
        public WorkFlowRecordDetail GetDetailById(string id)
        {
            return _detailRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
        }
    }
}
