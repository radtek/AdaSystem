using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Admin
{
   public class ManagerLoginLogView:BaseView
    {
        /// <summary>
        /// 登陆时间
        /// </summary>
        [Display(Name = "登陆时间")]
        public System.DateTime? LoginTime { get; set; }
        /// <summary>
        /// 登陆信息
        /// </summary>
        [Display(Name = "登陆信息")]
        public string WebInfo { get; set; }
        /// <summary>
        /// 登陆用户
        /// </summary>
        [Display(Name = "登陆用户")]
        public string UserName { get; set; }
        /// <summary>
        /// 登陆状态
        /// </summary>
        [Display(Name = "登陆状态")]
        public string Remark { get; set; }
        /// <summary>
        /// 登陆IP
        /// </summary>
        [Display(Name = "登陆IP")]
        public string Ip { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [Display(Name = "头像")]
        public string Image { get; set; }
    }
}
