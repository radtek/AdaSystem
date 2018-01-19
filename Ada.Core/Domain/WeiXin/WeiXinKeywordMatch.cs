using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.WeiXin
{
    /// <summary>
    /// 关键字回复内容
    /// </summary>
   public class WeiXinKeywordMatch:BaseEntity
    {
        /// <summary>
        /// 标题名称
        /// </summary>
        [Display(Name = "标题名称")]
        public string Title { get; set; }

        /// <summary>
        /// 详情地址
        /// </summary>
        [Display(Name = "详情地址")]
        public string LinkUrl { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [Display(Name = "图片地址")]
        public string ImgUrl { get; set; }

        /// <summary>
        /// 媒体地址
        /// </summary>
        [Display(Name = "媒体地址")]
        public string MediaUrl { get; set; }

        /// <summary>
        /// 高清地址
        /// </summary>
        [Display(Name = "高清地址")]
        public string MediaHDUrl { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        [Display(Name = "回复内容")]
        public string Content { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        [Display(Name = "关键字")]
        public string WeiXinKeywordId { get; set; }
        public virtual WeiXinKeyword WeiXinKeyword { get; set; }
    }
}
