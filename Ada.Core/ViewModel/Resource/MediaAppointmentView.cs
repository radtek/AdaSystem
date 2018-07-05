using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Resource
{
   public class MediaAppointmentView : BaseView
    {

        /// <summary>
        /// 预约人
        /// </summary>
        [Display(Name = "预约人")]
        public string Transactor { get; set; }
        /// <summary>
        /// 预约人
        /// </summary>
        [Display(Name = "预约人")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 预约状态
        /// </summary>
        [Display(Name = "预约状态")]
        public short? State { get; set; }
        /// <summary>
        /// 预约时间
        /// </summary>
        [Display(Name = "预约时间")]
        public DateTime? AppointmentDate { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaId { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaName { get; set; }
        /// <summary>
        /// 预约位置 1 头条 2 次条
        /// </summary>
        [Display(Name = "预约位置")]
        public int? Taxis { get; set; }
        /// <summary>
        /// 预约时间
        /// </summary>
        [Display(Name = "预约时间")]
        public string AppointmentDateRange { get; set; }
        /// <summary>
        /// 预约备注
        /// </summary>
        [Display(Name = "预约备注")]
        public string Remark { get; set; }
    }
}
