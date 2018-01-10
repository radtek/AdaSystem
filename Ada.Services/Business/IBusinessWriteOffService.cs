using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.ViewModel.Business;

namespace Ada.Services.Business
{
   public interface IBusinessWriteOffService:IDependency
    {
        void Add(BusinessWriteOff entity);
        void Update(BusinessWriteOff entity);
        void Delete(BusinessWriteOff entity);
        IQueryable<BusinessWriteOff> LoadEntitiesFilter(BusinessWriteOffView viewModel);
        IQueryable<BusinessWriteOffDetailView> LoadEntitiesFilter(BusinessWriteOffDetailView viewModel);
    }
}
