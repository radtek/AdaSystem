using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Common
{
   public class Fans : BaseEntity
    {
        /// <summary>
        /// 昵称
        /// </summary>
        [Display(Name = "昵称")]
        public string NickName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [Display(Name = "类型")]
        public string Type { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [Display(Name = "头像")]
        public string Avatar { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        [Display(Name = "封面")]
        public string Cover { get; set; }
        /// <summary>
        /// 父级
        /// </summary>
        [Display(Name = "父级")]
        public string ParentId { get; set; }
        /// <summary>
        /// 头像范围
        /// </summary>
        [Display(Name = "头像范围")]
        public string AvatarRange { get; set; }
        /// <summary>
        /// 粉丝范围
        /// </summary>
        [Display(Name = "粉丝范围")]
        public string FansRange { get; set; }
    }
}
