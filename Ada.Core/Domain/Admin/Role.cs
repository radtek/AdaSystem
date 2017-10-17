using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Admin
{
    public class Role : BaseEntity
    {
        public Role()
        {
            this.Managers = new HashSet<Manager>();
            this.Organizations = new HashSet<Organization>();
            this.Actions = new HashSet<Action>();
        }
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
        public virtual ICollection<Manager> Managers { get; set; }
        public virtual ICollection<Organization> Organizations { get; set; }
        public virtual ICollection<Action> Actions { get; set; }
    }
}
