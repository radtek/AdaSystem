using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Finance;

namespace Ada.Services.Finance
{
   public interface IIncomeExpendService : IDependency
    {
        void Add(IncomeExpend entity);
        void Update(IncomeExpend entity);
        void Delete(IncomeExpend entity);
        IQueryable<IncomeExpend> LoadEntitiesFilter(IncomeExpendView viewModel);
    }
}
