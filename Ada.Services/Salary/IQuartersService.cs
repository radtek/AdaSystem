using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Wages;
using Ada.Core.ViewModel.Wages;

namespace Ada.Services.Salary
{
   public interface IQuartersService : IDependency
   {
       void Add(Quarters entity);
       void Update(Quarters entity);
       void Delete(Quarters entity);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        IQueryable<Quarters> LoadEntitiesFilter(QuartersView viewModel);

   }
}
