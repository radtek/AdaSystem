using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.WeiXin
{
   public class WeiXinAccountView: BaseView
    {
        /// <summary>
        /// 账号名称
        /// </summary>
        [Display(Name = "账号名称")]
        public string Name { get; set; }
        /// <summary>
        /// 原始ID
        /// </summary>
        [Display(Name = "原始ID")]
        public string SourceId { get; set; }
        /// <summary>
        /// 账号类型 0 订阅号 1 服务号 2小程序 3企业号
        /// </summary>
        [Display(Name = "账号类型")]
        public short? AccountType { get; set; }

        /// <summary>
        /// 开发ID
        /// </summary>
        [Display(Name = "开发ID")]
        public string AppId { get; set; }

        /// <summary>
        /// 开发密钥
        /// </summary>
        [Display(Name = "开发密钥")]
        public string AppSecret { get; set; }
        /// <summary>
        /// 账号状态
        /// </summary>
        [Display(Name = "账号状态")]
        public bool? Status { get; set; }
        /// <summary>
        /// 授权令牌
        /// </summary>
        [Display(Name = "授权令牌")]
        public string Token { get; set; }

        /// <summary>
        /// 加密字符串
        /// </summary>
        [Display(Name = "加密字符串")]
        public string EncodingAESKey { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Display(Name = "商户号")]
        public string MchId { get; set; }

        /// <summary>
        /// 商户密钥
        /// </summary>
        [Display(Name = "商户密钥")]
        public string MchKey { get; set; }
        /// <summary>
        /// 回调地址
        /// </summary>
        [Display(Name = "回调地址")]
        public string NotifyUrl { get; set; }
        /// <summary>
        /// 授权证书
        /// </summary>
        [Display(Name = "授权证书")]
        public string CretPath { get; set; }
        /// <summary>
        /// 微信Logo
        /// </summary>
        [Display(Name = "微信Logo")]
        public string Image { get; set; }
        /// <summary>
        /// 授权地址
        /// </summary>
        [Display(Name = "授权地址")]
        public string Url { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}
