using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;

namespace Ada.Services.Customer
{
   public interface IPayAccountService:IDependency
    {
        void Add(PayAccount entity);
        void Update(PayAccount entity);
        void Delete(PayAccount entity);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        IQueryable<PayAccount> LoadEntitiesFilter(PayAccountView viewModel);
    }
}
