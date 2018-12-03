using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Ada.Framework.Filter;
using Crawler.Models;
using Crawler.Services;

namespace Crawler.Controllers
{
    public class ApiTestController : BaseController
    {
        private readonly IHttpHelper _httpHelper;
        public ApiTestController(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost,AdaValidateAntiForgeryToken,AllowAnonymous]
        public async Task<ActionResult> WeiXin(ApiRequest apiRequest)
        {
            var result = await _httpHelper.Get<ApiRequest>(apiRequest.UrlPath);
            return Json(result);
        }
        
    }
}