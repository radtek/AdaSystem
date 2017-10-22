using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain
{
    public enum StatusEnum
    {
        /// <summary>
        /// 正常|审核|开启|成功
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 锁定|草稿|关闭|待处理
        /// </summary>
        Locked = 0,
    }
}
