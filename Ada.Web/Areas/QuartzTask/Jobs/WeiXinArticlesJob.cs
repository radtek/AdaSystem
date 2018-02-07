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
using log4net;
using Quartz;

namespace QuartzTask.Jobs
{
    //抓微信文章
    public class WeiXinArticlesJob : IJob
    {
        private readonly IiDataAPIService _iDataAPIService;
        private readonly IRepository<Media> _repository;
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public WeiXinArticlesJob()
        {
            _iDataAPIService = EngineContext.Current.Resolve<IiDataAPIService>();
            _repository = EngineContext.Current.Resolve<IRepository<Media>>();
        }
        public void Execute(IJobExecutionContext context)
        {
            Task.Factory.StartNew(() =>
            {
                _logger.Info("微信自动任务开始：" + DateTime.Now);
                var medias = _repository.LoadEntities(d => d.IsDelete == false && d.MediaType.CallIndex == "weixin").ToList();
                foreach (var media in medias)
                {
                    if (string.IsNullOrWhiteSpace(media.MediaID)) continue;
                    WeiXinProParams wxparams = new WeiXinProParams
                    {
                        PageNum = 1,
                        Range = "d",
                        CallIndex = "weixinpro",
                        UID = media.MediaID.Trim()
                    };
                    
                    try
                    {
                        _iDataAPIService.GetWeiXinArticles(wxparams);
                    }
                    catch (Exception e)
                    {
                        _logger.Error("微信"+media.MediaID+",自动任务失败：" + DateTime.Now, e);
                    }
                }
                
                

            });
        }
    }
}