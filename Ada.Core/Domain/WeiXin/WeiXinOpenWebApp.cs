using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.WeiXin
{
    public class WeiXinOpenWebApp : BaseEntity
    {
        
        /// <summary>
        /// 平台名称
        /// </summary>
        [Display(Name = "平台名称")]
        public string Name { get; set; }

        /// <summary>
        /// 开发ID
        /// </summary>
        [Display(Name = "开发平台ID")]
        public string AppId { get; set; }

        /// <summary>
        /// 开发密钥
        /// </summary>
        [Display(Name = "开发平台密钥")]
        public string AppSecret { get; set; }
        /// <summary>
        /// 登陆回调
        /// </summary>
        [Display(Name = "登陆回调")]
        public string LoginCallBackUrl { get; set; }

        /// <summary>
        /// 首页地址
        /// </summary>
        [Display(Name = "首页地址")]
        public string HomeUrl { get; set; }
        /// <summary>
        /// 绑定页地址
        /// </summary>
        [Display(Name = "绑定页地址")]
        public string BindAccountUrl { get; set; }
        /// <summary>
        /// 微信账户
        /// </summary>
        [Display(Name = "微信账户")]
        public string WeiXinAccountId { get; set; }


    }
}
