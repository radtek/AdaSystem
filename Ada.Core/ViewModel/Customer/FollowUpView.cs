using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Customer
{
   public class FollowUpView:BaseView
    {
        /// <summary>
        /// 跟进内容
        /// </summary>
        [Display(Name = "跟进内容")]
        public string Content { get; set; }
        /// <summary>
        /// 跟进方式
        /// </summary>
        [Display(Name = "跟进方式")]
        public string FollowUpWay { get; set; }
        /// <summary>
        /// 联系客户
        /// </summary>
        [Display(Name = "联系客户")]
        public string LinkManId { get; set; }
        /// <summary>
        /// 联系客户
        /// </summary>
        [Display(Name = "联系客户")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [Display(Name = "公司名称")]
        public string CompanyName { get; set; }
        /// <summary>
        /// 下次联系时间
        /// </summary>
        [Display(Name = "下次联系时间")]
        public DateTime? NextTime { get; set; }
        /// <summary>
        /// 经办人员
        /// </summary>
        [Display(Name = "经办人员")]
        public string Transactor { get; set; }
        /// <summary>
        /// 经办人员
        /// </summary>
        [Display(Name = "经办人员")]
        public string TransactorId { get; set; }
    }
}
