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
   public interface IMediaAppointmentService : IDependency
    {
        void Delete(MediaAppointment entity);
        IQueryable<MediaAppointment> LoadEntitiesFilter(MediaAppointmentView viewModel);
    }
}
