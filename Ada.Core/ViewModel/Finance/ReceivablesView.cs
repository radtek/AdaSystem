﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.ViewModel.Business;

namespace Ada.Core.ViewModel.Finance
{
   public class ReceivablesView:BaseView
    {
        /// <summary>
        /// 收款类型
        /// </summary>
        [Display(Name = "收款类型")]
        public string ReceivablesType { get; set; }
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
        /// 实收金额
        /// </summary>
        [Display(Name = "实收金额")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 待领金额
        /// </summary>
        [Display(Name = "待领金额")]
        public decimal? BalanceMoney { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        [Display(Name = "税额")]
        public decimal? TaxMoney { get; set; }
        /// <summary>
        /// 待领金额 开始
        /// </summary>
        [Display(Name = "待领金额")]
        public decimal? BalanceMoneyMin { get; set; }
        /// <summary>
        /// 待领金额 结束
        /// </summary>
        [Display(Name = "待领金额")]
        public decimal? BalanceMoneyMax { get; set; }
        /// <summary>
        /// 收支项目
        /// </summary>
        [Display(Name = "收支项目")]
        public string IncomeExpendId { get; set; }
        /// <summary>
        /// 收支项目
        /// </summary>
        [Display(Name = "收支项目")]
        public string IncomeExpendName { get; set; }
        /// <summary>
        /// 结算账户
        /// </summary>
        [Display(Name = "结算账户")]
        public string SettleAccountName { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        [Display(Name = "结算方式")]
        public string SettleType { get; set; }
        /// <summary>
        /// 结算账户
        /// </summary>
        [Display(Name = "结算账户")]
        public string SettleAccountId { get; set; }
        /// <summary>
        /// 自定义税率
        /// </summary>
        [Display(Name = "自定义税率")]
        public decimal? Tax { get; set; }
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
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 领款明细
        /// </summary>
        [Display(Name = "领款明细")]
        public List<BusinessPayeeView> BusinessPayees { get; set; }
        /// <summary>
        /// 实收总额
        /// </summary>
        [Display(Name = "实收总额")]
        public decimal? TotalMoney { get; set; }
        /// <summary>
        /// 总税额
        /// </summary>
        [Display(Name = "总税额")]
        public decimal? TotalTaxMoney { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        [Display(Name = "单据日期")]
        public DateTime? BillDateStart { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        [Display(Name = "单据日期")]
        public DateTime? BillDateEnd { get; set; }
        /// <summary>
        /// 领款状态  true:已领完 false:未领完
        /// </summary>
        [Display(Name = "领款状态")]
        public bool? Status { get; set; }
        /// <summary>
        /// 关联单据号
        /// </summary>
        [Display(Name = "关联单据号")]
        public string RelationshipNum { get; set; }
        /// <summary>
        /// 领款人
        /// </summary>
        [Display(Name = "领款人")]
        public string PayeeBy { get; set; }
        /// <summary>
        /// 是否核销
        /// </summary>
        [Display(Name = "是否核销")]
        public bool? IsWriteOff { get; set; }
    }
}
