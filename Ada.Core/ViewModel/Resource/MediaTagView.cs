using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Resource
{
    public class MediaTagView : BaseView
    {
        /// <summary>
        /// 媒体标签
        /// </summary>
        [Display(Name = "媒体标签")]
        public string TagName { get; set; }
        /// <summary>
        /// 热门标签
        /// </summary>
        [Display(Name = "热门标签")]
        public bool? IsHot { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? Taxis { get; set; }
    }
}
