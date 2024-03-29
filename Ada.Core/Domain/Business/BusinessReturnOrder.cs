﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Business
{
    /// <summary>
    /// 销售退款单
    /// </summary>
    public class BusinessReturnOrder : BaseEntity
    {
        public BusinessReturnOrder()
        {
            BusinessReturnOrderDetails = new HashSet<BusinessReturnOrderDetail>();
        }
        /// <summary>
        /// 订单状态
        /// </summary>
        [Display(Name = "订单状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 销售订单
        /// </summary>
        [Display(Name = "销售订单")]
        public string BusinessOrderId { get; set; }
        /// <summary>
        /// 退款编号
        /// </summary>
        [Display(Name = "退款编号")]
        public string ReturnOrderNum { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        [Display(Name = "退款金额")]
        public decimal? TotalMoney { get; set; }

        /// <summary>
        /// 经办业务
        /// </summary>
        [Display(Name = "经办业务")]
        public string Transactor { get; set; }
        /// <summary>
        /// 经办业务
        /// </summary>
        [Display(Name = "经办业务")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        [Display(Name = "审核人")]
        public string AuditBy { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        [Display(Name = "审核人")]
        public string AuditById { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        [Display(Name = "审核时间")]
        public DateTime? AuditDate { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        [Display(Name = "审核状态")]
        public short? AuditStatus { get; set; }
        /// <summary>
        /// 作废人
        /// </summary>
        [Display(Name = "作废人")]
        public string CancelBy { get; set; }
        /// <summary>
        /// 作废人
        /// </summary>
        [Display(Name = "作废人")]
        public string CancelById { get; set; }
        /// <summary>
        /// 作废时间
        /// </summary>
        [Display(Name = "作废时间")]
        public DateTime? CancelDate { get; set; }
        /// <summary>
        /// 单据时间
        /// </summary>
        [Display(Name = "单据时间")]
        public DateTime? ReturnDate { get; set; }

        public virtual ICollection<BusinessReturnOrderDetail> BusinessReturnOrderDetails { get; set; }
    }
}
