using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Salary.Models
{
    public class SalarySet
    {
        public SalarySet()
        {
            NoClock = 20;
            Late = 20;
            OffWork = 50;
            Absenteeism = 100;
            Derate = 1;
        }
        /// <summary>
        /// 未打卡扣款（元/次）
        /// </summary>
        [Display(Name = "未打卡扣款（元/次）")]
        public decimal NoClock { get; set; }
        /// <summary>
        /// 迟到扣款（元/次）
        /// </summary>
        [Display(Name = "迟到扣款（元/次）")]
        public decimal Late { get; set; }
        /// <summary>
        /// 请假扣款（元/半天）
        /// </summary>
        [Display(Name = "请假扣款（元/半天）")]
        public decimal OffWork { get; set; }
        /// <summary>
        /// 旷工扣款（元/半天）
        /// </summary>
        [Display(Name = "旷工扣款（元/半天）")]
        public decimal Absenteeism { get; set; }
        /// <summary>
        /// 未打卡/迟到减免次数
        /// </summary>
        [Display(Name = "未打卡/迟到减免次数")]
        public int Derate { get; set; }
    }
}