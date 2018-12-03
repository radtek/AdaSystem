using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace Crawler.Services
{
    public class HttpHelper : IHttpHelper
    {
        private readonly HttpClient _client;
        public HttpHelper()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36"));
        }
        public async Task<T> Get<T>(string apiUri)
        {
            var response = await _client.GetAsync(apiUri);
            response.EnsureSuccessStatusCode();
            return default(T);
            //return await response.Content.ReadAsAsync<T>();
        }
        public async Task Get<T>(string apiUri,Action<Task<T>> callBack)
        {
            var response = await _client.GetAsync(apiUri);
            response.EnsureSuccessStatusCode();
            //await response.Content.ReadAsAsync<T>().ContinueWith(callBack);
        }
        public async Task<HttpStatusCode> Post<T>(string apiUri, T newData)
        {
            //var response = await _client.PostAsJsonAsync(apiUri, newData);
            var response = await _client.GetAsync(apiUri);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;

        }
        public async Task<T> Post<T>(string apiUri, HttpContent content)
        {
            var response = await _client.PostAsync(apiUri, content);
            response.EnsureSuccessStatusCode();
            //return await response.Content.ReadAsAsync<T>();
            return default(T);
        }
        public async Task Post<T>(string apiUri, HttpContent content, Action<Task<T>> callBack)
        {
            var response = await _client.PostAsync(apiUri, content);
            response.EnsureSuccessStatusCode();
            //await response.Content.ReadAsAsync<T>().ContinueWith(callBack);
        }
    }
}