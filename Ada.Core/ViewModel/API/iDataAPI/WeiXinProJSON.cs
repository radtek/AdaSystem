using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.API.iDataAPI
{
   public class WeiXinProJSON:iDataJsonResult
    {
        public WeiXinProJSON()
        {
            data=new List<WeiXinArticleData>();
        }
        /// <summary>
        /// 文章集合
        /// </summary>
        public List<WeiXinArticleData> data { get; set; }
    }

    public class WeiXinArticleData
    {
        /// <summary>
        /// 发布时间：时间戳格式
        /// </summary>
        public long? publishDate { get; set; }
        /// <summary>
        /// 当次发布的第几篇内容
        /// </summary>
        public string idx { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int? likeCount { get; set; }
        /// <summary>
        /// 微信公众原始ID
        /// </summary>
        public string posterOriginId { get; set; }
        /// <summary>
        /// 图文消息ID
        /// </summary>
        public string mid { get; set; }
        /// <summary>
        /// 是否原创
        /// </summary>
        public bool? original { get; set; }
        /// <summary>
        /// 文章ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 微信公众平台唯一ID
        /// </summary>
        public string biz { get; set; }
        /// <summary>
        /// 阅读原文链接
        /// </summary>
        public string originUrl { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string @abstract { get; set; }
        /// <summary>
        /// 浏览数
        /// </summary>
        public int? viewCount { get; set; }
        /// <summary>
        /// 微信号名称
        /// </summary>
        public string posterScreenName { get; set; }
        /// <summary>
        /// 分享数
        /// </summary>
        public int? shareCount { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public int? commentCount { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>
        public string posterId { get; set; }
        /// <summary>
        /// 是否头条
        /// </summary>
        public bool? isTop { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 文章链接
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 发布时间：字符串格式
        /// </summary>
        public string publishDateStr { get; set; }
    }
}
