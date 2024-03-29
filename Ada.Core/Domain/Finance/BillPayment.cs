﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Customer;

namespace Ada.Core.Domain.Finance
{
    /// <summary>
    /// 付款单
    /// </summary>
   public class BillPayment:BaseEntity
    {
        public BillPayment()
        {
            BillPaymentDetails=new HashSet<BillPaymentDetail>();
        }
        /// <summary>
        /// 付款类型
        /// </summary>
        [Display(Name = "付款类型")]
        public string PaymentType { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        [Display(Name = "单据号")]
        public string BillNum { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        [Display(Name = "单据日期")]
        public DateTime? BillDate { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        [Display(Name = "商户名称")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        [Display(Name = "商户名称")]
        public string LinkManId { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        [Display(Name = "开户行")]
        public string AccountBank { get; set; }
        /// <summary>
        /// 开户名
        /// </summary>
        [Display(Name = "开户名")]
        public string AccountName { get; set; }
        /// <summary>
        /// 开户号
        /// </summary>
        [Display(Name = "开户号")]
        public string AccountNum { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        [Display(Name = "经办人")]
        public string Transactor { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        [Display(Name = "经办人")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 申请单类型 0 采购付款单 1 业务付款单
        /// </summary>
        [Display(Name = "申请单类型")]
        public short? RequestType { get; set; }
        /// <summary>
        /// 申请单号
        /// </summary>
        [Display(Name = "申请单号")]
        public string RequestNum { get; set; }
        /// <summary>
        /// 付款凭证
        /// </summary>
        [Display(Name = "付款凭证")]
        public string Image { get; set; }
        public virtual ICollection<BillPaymentDetail> BillPaymentDetails { get; set; }
        public virtual LinkMan LinkMan { get; set; }
    }
}
