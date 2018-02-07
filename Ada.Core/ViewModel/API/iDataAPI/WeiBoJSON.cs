using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.API.iDataAPI
{
   public class WeiBoJSON: iDataJsonResult
    {
        public WeiBoJSON()
        {
            data=new List<WeiboArticleData>();
        }
        /// <summary>
        /// 文章集合
        /// </summary>
        public List<WeiboArticleData> data { get; set; }
    }

    public class WeiboArticleData
    {
        /// <summary>
        /// 发布时间：时间戳格式
        /// </summary>
        public long? publishDate { get; set; }
        /// <summary>
        /// 发布时间 
        /// </summary>
        public string pDate { get; set; }
        /// <summary>
        /// 发布时间 
        /// </summary>
        public string publishDateStr { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public int? commentCount { get; set; }
        /// <summary>
        /// 浏览数
        /// </summary>
        public int? viewCount { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int? likeCount { get; set; }
        /// <summary>
        /// 分享数
        /// </summary>
        public int? shareCount { get; set; }
        /// <summary>
        /// 文章链接
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 文章ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 微博信息
        /// </summary>
        public WeiboInfo from { get; set; }

    }

    public class WeiboInfo
    {
        /// <summary>
        /// 微博名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 微博描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 微博数
        /// </summary>
        public int? postCount { get; set; }
        /// <summary>
        /// 粉丝数
        /// </summary>
        public int? fansCount { get; set; }
        /// <summary>
        /// 关注数
        /// </summary>
        public int? friendCount { get; set; }
        /// <summary>
        /// 微博首页
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 微博UID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 更多微博信息
        /// </summary>
        public WeiboInfoExtend extend { get; set; }
    }

    public class WeiboInfoExtend
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string avatar_large { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        public bool? verified { get; set; }
        /// <summary>
        /// 用户所在地
        /// </summary>
        public string location { get; set; }
        /// <summary>
        /// 高清头像
        /// </summary>
        public string avatar_hd { get; set; }
        /// <summary>
        /// 性别，m：男、f：女、n：未知
        /// </summary>
        public string gender { get; set; }
        /// <summary>
        /// 认证级别
        /// </summary>
        public int? verified_type { get; set; }
    }
}
