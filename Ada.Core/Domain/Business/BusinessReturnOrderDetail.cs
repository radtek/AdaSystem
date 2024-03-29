﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Business
{
    /// <summary>
    /// 销售退款明细
    /// </summary>
    public class BusinessReturnOrderDetail : BaseEntity
    {
        /// <summary>
        /// 销售明细单
        /// </summary>
        [Display(Name = "销售明细单")]
        public string BusinessOrderDetailId { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        [Display(Name = "退款金额")]
        public decimal? Money { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        [Display(Name = "退款原因")]
        public string ReturnReason { get; set; }
        /// <summary>
        /// 退款类型
        /// </summary>
        [Display(Name = "退款类型")]
        public string ReturnType { get; set; }
        /// <summary>
        /// 退款日期
        /// </summary>
        [Display(Name = "退款日期")]
        public DateTime? ReturnDate { get; set; }
        /// <summary>
        /// 销售退款订单
        /// </summary>
        [Display(Name = "销售退款订单")]
        public string BusinessReturnOrderId { get; set; }
        public virtual BusinessReturnOrder BusinessReturnOrder { get; set; }
    }
}
