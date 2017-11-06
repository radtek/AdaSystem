using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Admin
{
    public class Field : BaseEntity
    {
        /// <summary>
        /// 字典名称
        /// </summary>
        [Display(Name = "字典名称")]
        public string Text { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>
        [Display(Name = "字典值")]
        public string Value { get; set; }
        /// <summary>
        /// 字典类别
        /// </summary>
        [Display(Name = "字典类别")]
        public string FieldTypeId { get; set; }
        public virtual FieldType FieldType { get; set; }

    }
}
