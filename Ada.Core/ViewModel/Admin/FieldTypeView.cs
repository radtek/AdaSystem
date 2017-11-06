using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Admin
{
   public class FieldTypeView
    {
        /// <summary>
        /// 操作
        /// </summary>
        [Display(Name = "操作")]
        public string TypeId { get; set; }
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

    }
}
