using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;

namespace Ada.Data
{
    public class EFDbcontext : IDbContext
    {
        private readonly DbContext _dbContext;
        public EFDbcontext(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// 执行SQL语句，返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public List<T> ExecuteQuery<T>(string sql, params SqlParameter[] pars)
        {
            return _dbContext.Database.SqlQuery<T>(sql, pars).ToList();
        }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public int ExecuteSql(string sql, params SqlParameter[] pars)
        {
            return _dbContext.Database.ExecuteSqlCommand(sql, pars);
        }
        /// <summary>
        /// 保存提交
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}
