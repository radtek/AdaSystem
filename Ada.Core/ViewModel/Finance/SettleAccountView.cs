﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Finance
{
    public class SettleAccountView : BaseView
    {
        /// <summary>
        /// 结算账户
        /// </summary>
        [Display(Name = "结算账户")]
        [StringLength(32, ErrorMessage = "字符长度不能超过32")]
        public string SettleName { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        [Display(Name = "开户行")]
        [StringLength(32, ErrorMessage = "字符长度不能超过32")]
        public string AccountBank { get; set; }
        /// <summary>
        /// 开户名
        /// </summary>
        [Display(Name = "开户名")]
        [StringLength(32, ErrorMessage = "字符长度不能超过32")]
        public string AccountName { get; set; }
        /// <summary>
        /// 开户号
        /// </summary>
        [Display(Name = "开户号")]
        [StringLength(32, ErrorMessage = "字符长度不能超过32")]
        public string AccountNum { get; set; }
        /// <summary>
        /// 账户金额
        /// </summary>
        [Display(Name = "账户金额")]
        public decimal? Money { get; set; }
    }
}