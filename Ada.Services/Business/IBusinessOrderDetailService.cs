using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Business;
using Ada.Core.ViewModel.Business;

namespace Ada.Services.Business
{
    public interface IBusinessOrderDetailService : IDependency
    {
        //void Add(BusinessOrder entity);
        //void Update(BusinessOrder entity);
        //void Delete(BusinessOrder entity);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        IQueryable<BusinessOrderDetail> LoadEntitiesFilter(BusinessOrderDetailView viewModel);

        List<BusinessOrderDetailView> BusinessPerformance(List<Manager> managers);
        BusinessOrderDetailView BusinessPerformance(string transactorId);
    }
}
