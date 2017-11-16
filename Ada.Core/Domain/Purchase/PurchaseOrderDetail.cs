using System;
using System.ComponentModel.DataAnnotations;
using Ada.Core.Domain.Resource;

namespace Ada.Core.Domain.Purchase
{
    /// <summary>
    /// 采购明细
    /// </summary>
    public class PurchaseOrderDetail : BaseEntity
    {
        /// <summary>
        /// 销售明细单
        /// </summary>
        [Display(Name = "销售明细单")]
        public string BusinessOrderDetailId { get; set; }
        /// <summary>
        /// 折扣金额
        /// </summary>
        [Display(Name = "折扣金额")]
        public decimal? DiscountMoney { get; set; }
        /// <summary>
        /// 折扣%
        /// </summary>
        [Display(Name = "折扣%")]
        public decimal? DiscountRate { get; set; }
        /// <summary>
        /// 采购金额
        /// </summary>
        [Display(Name = "采购金额")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 成本金额
        /// </summary>
        [Display(Name = "成本金额")]
        public decimal? CostMoney { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        [Display(Name = "税额")]
        public decimal? TaxMoney { get; set; }
        /// <summary>
        /// 税率%
        /// </summary>
        [Display(Name = "税率%")]
        public decimal? Tax { get; set; }
        /// <summary>
        /// 无税金额
        /// </summary>
        [Display(Name = "无税金额")]
        public decimal? PurchaseMoney { get; set; }
        /// <summary>
        /// 广告位
        /// </summary>
        [Display(Name = "广告位")]
        public string AdPositionName { get; set; }
        /// <summary>
        /// 出刊日期
        /// </summary>
        [Display(Name = "出刊日期")]
        public DateTime? PublishDate { get; set; }
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
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaPriceId { get; set; }
        public virtual MediaPrice MediaPrice { get; set; }
        /// <summary>
        /// 采购订单
        /// </summary>
        [Display(Name = "采购订单")]
        public string PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
    }
}
