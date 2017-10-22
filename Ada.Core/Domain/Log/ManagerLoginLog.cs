using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Admin
{
   public class ManagerLoginLog:BaseEntity
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
        /// 用户
        /// </summary>
        [Display(Name = "用户")]
        public string ManagerId { get; set; }

        public virtual Manager Manager { get; set; }
    }
}
