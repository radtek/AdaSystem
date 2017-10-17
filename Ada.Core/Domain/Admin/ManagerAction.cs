using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Admin
{
    public class ManagerAction : BaseEntity
    {
        /// <summary>
        /// 是否允许
        /// </summary>
        [Display(Name = "是否允许")]
        public bool IsPass { get; set; }

        /// <summary>
        /// 用户主键
        /// </summary>
        [Display(Name = "用户")]
        public string ManagerId { get; set; }

        /// <summary>
        /// 权限主键
        /// </summary>
        [Display(Name = "权限")]
        public string ActionInfoId { get; set; }

        public virtual Manager Manager { get; set; }

        public virtual Action Action { get; set; }
    }
}
