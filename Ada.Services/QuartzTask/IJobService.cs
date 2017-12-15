using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.QuartzTask;

using Ada.Core.ViewModel.QuartzTask;

namespace Ada.Services.QuartzTask
{
    public interface IJobService : IDependency
    {
        void Add(Job entity);
        void Update(Job entity);
        void Delete(Job entity);
        IQueryable<Job> LoadEntitiesFilter(JobView viewModel);
    }
}
