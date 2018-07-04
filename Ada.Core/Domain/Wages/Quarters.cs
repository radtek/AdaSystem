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
            Commission = 0;
            Attendance = 0;
        }
        /// <summary>
        /// 岗位名称
        /// </summary>
        [Display(Name = "岗位名称")]
        public string Title { get; set; }
        /// <summary>
        /// 岗位工资
        /// </summary>
        [Display(Name = "岗位工资")]
        public decimal BaseSalary { get; set; }
        /// <summary>
        /// 岗位津贴
        /// </summary>
        [Display(Name = "岗位津贴")]
        public decimal Allowance { get; set; }
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

    }
}
