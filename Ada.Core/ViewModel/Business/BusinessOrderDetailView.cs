﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Business
{
    public class BusinessOrderDetailView:BaseView
    {
        /// <summary>
        /// 税额
        /// </summary>
        [Display(Name = "税额")]
        public decimal? TaxMoney { get; set; }
        /// <summary>
        /// 折扣金额
        /// </summary>
        [Display(Name = "优惠金额")]
        public decimal? DiscountMoney { get; set; }
        /// <summary>
        /// 税率%
        /// </summary>
        [Display(Name = "税率%")]
        public decimal? Tax { get; set; }
        /// <summary>
        /// 折扣%
        /// </summary>
        [Display(Name = "折扣%")]
        public decimal? DiscountRate { get; set; }
        /// <summary>
        /// 广告位
        /// </summary>
        [Display(Name = "广告位")]
        public string AdPositionName { get; set; }
        /// <summary>
        /// 销售金额
        /// </summary>
        [Display(Name = "销售金额")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 成本金额
        /// </summary>
        [Display(Name = "成本金额")]
        public decimal? CostMoney { get; set; }
        /// <summary>
        /// 销售金额
        /// </summary>
        [Display(Name = "无税金额")]
        public decimal? SellMoney { get; set; }
        /// <summary>
        /// 出刊日期
        /// </summary>
        [Display(Name = "出刊日期")]
        public DateTime? PublishDate { get; set; }
        /// <summary>
        /// 预出刊日期
        /// </summary>
        [Display(Name = "预出刊日期")]
        public DateTime? PrePublishDate { get; set; }
        /// <summary>
        /// 稿件标题
        /// </summary>
        [Display(Name = "稿件标题")]
        public string MediaTitle { get; set; }
        /// <summary>
        /// 出刊链接
        /// </summary>
        [Display(Name = "出刊链接")]
        public string PublishLink { get; set; }
        /// <summary>
        /// 媒体类型
        /// </summary>
        [Display(Name = "媒体类型")]
        public string MediaTypeName { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaName { get; set; }
        /// <summary>
        /// 经办媒介
        /// </summary>
        [Display(Name = "经办媒介")]
        public string MediaByPurchase { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaPriceId { get; set; }
        /// <summary>
        /// 采购状态
        /// </summary>
        [Display(Name = "采购状态")]
        public short? PurchaseStatus { get; set; }
    }
}