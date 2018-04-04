using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.API.iDataAPI
{
   

    public class WeiXinInfosJSON : iDataJsonResult
    {
        public WeiXinInfosJSON()
        {
            data = new List<WeiXinInfo>();
        }
        /// <summary>
        /// 文章集合
        /// </summary>
        public List<WeiXinInfo> data { get; set; }
    }

    public class WeiXinInfo
    {
        /// <summary>
        /// 微信认证
        /// </summary>
        public string idVerifiedInfo { get; set; }
        /// <summary>
        /// 账号是否认证
        /// </summary>
        public bool? idVerified { get; set; }
        /// <summary>
        /// logo链接
        /// </summary>
        public string avatarUrl { get; set; }
        /// <summary>
        /// 二维码链接
        /// </summary>
        public string qrcodeUrl { get; set; }
        /// <summary>
        /// 月发文篇数
        /// </summary>
        public int? monthPostCount { get; set; }
        /// <summary>
        /// 功能介绍
        /// </summary>
        public string biography { get; set; }
        /// <summary>
        /// 账号名称
        /// </summary>
        public string screenName { get; set; }
        /// <summary>
        /// 最后推送信息
        /// </summary>
        public LastPost lastPost { get; set; }
    }

    public class LastPost
    {
        public string url { get; set; }
        public string date { get; set; }
    }


}
