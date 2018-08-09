using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Crawler.Models
{
    public class ProxyIp
    {
        public ConcurrentQueue<string> ProxyIpQueue { get; set; }

        #region 生产IP 代理 对象
        private void Grab_ProxyIp(ConcurrentQueue<string> proxyIpQueue)
        {
            HashSet<string> proxyIp = new HashSet<string>();
            int count = 0;
            var xcUrl = "http://www.xicidaili.com/nn/1"; // 西刺
            RetryFunc(() =>
            {
                var result= Ada.Core.Tools.HttpUtility.Get(xcUrl);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    string regex = @"<td>(\d+\.\d+\.\d+\.\d+)</td>\s+<td>(\d+)</td>";
                    Match mstr = Regex.Match(result, regex);
                    while (mstr.Success && count < 20)
                    {
                        proxyIp.Add(mstr.Groups[1].Value + ":" + mstr.Groups[2].Value);
                        mstr = mstr.NextMatch();
                        count++;
                    }

                    return true;
                }

                return false;
            }, 10);

            count = 0;
            var ip84Url = "http://ip84.com/dlgn"; // IP巴士
            RetryFunc(() =>
            {
                var result = Ada.Core.Tools.HttpUtility.Get(ip84Url);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    string regex = @"<td>(\d+\.\d+\.\d+\.\d+)</td>\s+<td>(\d+)</td>";
                    Match mstr = Regex.Match(result, regex);
                    while (mstr.Success && count < 10)
                    {
                        proxyIp.Add(mstr.Groups[1].Value + ":" + mstr.Groups[2].Value);
                        mstr = mstr.NextMatch();
                        count++;
                    }

                    return true;
                }

                return false;
            }, 10);

            count = 0;
            var ip3366Url = "http://www.ip3366.net/free/?stype=1"; // 云代理
            RetryFunc(() =>
            {
                var result = Ada.Core.Tools.HttpUtility.Get(ip3366Url);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    string regex = @"<td>(\d+\.\d+\.\d+\.\d+)</td>\s+<td>(\d+)</td>";
                    Match mstr = Regex.Match(result, regex);
                    while (mstr.Success && count < 10)
                    {
                        proxyIp.Add(mstr.Groups[1].Value + ":" + mstr.Groups[2].Value);
                        mstr = mstr.NextMatch();
                        count++;
                    }

                    return true;
                }

                return false;
            }, 10);

            count = 0;
            var ipHaiUrl = "http://www.iphai.com/free/ng"; // IP海
            RetryFunc(() =>
            {
                var result = Ada.Core.Tools.HttpUtility.Get(ipHaiUrl);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    string regex = @"<td>\s+(\d+\.\d+\.\d+\.\d+)\s+</td>\s+<td>\s+(\d+)\s+</td>";
                    Match mstr = Regex.Match(result, regex);
                    while (mstr.Success && count < 10)
                    {
                        proxyIp.Add(mstr.Groups[1].Value + ":" + mstr.Groups[2].Value);
                        mstr = mstr.NextMatch();
                        count++;
                    }

                    return true;
                }

                return false;
            }, 10);

            count = 0;
            var _66ipUrl = "http://www.66ip.cn/nmtq.php?getnum=10&isp=0&anonymoustype=3&start=&ports=&export=&ipaddress=&area=1&proxytype=2&api=66ip"; // 66ip
            RetryFunc(() =>
            {
                var result = Ada.Core.Tools.HttpUtility.Get(_66ipUrl);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    string regex = @"(\d+\.\d+\.\d+\.\d+):(\d+)<br/>";
                    Match mstr = Regex.Match(result, regex);
                    while (mstr.Success && count < 10)
                    {
                        proxyIp.Add(mstr.Groups[1].Value + ":" + mstr.Groups[2].Value);
                        mstr = mstr.NextMatch();
                        count++;
                    }

                    return true;
                }

                return false;
            }, 10);

            foreach (var item in proxyIp)
            {
                proxyIpQueue.Enqueue(item);
            }
        }
        #endregion

        private bool RetryFunc(Func<bool> func, int retryCount)
        {
            bool result;
            do
            {
                try
                {
                    result = func();
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            while (!result && retryCount-- > 0);

            return result;
        }
    }
}