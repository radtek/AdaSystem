using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ada.Core.Tools
{
    public static class HttpUtility
    {
        public static async Task<string> GetAsync(string url)
        {
            //创建HttpClient（注意传入HttpClientHandler）
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var http = new HttpClient(handler))
            {
                http.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                //await异步等待回应
                var response = await http.GetAsync(url);
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                return await response.Content.ReadAsStringAsync();
            }
        }
        public static string Get(string url)
        {
            //创建HttpClient（注意传入HttpClientHandler）
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var http = new HttpClient(handler))
            {
                http.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                var task = http.GetAsync(url);
                task.Result.EnsureSuccessStatusCode();
                return task.Result.Content.ReadAsStringAsync().Result;
            }
        }
        public static T GetJson<T>(string url)
        {
            string resultStr = Get(url);
            if (string.IsNullOrWhiteSpace(resultStr))
            {
                throw new ApplicationException("转换JSON失败，字符串信息为空");
            }
            return JsonConvert.DeserializeObject<T>(resultStr);
        }
        public static async Task<T> GetJsonAsync<T>(string url)
        {
            string resultStr = await GetAsync(url);
            return JsonConvert.DeserializeObject<T>(resultStr);
        }
        public static string Post(string url, IEnumerable<KeyValuePair<string, string>> postData = null)
        {
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            //创建HttpClient（注意传入HttpClientHandler）
            using (var http = new HttpClient(handler))
            {
                http.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                //使用FormUrlEncodedContent做HttpContent
                var content = postData == null ? null : new FormUrlEncodedContent(postData);
                //await异步等待回应
                var response = http.PostAsync(url, content);
                //确保HTTP成功状态值
                response.Result.EnsureSuccessStatusCode();
                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                return response.Result.Content.ReadAsStringAsync().Result;
            }
        }
        public static async Task<string> PostAsync(string url, IEnumerable<KeyValuePair<string, string>> postData = null)
        {
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            //创建HttpClient（注意传入HttpClientHandler）
            using (var http = new HttpClient(handler))
            {
                http.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                //使用FormUrlEncodedContent做HttpContent
                var content = postData == null ? null : new FormUrlEncodedContent(postData);
                //await异步等待回应
                var response = await http.PostAsync(url, content);
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
