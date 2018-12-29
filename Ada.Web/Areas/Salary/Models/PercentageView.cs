using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Salary.Models
{
    public class PercentageView
    {
        public PercentageView()
        {
            Percentage = 0;
            MediaTypeIds=new List<string>();
            TransactorIds=new List<string>();
        }
        /// <summary>
        /// 方案名称
        /// </summary>
        [Display(Name = "方案名称")]
        public string Title { get; set; }
        /// <summary>
        /// 出刊日期
        /// </summary>
        [Display(Name = "出刊日期")]
        public string DateRange { get; set; }
        /// <summary>
        /// 媒体类型
        /// </summary>
        [Display(Name = "媒体类型")]
        public List<string> MediaTypeIds { get; set; }
        /// <summary>
        /// 销售第一
        /// </summary>
        [Display(Name = "销售第一")]
        public List<string> TransactorIds { get; set; }
        /// <summary>
        /// 提成系数
        /// </summary>
        [Display(Name = "提成系数")]
        public decimal Percentage { get; set; }
    }
}