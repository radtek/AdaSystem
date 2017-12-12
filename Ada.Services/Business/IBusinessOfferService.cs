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
    public interface IBusinessOfferService:IDependency
    {
        void Add(BusinessOffer entity);
        void Update(BusinessOffer entity);
        void Delete(BusinessOffer entity);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        IQueryable<BusinessOffer> LoadEntitiesFilter(BusinessOfferView viewModel);
    }
}
