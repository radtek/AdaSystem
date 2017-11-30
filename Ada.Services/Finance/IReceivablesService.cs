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
   public interface IReceivablesService : IDependency
    {
        void Add(Receivables entity);
        void Update(Receivables entity);
        void Delete(Receivables entity);
        IQueryable<Receivables> LoadEntitiesFilter(ReceivablesView viewModel);
    }
}
