using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Admin
{
   public class RoleView
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Display(Name = "角色名称")]
        public string RoleName { get; set; }

        /// <summary>
        /// 角色类型
        /// </summary>
        [Display(Name = "角色类型")]
        public string RoleType { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        [Display(Name = "操作")]
        public string Id { get; set; }
        /// <summary>
        /// 权限集合
        /// </summary>
        [Display(Name = "权限集合")]
        public string ActionIds { get; set; }
    }
}
