using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Admin;

namespace Ada.Core.Domain.Wages
{
    /// <summary>
    /// 岗位
    /// </summary>
   public class Quarters: BaseEntity
    {
        public Quarters()
        {
            BaseSalary = 0;
            Allowance = 0;
            Commission = 1;
            Attendance = 0;
            Post = 0;
            Training = 0;
        }
        /// <summary>
        /// 岗位名称
        /// </summary>
        [Display(Name = "岗位名称")]
        public string Title { get; set; }
        /// <summary>
        /// 基本工资
        /// </summary>
        [Display(Name = "基本工资")]
        public decimal BaseSalary { get; set; }
        /// <summary>
        /// 岗位津贴
        /// </summary>
        [Display(Name = "岗位津贴")]
        public decimal Allowance { get; set; }
        /// <summary>
        /// 职务津贴
        /// </summary>
        [Display(Name = "职务津贴")]
        public decimal Post { get; set; }
        /// <summary>
        /// 销售提成系数
        /// </summary>
        [Display(Name = "销售提成系数")]
        public decimal Commission { get; set; }
        /// <summary>
        /// 全勤奖
        /// </summary>
        [Display(Name = "全勤奖")]
        public decimal Attendance { get; set; }
        /// <summary>
        /// 培训费用
        /// </summary>
        [Display(Name = "培训费用")]
        public decimal Training { get; set; }
    }
}
