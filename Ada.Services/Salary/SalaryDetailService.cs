using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Wages;
using Ada.Core.ViewModel.Wages;

namespace Ada.Services.Salary
{
    public class SalaryDetailService : ISalaryDetailService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<SalaryDetail> _repository;
        private readonly IRepository<AttendanceDetail> _attendanceRepository;
        public SalaryDetailService(IDbContext dbContext,
            IRepository<SalaryDetail> repository,
            IRepository<AttendanceDetail> attendanceRepository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _attendanceRepository = attendanceRepository;
        }
        public IQueryable<SalaryDetail> LoadEntitiesFilter(AttendanceDetailView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Manager.UserName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.ManagerId))
            {
                allList = allList.Where(d => d.ManagerId==viewModel.ManagerId);
            }

            if (viewModel.Date!=null)
            {
                allList = allList.Where(d => d.Date == viewModel.Date);
            }
            viewModel.total = allList.Count();
            if (allList.Any())
            {
                viewModel.TotalSum = allList.Sum(d => d.Total);
            }
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return allList.OrderByDescending(d => d.Id).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.Id).Skip(offset).Take(rows);
        }
        
        public void Delete(string id)
        {
            //明细
           var entity= _repository.LoadEntities(d=>d.Id==id).FirstOrDefault();
            //考勤
            var managerId = entity.ManagerId;
            var date = entity.Date;
            var detail = _attendanceRepository
                .LoadEntities(d => d.ManagerId == managerId && d.Date == date).FirstOrDefault();
            _repository.Remove(entity);
            _attendanceRepository.Remove(detail);
            _dbContext.SaveChanges();
        }

        
    }
}
