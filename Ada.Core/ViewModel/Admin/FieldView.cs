using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Admin
{
   public class FieldView:BaseView
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
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? Taxis { get; set; }
    }
}
