using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.API.iDataAPI
{
    public class RedBookJSON : iDataJsonResult
    {
        public RedBookJSON()
        {
            data = new List<RedBookInfo>();
        }
        public List<RedBookInfo> data { get; set; }
    }
    public class RedBookArticleJSON : iDataJsonResult
    {
        public RedBookArticleJSON()
        {
            data = new List<RedBookArticleData>();
        }
        public List<RedBookArticleData> data { get; set; }
    }
    public class RedBookInfo
    {

        /// <summary>
        /// 主页地址
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 笔记数
        /// </summary>
        public int? postCount { get; set; }
        /// <summary>
        /// 专辑数
        /// </summary>
        public int? albumCount { get; set; }
        /// <summary>
        /// logo链接
        /// </summary>
        public string avatarUrl { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public string idGrade { get; set; }
        /// <summary>
        /// 赞与收藏
        /// </summary>
        public int? likeCount { get; set; }
        /// <summary>
        /// 粉丝数
        /// </summary>
        public int? fansCount { get; set; }
        /// <summary>
        /// 关注数
        /// </summary>
        public int? followCount { get; set; }
        /// <summary>
        /// 功能介绍
        /// </summary>
        public string biography { get; set; }
        /// <summary>
        /// 账号名称
        /// </summary>
        public string screenName { get; set; }
        /// <summary>
        /// 账号ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 地域
        /// </summary>
        public string location { get; set; }

    }

    public class RedBookArticleData
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
        /// 收藏数
        /// </summary>
        public int? favoriteCount { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int? likeCount { get; set; }
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
        /// 内容
        /// </summary>
        public string content { get; set; }
        

    }
}
