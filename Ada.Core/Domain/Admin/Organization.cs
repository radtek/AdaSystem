using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Admin
{
    public class Organization : BaseEntity
    {
        public Organization()
        {
            this.Managers = new HashSet<Manager>();
            this.Roles = new HashSet<Role>();
        }


        /// <summary>
        /// 上级部门
        /// </summary>
        [Display(Name = "上级机构")]
        public string ParentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Display(Name = "机构名称")]
        public string OrganizationName { get; set; }

        /// <summary>
        /// 部门主管ID
        /// </summary>
        [Display(Name = "机构主管")]
        public string MasterId { get; set; }

        /// <summary>
        /// 是否节点
        /// </summary>
        [Display(Name = "是否节点")]
        public bool? IsLeaf { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        [Display(Name = "级别")]
        public int? Level { get; set; }

        /// <summary>
        /// 树路径
        /// </summary>
        [Display(Name = "树路径")]
        public string TreePath { get; set; }

        /// <summary>
        /// 机构LOGO
        /// </summary>
        [Display(Name = "机构LOGO")]
        public string Logo { get; set; }
        /// <summary>
        /// 机构编号
        /// </summary>
        [Display(Name = "机构编号")]
        public string Number { get; set; }
        public virtual ICollection<Manager> Managers { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
