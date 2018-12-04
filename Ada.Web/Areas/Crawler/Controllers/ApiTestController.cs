using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Framework.Filter;
using Ada.Services.Resource;
using Crawler.Models;
using Crawler.Services;

namespace Crawler.Controllers
{
    public class ApiTestController : BaseController
    {
        private readonly IHttpHelper _httpHelper;
        private readonly IMediaService _mediaService;
        public ApiTestController(IHttpHelper httpHelper, IMediaService mediaService)
        {
            _httpHelper = httpHelper;
            _mediaService = mediaService;
        }
        public ActionResult Index()
        {

            return View();
        }
        [HttpPost, AdaValidateAntiForgeryToken, AllowAnonymous]
        public async Task<ActionResult> WeiXin(ApiRequest apiRequest)
        {
            //var result = await _httpHelper.Get<WeiXinInfosJSON>(apiRequest.UrlPath);
            //return result.data.Any() ? Json(new{State=1,Msg= result.data.FirstOrDefault()?.biz }) : Json(new {State = 0, Msg = result.ToString()});
            string biz = string.Empty;
            await _httpHelper.Get<WeiXinInfosJSON>(apiRequest.UrlPath).ContinueWith(d =>
             {
                 if (!d.Result.data.Any()) return;
                 var weixin = d.Result.data.FirstOrDefault();
                 if (weixin != null)
                 {
                     if (!string.IsNullOrWhiteSpace(weixin.biz))
                     {
                         _mediaService.Update(m => m.MediaID == weixin.id && m.MediaType.CallIndex == "weixin", u => new Media() { MediaLink = weixin.biz });
                         biz = weixin.biz;
                     }
                 }
             });
            return Json(new { State = 1, Msg = "更新BIZ成功：" + biz });
        }

    }
}