using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Admin
{
  public class ManagerView: BaseView
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Display(Name = "真实姓名")]
        public string RealName { get; set; }

        /// <summary>
        /// 联系手机
        /// </summary>
        [Display(Name = "联系手机")]
        public string Phone { get; set; }

        /// <summary>
        /// 授权ID
        /// </summary>
        [Display(Name = "授权ID")]
        public string OpenId { get; set; }


        /// <summary>
        /// 用户状态
        /// </summary>
        [Display(Name = "用户状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        [Display(Name = "角色")]
        public string RoleId { get; set; }
        /// <summary>
        /// 机构组织
        /// </summary>
        [Display(Name = "机构组织")]
        public string OrganizationId { get; set; }
    }
}
