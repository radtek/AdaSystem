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
        /// 登陆信息
        /// </summary>
        [Display(Name = "登陆信息")]
        public string Content { get; set; }
        /// <summary>
        /// 登陆结果
        /// </summary>
        [Display(Name = "登陆结果")]
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
        /// 登陆时间
        /// </summary>
        [Display(Name = "登陆时间")]
        public DateTime? NextTime { get; set; }
        /// <summary>
        /// 登陆人员
        /// </summary>
        [Display(Name = "登陆人员")]
        public string Transactor { get; set; }
        /// <summary>
        /// 登陆人员
        /// </summary>
        [Display(Name = "登陆人员")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 登陆IP
        /// </summary>
        [Display(Name = "登陆IP")]
        public string IpAddress { get; set; }
    }
}
