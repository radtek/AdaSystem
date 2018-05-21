using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Resource;

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
    }
}
