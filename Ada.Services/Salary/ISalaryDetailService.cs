using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Wages;
using Ada.Core.ViewModel.Wages;

namespace Ada.Services.Salary
{
   public interface ISalaryDetailService : IDependency
   {
       void Delete(string id);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        IQueryable<SalaryDetail> LoadEntitiesFilter(AttendanceDetailView viewModel);

   }
}
