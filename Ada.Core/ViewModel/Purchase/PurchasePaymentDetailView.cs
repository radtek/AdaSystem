﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Purchase
{
    /// <summary>
    /// 请款单
    /// </summary>
  public  class PurchasePaymentDetailView : BaseView
    {
        
        /// <summary>
        /// 供应商
        /// </summary>
        [Display(Name = "供应商")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        [Display(Name = "供应商")]
        public string LinkManId { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [Display(Name = "申请日期")]
        public DateTime? BillDate { get; set; }
        /// <summary>
        /// 申请号
        /// </summary>
        [Display(Name = "申请号")]
        public string BillNum { get; set; }
       
        /// <summary>
        /// 经办媒介
        /// </summary>
        [Display(Name = "经办媒介")]
        public string Transactor { get; set; }
        /// <summary>
        /// 经办媒介
        /// </summary>
        [Display(Name = "经办媒介")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 本次申请金额
        /// </summary>
        [Display(Name = "本次申请金额")]
        public decimal? PayMoney { get; set; }
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
        /// 付款性质
        /// </summary>
        [Display(Name = "付款性质")]
        public string PaymentType { get; set; }
        /// <summary>
        /// 付款状态
        /// </summary>
        [Display(Name = "付款状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 付款申请单
        /// </summary>
        [Display(Name = "付款申请单")]
        public string PurchasePaymentId { get; set; }
        /// <summary>
        /// 审核提醒
        /// </summary>
        [Display(Name = "审核提醒")]
        public string WarningMsg { get; set; }
        /// <summary>
        /// 是否开票
        /// </summary>
        [Display(Name = "是否开票")]
        public bool? IsInvoice { get; set; }
        /// <summary>
        /// 开票公司
        /// </summary>
        [Display(Name = "开票公司")]
        public string InvoiceTitle { get; set; }
        /// <summary>
        /// 采购总额
        /// </summary>
        [Display(Name = "采购总额")]
        public decimal? OrderMoney { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        [Display(Name = "优惠金额")]
        public decimal? DiscountMoney { get; set; }
        /// <summary>
        /// 总申请金额
        /// </summary>
        [Display(Name = "总申请金额")]
        public decimal? TotalPayMoney { get; set; }
        /// <summary>
        /// 请款备注
        /// </summary>
        [Display(Name = "请款备注")]
        public string Remark { get; set; }
    }
}
