using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.API.iDataAPI
{
    public class DouYinJSON : iDataJsonResult
    {
        public DouYinJSON()
        {
            data = new List<DouYinInfo>();
        }
        public List<DouYinInfo> data { get; set; }
    }
    public class DouYinArticleJSON : iDataJsonResult
    {
        public DouYinArticleJSON()
        {
            data = new List<DouYinArticleData>();
        }
        public List<DouYinArticleData> data { get; set; }
    }
    public class DouYinInfo
    {

        /// <summary>
        /// 主页地址
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 账号是否认证
        /// </summary>
        public bool? idVerified { get; set; }
        /// <summary>
        /// logo链接
        /// </summary>
        public string avatarUrl { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string gender { get; set; }
        /// <summary>
        /// 喜欢数
        /// </summary>
        public int? favoriteCount { get; set; }
        /// <summary>
        /// 粉丝数
        /// </summary>
        public int? fansCount { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int? coinCount { get; set; }
        /// <summary>
        /// 关注数
        /// </summary>
        public int? followCount { get; set; }
        /// <summary>
        /// 发布总数
        /// </summary>
        public int? videoCount { get; set; }
        /// <summary>
        /// 功能介绍
        /// </summary>
        public string biography { get; set; }
        /// <summary>
        /// 抖音ID
        /// </summary>
        public string douyinID { get; set; }
        /// <summary>
        /// 账号名称
        /// </summary>
        public string screenName { get; set; }
        /// <summary>
        /// 账号ID
        /// </summary>
        public string id { get; set; }

    }

    public class DouYinArticleData
    {
        /// <summary>
        /// 发布时间：时间戳格式
        /// </summary>
        public long? publishDate { get; set; }
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
        /// 描述
        /// </summary>
        public string description { get; set; }
        

    }
}
