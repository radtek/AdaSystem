using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Message.Models
{
    public class AliYunSet
    {
        public AliYunSet()
        {
            Product = "Dysmsapi";
            Domain = "dysmsapi.aliyuncs.com";
            Area = "cn-shenzhen";
        }
        /// <summary>
        /// 产品名称
        /// </summary>
        [Display(Name = "产品名称")]
        public string Product { get; set; }
        [Display(Name = "接口域名")]
        public string Domain { get; set; }
        [Display(Name = "开发KEY")]
        public string AccessKey { get; set; }
        [Display(Name = "开发密钥")]
        public string AccessKeySecret { get; set; }
        [Display(Name = "所属区域")]
        public string Area { get; set; }
        [Display(Name = "签名")]
        public string SignName { get; set; }
    }
}