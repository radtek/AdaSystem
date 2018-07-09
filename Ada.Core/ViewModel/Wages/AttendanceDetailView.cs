using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Wages
{
  public  class AttendanceDetailView: BaseView
    {
        public AttendanceDetailView()
        {
            OffWork = 0;
            NoClockTimes = 0;
            LateTimes = 0;
            Absenteeism = 0;
            Overtime = 0;
            Commission = 0;
            Bonus = 0;
            DeductMoney = 0;
            TotalMoney = 0;
            TotalSum = 0;
        }
        /// <summary>
        /// 月份
        /// </summary>
        [Display(Name = "月份")]
        public DateTime? Date { get; set; }
        /// <summary>
        /// 请假天数
        /// </summary>
        [Display(Name = "请假天数")]
        public double OffWork { get; set; }
        /// <summary>
        /// 未打卡次数
        /// </summary>
        [Display(Name = "未打卡次数")]
        public int NoClockTimes { get; set; }
        /// <summary>
        /// 迟到次数
        /// </summary>
        [Display(Name = "迟到次数")]
        public int LateTimes { get; set; }
        /// <summary>
        /// 旷工天数
        /// </summary>
        [Display(Name = "旷工天数")]
        public double Absenteeism { get; set; }
        /// <summary>
        /// 加班时间
        /// </summary>
        [Display(Name = "加班时间")]
        public int Overtime { get; set; }
        /// <summary>
        /// 用户主键
        /// </summary>
        [Display(Name = "用户")]
        public string ManagerId { get; set; }
        /// <summary>
        /// 用户主键
        /// </summary>
        [Display(Name = "用户")]
        public string ManagerName { get; set; }
        /// <summary>
        /// 红包提成
        /// </summary>
        [Display(Name = "红包提成")]
        public decimal Commission { get; set; }
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
        /// 实发工资
        /// </summary>
        [Display(Name = "实发工资")]
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 实发总工资
        /// </summary>
        [Display(Name = "实发总工资")]
        public decimal TotalSum { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}
