using System.Collections.Generic;
using System.Linq;
using Ada.Core;
using Ada.Core.Domain.API;
using Ada.Core.ViewModel.API;

namespace Ada.Services.API
{
   public interface IAPIInterfacesService : IDependency
   {
       void Add(APIInterfaces entity);
       void Update(APIInterfaces entity);
       void Delete(APIInterfaces entity);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        IQueryable<APIInterfaces> LoadEntitiesFilter(APIInterfacesView viewModel);

       APIInterfaces GetAPIInterfacesByCallIndex(string callIndex);
   }
}
