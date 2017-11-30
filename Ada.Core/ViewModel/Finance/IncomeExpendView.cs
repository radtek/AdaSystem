using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Finance
{
  public  class IncomeExpendView : BaseView
    {
        /// <summary>
        /// 科目名称
        /// </summary>
        [Display(Name = "科目名称")]
        [StringLength(32, ErrorMessage = "字符长度不能超过32")]
        public string SubjectName { get; set; }
        /// <summary>
        /// 科目编号
        /// </summary>
        [Display(Name = "科目编号")]
        [StringLength(32, ErrorMessage = "字符长度不能超过32")]
        public string SubjectNum { get; set; }
        /// <summary>
        /// 科目类型  1 收入  0 支出
        /// </summary>
        [Display(Name = "科目类型")]
        [Required]
        public short? SubjectType { get; set; }
        /// <summary>
        /// 是否主营项目
        /// </summary>
        [Display(Name = "是否主营项目")]
        [Required]
        public bool? IsMain { get; set; }
        /// <summary>
        /// 父级类别
        /// </summary>
        [Display(Name = "上级科目")]
        [StringLength(32, ErrorMessage = "字符长度不能超过32")]
        public string ParentId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        [Required]
        public int? Taxis { get; set; }
    }
}
