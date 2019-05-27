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
    public interface ISubjectService: IDependency
    {
        void Add(Subject entity);
        void Update(Subject entity);
        void Delete(Subject entity);
        IQueryable<Subject> LoadEntitiesFilter(SubjectView viewModel);
        Subject GetById(string id);
    }
}
