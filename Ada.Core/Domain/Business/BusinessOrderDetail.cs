﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Resource;

namespace Ada.Core.Domain.Business
{
    /// <summary>
    /// 销售明细
    /// </summary>
    public class BusinessOrderDetail : BaseEntity
    {
        public BusinessOrderDetail()
        {
            BusinessWriteOffs=new HashSet<BusinessWriteOff>();
            OrderDetailComments=new HashSet<OrderDetailComment>();
        }
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
        /// 无税金额
        /// </summary>
        [Display(Name = "无税金额")]
        public decimal? SellMoney { get; set; }
        /// <summary>
        /// 申请修改金额
        /// </summary>
        [Display(Name = "申请修改金额")]
        public decimal? RequestSellMoney { get; set; }
        /// <summary>
        /// 未核销金额
        /// </summary>
        [Display(Name = "未核销金额")]
        public decimal? VerificationMoney { get; set; }
        /// <summary>
        /// 已核销金额
        /// </summary>
        [Display(Name = "已核销金额")]
        public decimal? ConfirmVerificationMoney { get; set; }
        /// <summary>
        /// 核销状态
        /// </summary>
        [Display(Name = "核销状态")]
        public short? VerificationStatus { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        [Display(Name = "审核状态")]
        public short? AuditStatus { get; set; }
        /// <summary>
        /// 申请说明
        /// </summary>
        [Display(Name = "申请说明")]
        public string AuditRemark { get; set; }
        /// <summary>
        /// 审核日期
        /// </summary>
        [Display(Name = "审核日期")]
        public DateTime? AuditDate { get; set; }
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
        /// 更新日期
        /// </summary>
        [Display(Name = "更新日期")]
        public DateTime? MediaUpdate { get; set; }
        /// <summary>
        /// 媒体指数
        /// </summary>
        [Display(Name = "媒体指数")]
        public string UpdateContent { get; set; }
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
        /// 订单状态
        /// </summary>
        [Display(Name = "订单状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 是否推荐
        /// </summary>
        [Display(Name = "是否推荐")]
        public bool? IsRecommend { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaPriceId { get; set; }
        public virtual MediaPrice MediaPrice { get; set; }
        /// <summary>
        /// 销售订单
        /// </summary>
        [Display(Name = "销售订单")]
        public string BusinessOrderId { get; set; }
        public virtual BusinessOrder BusinessOrder { get; set; }
        public virtual ICollection<BusinessWriteOff> BusinessWriteOffs { get; set; }
        public virtual ICollection<OrderDetailComment> OrderDetailComments { get; set; }
    }
}
