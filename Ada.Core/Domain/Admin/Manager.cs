using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Log;
using Newtonsoft.Json;

namespace Ada.Core.Domain.Admin
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class Manager: BaseEntity
    {
        public Manager()
        {
            this.Organizations = new HashSet<Organization>();
            this.Roles = new HashSet<Role>();
            this.ManagerActions = new HashSet<ManagerAction>();
            this.ManagerLoginLogs = new HashSet<ManagerLoginLog>();
        }
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
        /// 登陆密码
        /// </summary>
        [Display(Name = "登陆密码")]
        public string Password { get; set; }

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
        /// 用户头像
        /// </summary>
        [Display(Name = "用户头像")]
        public string Image { get; set; }

        /// <summary>
        /// 用户主题
        /// </summary>
        [Display(Name = "用户主题")]
        public string Theme { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [Display(Name = "过期时间")]
        public System.DateTime? ExpireTime { get; set; }
        /// <summary>
        /// 机构组织
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Organization> Organizations { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Role> Roles { get; set; }
        /// <summary>
        /// 权限
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<ManagerAction> ManagerActions { get; set; }
        /// <summary>
        /// 权限
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<ManagerLoginLog> ManagerLoginLogs { get; set; }
    }
}
