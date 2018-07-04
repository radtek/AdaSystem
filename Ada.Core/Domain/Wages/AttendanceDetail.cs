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
    /// 考勤明细
    /// </summary>
    public class AttendanceDetail : BaseEntity
    {
        public AttendanceDetail()
        {
            OffWork = 0;
            NoClockTimes = 0;
            LateTimes = 0;
            Absenteeism = 0;
            Overtime = 0;

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

        public virtual Manager Manager { get; set; }
    }
}
