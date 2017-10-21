using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ada.Core.Tools
{
   public class Utils
    {
        #region 获取请求IP
        /// <summary>
        /// 获取请求IP
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress()
        {

            string userIP = "";

            try
            {
                if (HttpContext.Current == null)
                {
                    return userIP;
                }

                //CDN加速后取到的IP simone 090805
                var customerIp = HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(customerIp))
                {
                    return customerIp;
                }

                customerIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!String.IsNullOrEmpty(customerIp))
                {
                    return customerIp;
                }

                if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    customerIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    customerIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (String.Compare(customerIp, "unknown", StringComparison.OrdinalIgnoreCase) == 0 || String.IsNullOrEmpty(customerIp))
                {
                    return HttpContext.Current.Request.UserHostAddress;
                }
                return customerIp;
            }
            catch
            {
                // ignored
            }

            return userIP;

        } 
        #endregion
    }
}
