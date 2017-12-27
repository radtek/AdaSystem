﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Business
{
    public class BusinessInvoiceView : BaseView
    {
        /// <summary>
        /// 发票抬头
        /// </summary>
        [Display(Name = "发票抬头")]
        public string InvoiceTitle { get; set; }
        /// <summary>
        /// 发票类型
        /// </summary>
        [Display(Name = "发票类型")]
        public string InvoiceType { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [Display(Name = "公司名称")]
        public string Company { get; set; }
        /// <summary>
        /// 纳税识别号
        /// </summary>
        [Display(Name = "纳税识别号")]
        public string TaxNum { get; set; }
        /// <summary>
        /// 注册地址
        /// </summary>
        [Display(Name = "注册地址")]
        public string Address { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        [Display(Name = "开户行")]
        public string Bank { get; set; }
        /// <summary>
        /// 开户账号
        /// </summary>
        [Display(Name = "开户账号")]
        public string BankNum { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [Display(Name = "联系电话")]
        public string Phone { get; set; }
        /// <summary>
        /// 发票状态
        /// </summary>
        [Display(Name = "发票状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 到款状态
        /// </summary>
        [Display(Name = "到款状态")]
        public short? MoneyStatus { get; set; }
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
        /// 客户名称
        /// </summary>
        [Display(Name = "客户名称")]
        public string LinkManId { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [Display(Name = "客户名称")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 开票金额
        /// </summary>
        [Display(Name = "发票金额")]
        public decimal? TotalMoney { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [Display(Name = "申请日期")]
        public DateTime? AddedDate { get; set; }
        /// <summary>
        /// 开票明细
        /// </summary>
        [Display(Name = "开票明细")]
        public string OrderDetails { get; set; }
        /// <summary>
        /// 开票内容
        /// </summary>
        [Display(Name = "开票内容")]
        public string Remark { get; set; }
        /// <summary>
        /// 开票日期
        /// </summary>
        [Display(Name = "开票日期")]
        public DateTime? InvoiceTime { get; set; }
        /// <summary>
        /// 到款日期
        /// </summary>
        [Display(Name = "到款日期")]
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 收款单号
        /// </summary>
        [Display(Name = "收款单号")]
        public string ReceivableNum { get; set; }
        /// <summary>
        /// 发票号
        /// </summary>
        [Display(Name = "发票号")]
        public string InvoiceNum { get; set; }
    }
}