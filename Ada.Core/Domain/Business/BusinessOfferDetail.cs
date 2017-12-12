using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Resource;

namespace Ada.Core.Domain.Business
{
    /// <summary>
    /// 报价明细
    /// </summary>
    public class BusinessOfferDetail : BaseEntity
    {
        /// <summary>
        /// 税额
        /// </summary>
        [Display(Name = "税额")]
        public decimal? TaxMoney { get; set; }
        /// <summary>
        /// 优惠金额
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
        /// 报价金额
        /// </summary>
        [Display(Name = "报价金额")]
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
        /// 媒体资源
        /// </summary>
        [Display(Name = "媒体资源")]
        public string MediaPriceId { get; set; }
        public virtual MediaPrice MediaPrice { get; set; }
        /// <summary>
        /// 报价单
        /// </summary>
        [Display(Name = "报价单")]
        public string BusinessOfferId { get; set; }
        public virtual BusinessOffer BusinessOffer { get; set; }
    }
}
