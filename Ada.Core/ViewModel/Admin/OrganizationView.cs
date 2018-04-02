using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Admin
{
   public class OrganizationView
    {
        /// <summary>
        /// 机构名称
        /// </summary>
        [Display(Name = "机构名称")]
        public string OrganizationName { get; set; }
        /// <summary>
        /// 上级机构
        /// </summary>
        [Display(Name = "上级机构")]
        public string ParentId { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        [Display(Name = "主键")]
        public string Id { get; set; }
        /// <summary>
        /// 树路径
        /// </summary>
        [Display(Name = "树路径")]
        public string TreePath { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? Taxis { get; set; }
    }
}
