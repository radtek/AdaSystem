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
            //_client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Chrome"));
        }
        public async Task<T> Get<T>(string apiUri)
        {
            var response = await _client.GetAsync(apiUri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }
        public async Task<HttpStatusCode> Post<T>(string apiUri, T newData)
        {
            var response = await _client.PostAsJsonAsync(apiUri, newData);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;

        }
        public async Task<T> Post<T>(string apiUri, HttpContent content)
        {
            var response = await _client.PostAsync(apiUri, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();

        }
    }
}