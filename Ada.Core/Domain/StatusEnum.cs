﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain
{
    public enum StatusEnum
    {
        /// <summary>
        /// 正常|审核|开启|成功|处理中
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 锁定|草稿|关闭|待处理
        /// </summary>
        Locked = 0,
        /// <summary>
        /// 拒绝|待确认
        /// </summary>
        Error = -1,
        /// <summary>
        /// 成功
        /// </summary>
        Success = 2,
        /// <summary>
        /// 失败
        /// </summary>
        Fail = 3
    }
    public enum AuditEnum
    {
        /// <summary>
        /// 确认申请
        /// </summary>
        Confirm = -1,
        /// <summary>
        /// 价格申请
        /// </summary>
        Price = -2
       
    }
    public enum PurchaseStatusEnum
    {
        /// <summary>
        /// 待处理
        /// </summary>
        Wait = 0,
        /// <summary>
        /// 已处理
        /// </summary>
        Todo = 1,
        /// <summary>
        /// 失败
        /// </summary>
        Fail = -1,
        /// <summary>
        /// 已成功
        /// </summary>
        Success = 3,
        /// <summary>
        /// 已确认
        /// </summary>
        Confirm = 2

    }
}
