using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Resource
{
   public class MediaArticle:BaseEntity
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        [Display(Name = "文章ID")]
        public string ArticleId { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        [Display(Name = "文章标题")]
        public string Title { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        [Display(Name = "文章内容")]
        public string Content { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        [Display(Name = "发布时间")]
        public DateTime? PublishDate { get; set; }
        /// <summary>
        /// 原文链接
        /// </summary>
        [Display(Name = "原文链接")]
        public string OriginUrl { get; set; }
        /// <summary>
        /// 文章链接
        /// </summary>
        [Display(Name = "文章链接")]
        public string ArticleUrl { get; set; }
        /// <summary>
        /// 是否原创
        /// </summary>
        [Display(Name = "是否原创")]
        public bool? IsOriginal { get; set; }
        /// <summary>
        /// 是否头条
        /// </summary>
        [Display(Name = "是否头条")]
        public bool? IsTop { get; set; }
        /// <summary>
        /// 文章排序
        /// </summary>
        [Display(Name = "文章排序")]
        public string ArticleIdx { get; set; }
        /// <summary>
        /// 微信公众平台唯一ID
        /// </summary>
        [Display(Name = "微信公众平台唯一ID")]
        public string Biz { get; set; }
        /// <summary>
        /// 浏览数
        /// </summary>
        [Display(Name = "浏览数")]
        public int? ViewCount { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        [Display(Name = "评论数")]
        public int? CommentCount { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        [Display(Name = "点赞数")]
        public int? LikeCount { get; set; }
        /// <summary>
        /// 转发数
        /// </summary>
        [Display(Name = "转发数")]
        public int? ShareCount { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaId { get; set; }
        public virtual Media Media { get; set; }
    }
}
