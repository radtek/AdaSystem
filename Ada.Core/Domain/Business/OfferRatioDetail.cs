using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Business
{
   public class OfferRatioDetail:BaseEntity
    {
        /// <summary>
        /// 最大值
        /// </summary>
        [Display(Name = "最大值")]
        public decimal? OfferMax { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        [Display(Name = "最小值")]
        public decimal? OfferMin { get; set; }
        /// <summary>
        /// 系数类型
        /// </summary>
        [Display(Name = "系数类型")]
        public bool? RatioType { get; set; }
        /// <summary>
        /// 增加值
        /// </summary>
        [Display(Name = "增加值")]
        public decimal? RatioValue { get; set; }
        /// <summary>
        /// 联系客户
        /// </summary>
        [Display(Name = "报价规则")]
        public string OfferRatioId { get; set; }
        public virtual OfferRatio OfferRatio { get; set; }
    }
}
