using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.API
{
    public class APIInterfacesView : BaseView
    {
        /// <summary>
        /// API类型
        /// </summary>
        [Display(Name = "API类型")]
        public short? APIType { get; set; }
        /// <summary>
        /// API名称
        /// </summary>
        [Display(Name = "API名称")]
        public string APIName { get; set; }
        /// <summary>
        /// 调用别名
        /// </summary>
        [Display(Name = "调用别名")]
        public string CallIndex { get; set; }
        /// <summary>
        /// 域名https://120.76.205.241/post/weixinpro?pageToken=1&uid=rmrbwx&apikey=aHkIQg6KZL5nKgqhcAbrT7AYq484DkAfmFzd8rBgYDrK6CItsvAAOWwz7BiFkoQx
        /// </summary>
        [Display(Name = "域名")]
        public string APIUrl { get; set; }
        /// <summary>
        /// 请求方法
        /// </summary>
        [Display(Name = "请求方法")]
        public string HttpMethod { get; set; }
        /// <summary>
        /// 请求令牌
        /// </summary>
        [Display(Name = "请求令牌")]
        public string Token { get; set; }
        /// <summary>
        /// 公钥
        /// </summary>
        [Display(Name = "公钥")]
        public string AppId { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        [Display(Name = "密钥")]
        public string AppSecret { get; set; }
        /// <summary>
        /// 通用参数
        /// </summary>
        [Display(Name = "通用参数")]
        public string Parameters { get; set; }
        /// <summary>
        /// 请求速度(毫秒)
        /// </summary>
        [Display(Name = "请求速度")]
        public int? TimeOut { get; set; }
    }
}
