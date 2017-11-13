using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Business
{
   public class BusinessCart : BaseEntity
    {

        /// <summary>
        /// 媒体信息
        /// </summary>
        [Display(Name = "媒体信息")]
        public string MediaPriceId { get; set; }
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
    }
}
