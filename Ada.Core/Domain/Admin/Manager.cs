using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Log;
using Ada.Core.Domain.Wages;
using Newtonsoft.Json;

namespace Ada.Core.Domain.Admin
{
    public class Manager: BaseEntity
    {
        public Manager()
        {
            this.Organizations = new HashSet<Organization>();
            this.Roles = new HashSet<Role>();
            this.ManagerActions = new HashSet<ManagerAction>();
            this.ManagerLoginLogs = new HashSet<ManagerLoginLog>();
            SalaryDetails = new HashSet<SalaryDetail>();
            AttendanceDetails= new HashSet<AttendanceDetail>();
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
        /// 开发平台授权ID
        /// </summary>
        [Display(Name = "开发平台授权ID")]
        public string UnionId { get; set; }

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
        /// 生日
        /// </summary>
        [Display(Name = "生日")]
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 是否农历
        /// </summary>
        [Display(Name = "是否农历")]
        public bool? IsLunar { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        [Display(Name = "身份证")]
        public string IdCard { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>
        [Display(Name = "入职日期")]
        public DateTime? EntryDate { get; set; }
        /// <summary>
        /// 离职日期
        /// </summary>
        [Display(Name = "离职日期")]
        public DateTime? QuitDate { get; set; }
        /// <summary>
        /// 转正日期
        /// </summary>
        [Display(Name = "转正日期")]
        public DateTime? PromotionDate { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        [Display(Name = "过期时间")]
        public System.DateTime? ExpireTime { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        [Display(Name = "开户行")]
        public string BankName { get; set; }
        /// <summary>
        /// 开户号
        /// </summary>
        [Display(Name = "开户号")]
        public string BankNum { get; set; }
        /// <summary>
        /// 开户名
        /// </summary>
        [Display(Name = "开户名")]
        public string BankAccount { get; set; }
        /// <summary>
        /// 岗位
        /// </summary>
        [Display(Name = "岗位")]
        public string QuartersId { get; set; }

        public virtual ICollection<Organization> Organizations { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<ManagerAction> ManagerActions { get; set; }
        public virtual ICollection<ManagerLoginLog> ManagerLoginLogs { get; set; }

        public virtual ICollection<SalaryDetail> SalaryDetails { get; set; }
        public virtual ICollection<AttendanceDetail> AttendanceDetails { get; set; }
    }
}
