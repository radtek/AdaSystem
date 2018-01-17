using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Business
{
   public class BusinessTotal
    {
        public BusinessTotal()
        {
            Business1=new List<BusinessOrderDetailView>();
            Business2=new List<BusinessOrderDetailView>();
        }
        /// <summary>
        /// 待转单
        /// </summary>
        [Display(Name = "待转单")]
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
        /// 已完成
        /// </summary>
        [Display(Name = "已完成")]
        public int? Done { get; set; }
        /// <summary>
        /// 今日预出刊
        /// </summary>
        [Display(Name = "今日预出刊")]
        public int? Today { get; set; }
        /// <summary>
        /// 明日预出刊
        /// </summary>
        [Display(Name = "明日预出刊")]
        public int? Tomorrow { get; set; }

        /// <summary>
        /// 销售一部业绩
        /// </summary>
        [Display(Name = "销售一部业绩")]
        public List<BusinessOrderDetailView> Business1 { get; set; }
        /// <summary>
        /// 销售二部业绩
        /// </summary>
        [Display(Name = "销售二部业绩")]
        public List<BusinessOrderDetailView> Business2 { get; set; }

    }
}
