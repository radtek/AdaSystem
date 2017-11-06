using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;

namespace Ada.Services.Admin
{
   public interface IFieldTypeService:IDependency
   {
       void Add(FieldType entity);
       void Update(FieldType entity);
       void Delete(FieldType entity);
   }
}
