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
    /// 工资明细
    /// </summary>
    public class SalaryDetail : BaseEntity
    {
        public SalaryDetail()
        {

            Total = 0;
            Commission = 0;
            Bonus = 0;
            DeductMoney = 0;
            SaleCommission = 0;
            AttendanceTotal = 0;
        }
        /// <summary>
        /// 工资月份
        /// </summary>
        [Display(Name = "工资月份")]
        public DateTime? Date { get; set; }
        /// <summary>
        /// 实发工资
        /// </summary>
        [Display(Name = "实发工资")]
        public decimal Total { get; set; }
        /// <summary>
        /// 红包提成
        /// </summary>
        [Display(Name = "红包提成")]
        public decimal Commission { get; set; }
        /// <summary>
        /// 销售提成
        /// </summary>
        [Display(Name = "销售提成")]
        public decimal SaleCommission { get; set; }
        /// <summary>
        /// 其他奖金
        /// </summary>
        [Display(Name = "其他奖金")]
        public decimal Bonus { get; set; }
        /// <summary>
        /// 其他扣款
        /// </summary>
        [Display(Name = "其他扣款")]
        public decimal DeductMoney { get; set; }
        /// <summary>
        /// 考勤合计
        /// </summary>
        [Display(Name = "考勤合计")]
        public decimal AttendanceTotal { get; set; }
        /// <summary>
        /// 用户主键
        /// </summary>
        [Display(Name = "用户")]
        public string ManagerId { get; set; }

        public virtual Manager Manager { get; set; }
    }
}
