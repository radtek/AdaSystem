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
    public interface IBusinessOrderService:IDependency
    {
        void Add(BusinessOrder entity);
        void Update(BusinessOrder entity);
        void Delete(BusinessOrder entity);
        void Remove(BusinessOrder entity);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        IQueryable<BusinessOrder> LoadEntitiesFilter(BusinessOrderView viewModel);
    }
}
