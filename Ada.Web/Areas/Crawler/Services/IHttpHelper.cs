using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Ada.Core;

namespace Crawler.Services
{
    public interface IHttpHelper :ISingleDependency
    {
        Task<T> Get<T>(string apiUri);
        Task Get<T>(string apiUri, Action<Task<T>> callBack);
        Task<HttpStatusCode> Post<T>(string apiUri, T newData);
        Task<T> Post<T>(string apiUri, HttpContent content);
        Task Post<T>(string apiUri, HttpContent content, Action<Task<T>> callBack);
    }
}
