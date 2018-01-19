using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.WeiXin
{
    /// <summary>
    /// 请求回复记录
    /// </summary>
   public class WeiXinRequestReocrd:BaseEntity
    {
        /// <summary>
        /// 发送人（在 Request 中为OpenId，在 Response 中为公众号的微信号）
        /// </summary>
        [Display(Name = "发送人")]
        public string FromUserName { get; set; }
        /// <summary>
        /// 接收人（在 Request 中为公众号的微信号，在 Response 中为 OpenId）
        /// </summary>
        [Display(Name = "接收人")]
        public string ToUserName { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        [Display(Name = "请求类型")]
        public string RequestType { get; set; }

        /// <summary>
        /// 请求内容
        /// </summary>
        [Display(Name = "请求内容")]
        public string RequestContent { get; set; }
        /// <summary>
        /// 回复类型
        /// </summary>
        [Display(Name = "回复类型")]
        public string ReponseType { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        [Display(Name = "回复内容")]
        public string ReponseContent { get; set; }
        /// <summary>
        /// 请求时间
        /// </summary>
        [Display(Name = "请求时间")]
        public DateTime? RequestTime { get; set; }
        /// <summary>
        /// 微信账号
        /// </summary>
        [Display(Name = "微信账号")]
        public string WeiXinAccountId { get; set; }


        public virtual WeiXinAccount WeiXinAccount { get; set; }
    }
}
