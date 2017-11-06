using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Admin
{
    public class FieldType : BaseEntity
    {
        public FieldType()
        {
            Fields = new HashSet<Field>();
        }
        /// <summary>
        /// 类别名称
        /// </summary>
        [Display(Name = "类别名称")]
        public string TypeName { get; set; }
        /// <summary>
        /// 调用别名
        /// </summary>
        [Display(Name = "调用别名")]
        public string CallIndex { get; set; }
        /// <summary>
        /// 父级类别
        /// </summary>
        [Display(Name = "父级类别")]
        public string ParentId { get; set; }
        public virtual ICollection<Field> Fields { get; set; }
    }
}
