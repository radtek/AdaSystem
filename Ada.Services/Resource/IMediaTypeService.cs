using System.Collections.Generic;
using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;

namespace Ada.Services.Resource
{
   public interface IMediaTypeService:IDependency
   {
       void Add(MediaType entity, List<string> adPositions = null);
       void Update(MediaType entity, List<string> adPositions = null);
       void Delete(MediaType entity);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        IQueryable<MediaType> LoadEntitiesFilter(MediaTypeView viewModel);
    }
}
