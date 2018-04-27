using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

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

        #region 时间转换字符串
        public static string ToRead(DateTime src)
        {
            return ToRead(src as DateTime?);
        }
        public static string ToRead(DateTime? src)
        {
            string result = null;
            if (src.HasValue)
            {
                result = DateTimeToStr(src.Value);
            }
            return result;
        }
        private static string DateTimeToStr(DateTime src)
        {
            string result;

            long currentSecond = (long)(DateTime.Now - src).TotalSeconds;

            long minSecond = 60;                //60s = 1min  
            long hourSecond = minSecond * 60;   //60*60s = 1 hour  
            long daySecond = hourSecond * 24;   //60*60*24s = 1 day  
            long weekSecond = daySecond * 7;    //60*60*24*7s = 1 week  
            long monthSecond = daySecond * 30;  //60*60*24*30s = 1 month  
            long yearSecond = daySecond * 365;  //60*60*24*365s = 1 year  

            if (currentSecond >= yearSecond)
            {
                int year = (int)(currentSecond / yearSecond);
                result = $"{year}年前";
            }
            else if (currentSecond < yearSecond && currentSecond >= monthSecond)
            {
                int month = (int)(currentSecond / monthSecond);
                result = $"{month}个月前";
            }
            else if (currentSecond < monthSecond && currentSecond >= weekSecond)
            {
                int week = (int)(currentSecond / weekSecond);
                result = $"{week}周前";
            }
            else if (currentSecond < weekSecond && currentSecond >= daySecond)
            {
                int day = (int)(currentSecond / daySecond);
                result = $"{day}天前";
            }
            else if (currentSecond < daySecond && currentSecond >= hourSecond)
            {
                int hour = (int)(currentSecond / hourSecond);
                result = $"{hour}小时前";
            }
            else if (currentSecond < hourSecond && currentSecond >= minSecond)
            {
                int min = (int)(currentSecond / minSecond);
                result = $"{min}分钟前";
            }
            else if (currentSecond < minSecond && currentSecond >= 0)
            {
                result = "刚刚";
            }
            else
            {
                result = src.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return result;
        }
        #endregion

        #region 新浪微博
        /// <summary>
        /// 获取新浪微博ID
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetBlogId(string url)
        {
            string result = string.Empty;
            if (string.IsNullOrWhiteSpace(url))
            {
                return result;
            }
            Match match = Regex.Match(url, @"weibo.com/u/(\d+)", RegexOptions.ECMAScript);
            result = match.Groups[1].Value;
            if (string.IsNullOrWhiteSpace(result))
            {
                Match match2 = Regex.Match(url, @"www.weibo.com/u/(\d+)", RegexOptions.ECMAScript);
                result = match2.Groups[1].Value;
            }
            return result;

        }
        public static decimal ShowFansNum(int? fans, int baseNum = 10000)
        {
            if (!fans.HasValue) return 0;
            var temp = Convert.ToDecimal(fans);
            return Math.Round(temp / baseNum, 1);

        }
        public static int SetFansNum(decimal? fans,int baseNum=10000)
        {
            if (fans>baseNum)
            {
                return Convert.ToInt32(fans);
            }
            return fans.HasValue ? Convert.ToInt32(fans*baseNum) : 0;
        }
        public static string BlogAuthenticateType(int? verifiedtype)
        {
            if (verifiedtype != null)
            {
                string type = null;
                switch (verifiedtype)
                {
                    case 0:
                        type = "黄V";
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        type = "蓝V";
                        break;
                    case 200:
                    case 220:
                    case 400:
                        type = "达人";
                        break;
                }

                return type;
            }

            return null;
        }
        public static string BlogSex(string sex)
        {
            if (string.IsNullOrWhiteSpace(sex)) return null;
            string temp = null;
            switch (sex)
            {
                case "m":
                    temp = "男";
                    break;
                case "f":
                    temp = "女";
                    break;
            }
            return temp;

        }
        #endregion

        #region 获得当前绝对路径
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (string.IsNullOrWhiteSpace(strPath))
            {
                return "";
            }
            if (strPath.ToLower().StartsWith("http://"))
            {
                return strPath;
            }
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            return MapPath(strPath);
        }
        #endregion

        #region 虚拟路径转物理路径
        /// <summary>
        /// 虚拟路径转物理路径
        /// </summary>
        /// <param name="path">虚拟路径格式 "~/bin"</param>
        /// <returns>物理路径格式 E.g. "c:\inetpub\wwwroot\bin"</returns>
        public static string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }

            //not hosted. For example, run in unit tests
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }
        #endregion

        #region 截取字符长度
        /// <summary>
        /// 截取字符长度
        /// </summary>
        /// <param name="inputString">字符</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CutString(string inputString, int len)
        {
            if (string.IsNullOrEmpty(inputString))
                return "";
            inputString = DropHtml(inputString);
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }
            //如果截过则加上半个省略号 
            byte[] mybyte = System.Text.Encoding.Default.GetBytes(inputString);
            if (mybyte.Length > len)
                tempString += "…";
            return tempString;
        }
        #endregion

        #region 清除HTML标记
        public static string DropHtml(string htmlstring)
        {
            if (string.IsNullOrEmpty(htmlstring)) return "";
            //删除脚本  
            htmlstring = Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML  
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);

            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlstring.Replace("<", "");
            htmlstring.Replace(">", "");
            htmlstring.Replace("\r\n", "");
            htmlstring = HttpContext.Current.Server.HtmlEncode(htmlstring).Trim();
            return htmlstring;
        }
        #endregion

        /// <summary>
        /// 判断输入的字符串是否是一个合法的手机号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(string input)
        {
            Regex regex = new Regex("^[1][3,4,5,6,7,8,9][0-9]{9}$");
            return regex.IsMatch(input);

        }
        public static bool IsNum(string str)
        {
            return str.All(t => t >= '0' && t <= '9');
        }
    }


}
