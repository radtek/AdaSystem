using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Demand;
using Ada.Core.ViewModel.Demand;

namespace Ada.Services.Demand
{
    public interface ISubjectDetailService : IDependency
    {
        void Update(SubjectDetail entity);
        void Delete(SubjectDetail entity);
        IQueryable<SubjectDetail> LoadEntitiesFilter(SubjectDetailView viewModel);
        SubjectDetail GetById(string id);
        SubjectDetailProgress GetProgressById(string id);
    }
}
