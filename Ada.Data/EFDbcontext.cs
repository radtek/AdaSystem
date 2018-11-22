using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
            try
            {
                return _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException exception)
                {
                    throw new Exception(GetFullErrorText(exception), exception);
                }
                throw new Exception("数据保存异常",e);
            }
            
        }
        protected string GetFullErrorText(DbEntityValidationException exc)
        {
            var msg = string.Empty;
            foreach (var validationErrors in exc.EntityValidationErrors)
            foreach (var error in validationErrors.ValidationErrors)
                msg += $"属性: {error.PropertyName} 错误: {error.ErrorMessage}" + Environment.NewLine;
            return msg;
        }
    }
}
