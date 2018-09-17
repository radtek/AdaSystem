using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Content
{
    public class ArticleView : BaseView
    {
        /// <summary>
        /// 文章标题
        /// </summary>
        [Display(Name = "文章标题")]
        public string Title { get; set; }

        /// <summary>
        /// 所属栏目
        /// </summary>
        [Display(Name = "所属栏目")]
        public string ColumnId { get; set; }
        /// <summary>
        /// 所属栏目
        /// </summary>
        [Display(Name = "所属栏目")]
        public string ColumnName { get; set; }

        /// <summary>
        /// 跳转链接
        /// </summary>
        [Display(Name = "跳转链接")]
        public string Url { get; set; }

        /// <summary>
        /// 封面图片
        /// </summary>
        [Display(Name = "封面图片")]
        public string CoverPic { get; set; }

        /// <summary>
        /// 文章摘要
        /// </summary>
        [Display(Name = "文章摘要")]
        public string Summary { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        [Display(Name = "文章内容")]
        public string Content { get; set; }

        /// <summary>
        /// 文章作者
        /// </summary>
        [Display(Name = "文章作者")]
        public string Author { get; set; }

        /// <summary>
        /// 文章来源
        /// </summary>
        [Display(Name = "文章来源")]
        public string Source { get; set; }

        /// <summary>
        /// 浏览量
        /// </summary>
        [Display(Name = "浏览量")]
        public int? Click { get; set; }

        /// <summary>
        /// 文章状态
        /// </summary>
        [Display(Name = "文章状态")]
        public short? Status { get; set; }

        /// <summary>
        /// 允许评论
        /// </summary>
        [Display(Name = "允许评论")]
        public bool? IsComment { get; set; }

        /// <summary>
        /// 是否热门
        /// </summary>
        [Display(Name = "是否热门")]
        public bool? IsHot { get; set; }

        /// <summary>
        /// 是否置首
        /// </summary>
        [Display(Name = "是否置首")]
        public bool? IsTop { get; set; }

        /// <summary>
        /// 是否推荐
        /// </summary>
        [Display(Name = "是否推荐")]
        public bool? IsRecommend { get; set; }

        /// <summary>
        /// 是否轮播
        /// </summary>
        [Display(Name = "是否轮播")]
        public bool? IsSlide { get; set; }

        /// <summary>
        /// 是否推送
        /// </summary>
        [Display(Name = "是否推送")]
        public bool? IsPush { get; set; }

        /// <summary>
        /// 文章类型
        /// </summary>
        [Display(Name = "文章类型")]
        public string Type { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? Taxis { get; set; }
    }
}
