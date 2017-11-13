using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Resource;

namespace Ada.Core.Domain.Business
{
   public class BusinessOrderDetail:BaseEntity
    {
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "订单状态")]
        public string Status { get; set; }
        /// <summary>
        /// 折扣金额
        /// </summary>
        [Display(Name = "折扣金额")]
        public string DiscountMoney { get; set; }
        /// <summary>
        /// 折扣比例
        /// </summary>
        [Display(Name = "折扣比例")]
        public string DiscountRate { get; set; }
        /// <summary>
        /// 广告位
        /// </summary>
        [Display(Name = "广告位")]
        public string AdPositionName { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [Display(Name = "应收金额")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [Display(Name = "成本金额")]
        public decimal? CostMoney { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [Display(Name = "销售金额")]
        public decimal? SellMoney { get; set; }
        /// <summary>
        /// 经办业务
        /// </summary>
        [Display(Name = "经办媒介")]
        public string Transactor { get; set; }
        /// <summary>
        /// 经办业务
        /// </summary>
        [Display(Name = "经办媒介")]
        public string TransactorId { get; set; }
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
        /// 联系客户
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaPriceId { get; set; }
        public virtual MediaPrice MediaPrice { get; set; }
        /// <summary>
        /// 联系客户
        /// </summary>
        [Display(Name = "业务订单")]
        public string BusinessOrderId { get; set; }
        public virtual BusinessOrder BusinessOrder { get; set; }
    }
}
