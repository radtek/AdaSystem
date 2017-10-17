using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core
{
   public interface IRepository<T> where T : class, new()
    {

        bool Add(T entity);
        bool Add(IEnumerable<T> entitites);
        bool Update(T entity);
        bool Update(string id, IDictionary<string, object> iDictionary);
        bool Remove(T entity);
        bool Remove(params string[] ids);
        bool Delete(params string[] ids);
        bool Delete(IEnumerable<T> entities);
        IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda);
        IQueryable<T> LoadPageEntities<TK>(int pageSize, int pageIndex, out int totalPage,
            Expression<Func<T, bool>> whereLambda, Expression<Func<T, TK>> orderLambda, bool isAsc);
    }
}
