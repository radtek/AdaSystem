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
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Display(Name = "姓名")]
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
        /// 激活
        /// </summary>
        [Display(Name = "激活")]
        public short? Status { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        [Display(Name = "角色")]
        public List<string> RoleIds { get; set; }
        /// <summary>
        /// 机构组织
        /// </summary>
        [Display(Name = "机构组织")]
        public string OrganizationIds { get; set; }
        /// <summary>
        /// 特殊权限
        /// </summary>
        [Display(Name = "特殊权限")]
        public string ActionIds { get; set; }
        /// <summary>
        /// 所属角色
        /// </summary>
        [Display(Name = "所属角色")]
        public string Roles { get; set; }
        /// <summary>
        /// 所属机构
        /// </summary>
        [Display(Name = "所属机构")]
        public string Organizations { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public string AddDate { get; set; }
        /// <summary>
        /// 登陆密码
        /// </summary>
        [Display(Name = "登陆密码")]
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// 上次登陆
        /// </summary>
        [Display(Name = "上次登陆")]
        public string LastLoginDate { get; set; }
        /// <summary>
        /// 相片
        /// </summary>
        [Display(Name = "相片")]
        public string Image { get; set; }
        /// <summary>
        /// 角色集合
        /// </summary>
        [Display(Name = "角色集合")]
        public IEnumerable<RoleView> RoleList { get; set; }
        /// <summary>
        /// 权限集合
        /// </summary>
        [Display(Name = "权限集合")]
        public List<TreeView> ActionList { get; set; }
        /// <summary>
        /// 机构组织集合
        /// </summary>
        [Display(Name = "机构组织集合")]
        public List<TreeView> OrganizationList { get; set; }
        /// <summary>
        /// 当前角色
        /// </summary>
        [Display(Name = "当前角色")]
        public string RoleId { get; set; }
        /// <summary>
        /// 当前角色
        /// </summary>
        [Display(Name = "当前角色")]
        public string RoleName { get; set; }
    }
}
