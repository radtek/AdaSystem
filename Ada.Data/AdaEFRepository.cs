using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;

namespace Ada.Data
{
    /// <summary>
    /// 公共CURD方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AdaEFRepository<T> : IRepository<T> where T : class, new()
    {
        private readonly DbContext _context;
        public AdaEFRepository(DbContext context)
        {
            _context = context;
        }
        #region 新增实体
        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">新增实体</param>
        /// <returns>新增实体</returns>
        public virtual void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }
        #endregion

        #region 新增实体集合
        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entitites">新增实体集合</param>
        /// <returns>新增实体</returns>
        public virtual void Add(IEnumerable<T> entitites)
        {
            _context.Set<T>().AddRange(entitites);
        }
        #endregion

        #region 更新实体
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">更新实体</param>
        /// <returns>是否成功</returns>
        public virtual void Update(T entity)
        {
            //db.Set<T>().Attach(entity); //内部就是只是把实体的 状态改成unchange跟数据库查询出来的状态时一样的。
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Update(string id, IDictionary<string, object> iDictionary)
        {
            var entity = _context.Set<T>().Find(id);
            foreach (KeyValuePair<string, object> keyValuePair in iDictionary)
            {
                _context.Entry(entity).Property(keyValuePair.Key).CurrentValue = keyValuePair.Value;
                _context.Entry(entity).Property(keyValuePair.Key).IsModified = true;
            }
        }
        #endregion

        #region 删除实体
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">删除实体</param>
        /// <returns>是否成功</returns>
        public virtual void Remove(T entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
        }
        #endregion

        #region 批量删除实体
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="ids">实体ID数组</param>
        /// <returns>删除数目</returns>
        public virtual void Remove(params string[] ids)
        {
            foreach (var id in ids)
            {
                var entity = _context.Set<T>().Find(id);
                _context.Set<T>().Remove(entity);
            }
        }
        /// <summary>
        /// 批量删除实体(逻辑删除)
        /// </summary>
        /// <param name="ids">实体ID数组</param>
        /// <returns>删除数目</returns>
        public virtual void Delete(params string[] ids)
        {
            foreach (var id in ids)
            {
                var entity = _context.Set<T>().Find(id);
                _context.Entry(entity).Property("IsDelete").CurrentValue = true;
                _context.Entry(entity).Property("IsDelete").IsModified = true;
            }
        }
        #endregion

        #region 批量删除实体
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>删除数目</returns>
        public virtual void Delete(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }
        #endregion

        #region 查询实体集合
        /// <summary>
        /// 查询实体集合
        /// </summary>
        /// <param name="whereLambda">查询条件</param>
        /// <returns>实体集合</returns>
        public IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda)
        {
            return _context.Set<T>().Where(whereLambda);
        }
        #endregion

        #region 分页查询实体集合
        /// <summary>
        /// 分页查询实体
        /// </summary>
        /// <typeparam name="TK">排序字段类型</typeparam>
        /// <param name="pageSize">每页总数</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="totalPage">总页数</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="orderLambda">排序条件</param>
        /// <param name="isAsc">是否升降序</param>
        /// <returns>实体集合</returns>
        public IQueryable<T> LoadPageEntities<TK>(int pageSize, int pageIndex, out int totalPage,
            Expression<Func<T, bool>> whereLambda, Expression<Func<T, TK>> orderLambda, bool isAsc)
        {
            totalPage = _context.Set<T>().Where(whereLambda).Count();
            if (isAsc)
            {
                return _context.Set<T>().Where(whereLambda).OrderBy(orderLambda).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            }
            return _context.Set<T>().Where(whereLambda).OrderByDescending(orderLambda).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }
        #endregion
    }
}
