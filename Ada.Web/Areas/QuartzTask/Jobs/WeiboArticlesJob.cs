using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Ada.Core;
using Ada.Core.Domain.Resource;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Services.API;
using log4net;
using Quartz;

namespace QuartzTask.Jobs
{
    public class WeiboArticlesJob : IJob
    {
        private readonly IiDataAPIService _iDataAPIService;
        private readonly IRepository<Media> _repository;
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public WeiboArticlesJob()
        {
            _iDataAPIService = EngineContext.Current.Resolve<IiDataAPIService>();
            _repository = EngineContext.Current.Resolve<IRepository<Media>>();
        }
        public void Execute(IJobExecutionContext context)
        {
            Task.Factory.StartNew(() =>
            {
                _logger.Info("微博自动任务开始：" + DateTime.Now);
                var medias = _repository.LoadEntities(d => d.IsDelete == false && d.MediaType.CallIndex == "sinablog" && d.IsSlide==true);
                long addcount = 0;
                long updatecount = 0;
                foreach (var media in medias)
                {
                    if (string.IsNullOrWhiteSpace(media.MediaID)) continue;
                    if (!Utils.IsNum(media.MediaID.Trim())) continue;
                    WeiBoParams wbparams = new WeiBoParams
                    {
                        PageNum = 1,
                        CallIndex = "weibo",
                        IsLog = false,
                        UID = media.MediaID.Trim()
                    };
                    try
                    {
                        var result = _iDataAPIService.GetWeiBoArticles(wbparams);
                        addcount += result.AddCount;
                        updatecount += result.UpdateCount;
                    }
                    catch (Exception e)
                    {
                        _logger.Error("微博" + media.MediaID + "，自动任务失败：" + DateTime.Now, e);
                    }

                }
                _logger.Info("微博自动任务结束：" + DateTime.Now + "，共成功采集新增" + addcount + "文章篇，更新" + updatecount + "文章篇");


            });
        }
    }
}