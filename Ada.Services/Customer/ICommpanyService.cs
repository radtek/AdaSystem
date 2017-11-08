using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;

namespace Ada.Services.Customer
{
   public interface ICommpanyService:IDependency
   {
       void Add(Commpany entity);
       void Update(Commpany entity);
       void Delete(Commpany entity);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        IQueryable<Commpany> LoadEntitiesFilter(CommpanyView viewModel);
    }
}
