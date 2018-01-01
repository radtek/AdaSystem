using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Setting
{
    /// <summary>
    /// 站点配置
    /// </summary>
    public class Site
    {
        public Site()
        {
            SiteStatus = true;
        }
        /// <summary>
        /// 网站名称
        /// </summary>
        [Display(Name = "网站名称")]
        public string SiteName { get; set; }
        /// <summary>
        /// 网站域名
        /// </summary>
        [Display(Name = "网站域名")]
        public string Domain { get; set; }
        /// <summary>
        /// 网站备案
        /// </summary>
        [Display(Name = "网站备案")]
        public string SiteICP { get; set; }
        /// <summary>
        /// 网站状态
        /// </summary>
        [Display(Name = "网站状态")]
        public bool SiteStatus { get; set; }
        /// <summary>
        /// 网站LOGO
        /// </summary>
        [Display(Name = "网站LOGO")]
        public string SiteLogo { get; set; }
        /// <summary>
        /// 网站关闭原因
        /// </summary>
        [Display(Name = "网站关闭原因")]
        public string SiteCloseReson { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [Display(Name = "公司名称")]
        public string CompanyName { get; set; }
        /// <summary>
        /// 公司LOGO
        /// </summary>
        [Display(Name = "公司LOGO")]
        public string CompanyLogo { get; set; }
        /// <summary>
        /// 公司地址
        /// </summary>
        [Display(Name = "公司地址")]
        public string CompanyAddress { get; set; }
        /// <summary>
        /// 公司电话
        /// </summary>
        [Display(Name = "公司电话")]
        public string CompanyPhone { get; set; }
        /// <summary>
        /// 公司邮箱
        /// </summary>
        [Display(Name = "公司邮箱")]
        public string CompanyEmail { get; set; }
        
    }
}
