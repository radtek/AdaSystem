﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;

namespace Ada.Core.ViewModel.Statistics
{
   public class WeiGuangTotal
    {
        /// <summary>
        /// 销售订单
        /// </summary>
        [Display(Name = "销售订单")]
        public int? BusinessCount { get; set; }
        /// <summary>
        /// 媒介订单
        /// </summary>
        [Display(Name = "媒介订单")]
        public int? PurchaseCount { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [Display(Name = "订单状态")]
        public bool OrderStatus { get; set; }
        /// <summary>
        /// 金额状态
        /// </summary>
        [Display(Name = "金额状态")]
        public bool MoneyStatus { get; set; }
        /// <summary>
        /// 销售金额
        /// </summary>
        [Display(Name = "销售金额")]
        public decimal? BusinessSellMoney { get; set; }
        /// <summary>
        /// 未核销金额
        /// </summary>
        [Display(Name = "未核销金额")]
        public decimal? BusinessVerificationMoney { get; set; }
        /// <summary>
        /// 已核销金额
        /// </summary>
        [Display(Name = "已核销金额")]
        public decimal? BusinessConfirmVerificationMoney { get; set; }
        /// <summary>
        /// 总收入
        /// </summary>
        [Display(Name = "总收入")]
        public decimal? Income { get; set; }
        /// <summary>
        /// 总支出
        /// </summary>
        [Display(Name = "总支出")]
        public decimal? Expend { get; set; }
        /// <summary>
        /// 资源类型
        /// </summary>
        [Display(Name = "资源类型")]
        public List<MediaTypeView> MediaTypes { get; set; }
    }
}