using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.ViewModel.Admin;

namespace Ada.Services.Admin
{
   public interface IFieldService:IDependency
   {
       void Add(Field entity);
       void Update(Field entity);
       void Delete(Field entity);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        IQueryable<Field> LoadEntitiesFilter(FieldView viewModel);
    }
}
