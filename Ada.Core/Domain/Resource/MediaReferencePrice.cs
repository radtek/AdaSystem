using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Resource
{
  public  class MediaReferencePrice : BaseEntity
    {
        public MediaReferencePrice()
        {
            Offer = 0;
        }
        /// <summary>
        /// 平台
        /// </summary>
        [Display(Name = "平台")]
        public string Platform { get; set; }
        /// <summary>
        /// 价格名称
        /// </summary>
        [Display(Name = "价格名称")]
        public string PriceName { get; set; }
        /// <summary>
        /// 报价
        /// </summary>
        [Display(Name = "报价")]
        public decimal Offer { get; set; }
        /// <summary>
        /// 报价日期
        /// </summary>
        [Display(Name = "报价日期")]
        public DateTime? OfferDate { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaId { get; set; }
        public virtual Media Media { get; set; }
    }
}
