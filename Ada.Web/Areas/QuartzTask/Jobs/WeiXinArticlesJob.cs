using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Resource;
using Ada.Core.Infrastructure;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Services.API;
using Quartz;

namespace QuartzTask.Jobs
{
    //抓微信文章
    public class WeiXinArticlesJob : IJob
    {
        private readonly IiDataAPIService _iDataAPIService;
        private readonly IRepository<Media> _repository;
        public WeiXinArticlesJob()
        {
            _iDataAPIService = EngineContext.Current.Resolve<IiDataAPIService>();
            _repository = EngineContext.Current.Resolve<IRepository<Media>>();
        }
        public void Execute(IJobExecutionContext context)
        {
            Task.Factory.StartNew(() =>
            {
                var medias = _repository.LoadEntities(d => d.IsDelete == false && d.Status == Consts.StateNormal && d.MediaType.CallIndex == "weixin").Take(10);
                foreach (var media in medias)
                {
                    WeiXinProParams wxparams = new WeiXinProParams();
                    wxparams.PageNum = 1;
                    wxparams.Range = "d";
                    wxparams.CallIndex = "weixinpro";
                    wxparams.WeiXinId = media.MediaID;
                    _iDataAPIService.GetWeiXinArticlesAsync(wxparams);
                    Thread.Sleep(2000);
                }

            });
        }
    }
}