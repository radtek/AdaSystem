using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.WeiXin
{
   public class WeiXinKeyword:BaseEntity
    {
        public WeiXinKeyword()
        {
            WeiXinKeywordMatchs=new HashSet<WeiXinKeywordMatch>();
        }
        /// <summary>
        /// 规则名称
        /// </summary>
        [Display(Name = "规则名称")]
        public string Name { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>
        [Display(Name = "关键词")]
        public string Keywords { get; set; }

        /// <summary>
        /// 是否模糊匹配
        /// </summary>
        [Display(Name = "是否模糊匹配")]
        public bool? IsLikeQuery { get; set; }

        /// <summary>
        /// 是否默认回复
        /// </summary>
        [Display(Name = "是否默认回复")]
        public bool? IsDefault { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        [Display(Name = "请求类型")]
        public short? RequestType { get; set; }

        /// <summary>
        /// 回复类型
        /// </summary>
        [Display(Name = "回复类型")]
        public short? ResponseType { get; set; }

        /// <summary>
        /// 公众号主键
        /// </summary>
        [Display(Name = "微信账号")]
        public string WeiXinAccountId { get; set; }
        public virtual WeiXinAccount WeiXinAccount { get; set; }
        public virtual ICollection<WeiXinKeywordMatch> WeiXinKeywordMatchs { get; set; }
    }
}
