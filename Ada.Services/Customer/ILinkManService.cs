using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;

namespace Ada.Services.Customer
{
   public interface ILinkManService : IDependency
   {
       void Add(LinkMan entity);
       void Update(LinkMan entity);
       void Delete(LinkMan entity);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        IQueryable<LinkMan> LoadEntitiesFilter(LinkManView viewModel);

       LinkMan CheackUser(string name);
       LinkMan GetUserByOpenId(string openId);
   }
}
