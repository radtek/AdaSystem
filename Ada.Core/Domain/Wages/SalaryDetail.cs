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
            Endowment = 0;
            Health = 0;
            Childbirth = 0;
            Unemployment = 0;
            Tax = 0;
            HousingFund = 0;
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
        /// 养老保险
        /// </summary>
        [Display(Name = "养老保险")]
        public decimal Endowment { get; set; }
        /// <summary>
        /// 医疗保险
        /// </summary>
        [Display(Name = "医疗保险")]
        public decimal Health { get; set; }
        /// <summary>
        /// 工伤保险
        /// </summary>
        [Display(Name = "工伤保险")]
        public decimal Injury { get; set; }
        /// <summary>
        /// 生育保险
        /// </summary>
        [Display(Name = "生育保险")]
        public decimal Childbirth { get; set; }
        /// <summary>
        /// 失业保险
        /// </summary>
        [Display(Name = "失业保险")]
        public decimal Unemployment { get; set; }
        /// <summary>
        /// 住房公积金
        /// </summary>
        [Display(Name = "住房公积金")]
        public decimal HousingFund { get; set; }
        /// <summary>
        /// 个人所得税
        /// </summary>
        [Display(Name = "个人所得税")]
        public decimal Tax { get; set; }
        /// <summary>
        /// 用户主键
        /// </summary>
        [Display(Name = "用户")]
        public string ManagerId { get; set; }

        public virtual Manager Manager { get; set; }
    }
}
