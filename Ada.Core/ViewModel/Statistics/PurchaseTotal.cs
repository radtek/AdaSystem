using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Statistics
{
  public  class PurchaseTotal
    {
        /// <summary>
        /// 待响应
        /// </summary>
        [Display(Name = "待响应")]
        public int? Waiting { get; set; }
        /// <summary>
        /// 正处理
        /// </summary>
        [Display(Name = "正处理")]
        public int? Doing { get; set; }
        /// <summary>
        /// 已确认
        /// </summary>
        [Display(Name = "已确认")]
        public int? Confirm { get; set; }
        /// <summary>
        /// 未到票数
        /// </summary>
        [Display(Name = "未到票数")]
        public int? InvoiceCount { get; set; }
        /// <summary>
        /// 今日预出刊
        /// </summary>
        [Display(Name = "今日出刊")]
        public int? Today { get; set; }
        /// <summary>
        /// 明日预出刊
        /// </summary>
        [Display(Name = "明日出刊")]
        public int? Tomorrow { get; set; }
        /// <summary>
        /// 预付金额
        /// </summary>
        [Display(Name = "预付金额")]
        public decimal? TotalPayMoney { get; set; }
        /// <summary>
        /// 已付金额
        /// </summary>
        [Display(Name = "已付金额")]
        public decimal? PayMoney { get; set; }
        /// <summary>
        /// 媒体资源
        /// </summary>
        [Display(Name = "媒体资源")]
        public string Medias { get; set; }
    }
}
