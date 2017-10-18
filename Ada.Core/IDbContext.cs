using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core
{
    /// <summary>
    /// 用于服务层的数据上下文
    /// </summary>
   public interface IDbContext
    {
        int SaveChanges();
        int ExecuteSql(string sql, params SqlParameter[] pars);
        List<T> ExecuteQuery<T>(string sql, params SqlParameter[] pars);
    }
}
