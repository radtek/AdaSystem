using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkFlow.Models
{
    public enum WorkFlowEnum
    {
        /// <summary>
        /// 审批中
        /// </summary>
        UnProecess = 0,
        /// <summary>
        /// 已审批
        /// </summary>
        Processed = 1
    }
}