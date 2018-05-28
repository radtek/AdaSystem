using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Resource
{
    /// <summary>
    /// 价格变动
    /// </summary>
   public class MediaPriceChange : BaseEntity
    {
       /// <summary>
        /// 采购价格
        /// </summary>
        [Display(Name = "采购价格")]
        public decimal? PurchasePrice { get; set; }
        /// <summary>
        /// 销售价格
        /// </summary>
        [Display(Name = "销售价格")]
        public decimal? MarketPrice { get; set; }
        /// <summary>
        /// 零售价格
        /// </summary>
        [Display(Name = "零售价格")]
        public decimal? SellPrice { get; set; }
        /// <summary>
        /// 变动日期
        /// </summary>
        [Display(Name = "变动日期")]
        public DateTime? ChangeDate { get; set; }
        /// <summary>
        /// 媒体价格
        /// </summary>
        [Display(Name = "媒体价格")]
        public string MediaPriceId { get; set; }
        public virtual MediaPrice MediaPrice { get; set; }
    }
}
