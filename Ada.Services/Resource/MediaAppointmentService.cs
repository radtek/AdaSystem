using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;

namespace Ada.Services.Resource
{
   public class MediaAppointmentService : IMediaAppointmentService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<MediaAppointment> _repository;
        public MediaAppointmentService(IRepository<MediaAppointment> repository,
            IDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }
        public void Delete(MediaAppointment entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<MediaAppointment> LoadEntitiesFilter(MediaAppointmentView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Media.MediaName.Contains(viewModel.search)||d.Transactor.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AppointmentDateRange))
            {
                var temp = viewModel.AppointmentDateRange.Trim().Replace("至", "#").Split('#');
                var min = Convert.ToDateTime(temp[0].Trim());
                var max = Convert.ToDateTime(temp[1].Trim()).AddDays(1);
                allList = allList.Where(d => d.AppointmentDate >= min && d.AppointmentDate < max);
            }
            viewModel.total = allList.Count();
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return allList.OrderByDescending(d => d.AppointmentDate).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.AppointmentDate).Skip(offset).Take(rows);
        }
    }
}
