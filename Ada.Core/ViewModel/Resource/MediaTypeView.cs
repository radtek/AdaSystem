using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Resource
{
    public class MediaTypeView : BaseView
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        [Display(Name = "类型名称")]
        public string TypeName { get; set; }
        /// <summary>
        /// 调用别名
        /// </summary>
        [Display(Name = "调用别名")]
        public string CallIndex { get; set; }
        /// <summary>
        /// 父级类型
        /// </summary>
        [Display(Name = "父级类型")]
        public string ParentId { get; set; }
        /// <summary>
        /// 封面图片
        /// </summary>
        [Display(Name = "封面图片")]
        public string Image { get; set; }
        /// <summary>
        /// 是否评论
        /// </summary>
        [Display(Name = "是否评论")]
        public bool? IsComment { get; set; }
        /// <summary>
        /// 广告位
        /// </summary>
        [Display(Name = "广告位")]
        public List<string> AdPositions { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? Taxis { get; set; }
    }
}
