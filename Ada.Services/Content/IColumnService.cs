using Ada.Core;
using Ada.Core.Domain.Content;

namespace Ada.Services.Content
{
   public interface IColumnService : IDependency
   {
       void Add(Column entity);
       void Update(Column entity);
       void Delete(Column entity);
   }
}
