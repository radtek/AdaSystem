using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Crawler.Models
{
    public class MediaCrawlerSet
    {

        [Display(Name = "微博点赞匹配")]
        public string BlogLikeReg { get; set; }
        [Display(Name = "微博转发匹配")]
        public string BlogRelayReg { get; set; }
        [Display(Name = "微博评论匹配")]
        public string BlogCommentReg { get; set; }
        [Display(Name = "小红书点赞匹配")]
        public string RedbookLikeReg { get; set; }
        [Display(Name = "小红书评论匹配")]
        public string RedbookCommentReg { get; set; }
        [Display(Name = "小红书收藏匹配")]
        public string RedbookCollectionReg { get; set; }
    }
}