using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core
{
    /// <summary>
    /// 服务于数据层的接口，规范CURD
    /// </summary>
    /// <typeparam name="T"></typeparam>
   public interface IRepository<T> where T : class, new()
    {

        void Add(T entity);
        void Add(IEnumerable<T> entitites);
        void Update(T entity);
        void Update(string id, IDictionary<string, object> iDictionary);
        int Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> updateLambda);
        void Remove(T entity);
        void Remove(params string[] ids);
        void Remove(IEnumerable<T> entities);
        void Remove(Expression<Func<T, bool>> whereLambda);
        void Delete(params string[] ids);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
        IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda);
        IQueryable<T> LoadPageEntities<TK>(int pageSize, int pageIndex, out int totalPage,
            Expression<Func<T, bool>> whereLambda, Expression<Func<T, TK>> orderLambda, bool isAsc);
    }
}
