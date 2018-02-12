﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Ada.Core;
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
                _logger.Info("微信文章自动任务开始：" + DateTime.Now);
                try
                {
                    var medias = _repository.LoadEntities(d => d.IsDelete == false && d.MediaType.CallIndex == "weixin" && d.IsSlide == true).OrderBy(d=>d.Id).ToList();
                    long addcount = 0;
                    long updatecount = 0;
                    foreach (var media in medias)
                    {
                        if (string.IsNullOrWhiteSpace(media.MediaID)) continue;
                        WeiXinProParams wxparams = new WeiXinProParams
                        {
                            PageNum = 6,
                            Range = "w",
                            CallIndex = "weixinpro",
                            IsLog = false,
                            UID = media.MediaID.Trim(),
                            Transactor = "系统自动",
                            TransactorId = "系统自动"
                        };
                        try
                        {
                            var result = _iDataAPIService.GetWeiXinArticles(wxparams);
                            addcount += result.AddCount;
                            updatecount += result.UpdateCount;
                        }
                        catch (Exception e)
                        {
                            _logger.Error("微信" + media.MediaID + ",自动任务失败：" + DateTime.Now, e);
                        }
                    }

                    _logger.Info("微信文章自动任务结束：" + DateTime.Now + "，共成功采集新增" + addcount + "文章篇，更新" + updatecount + "文章篇");
                }
                catch (Exception ex)
                {
                    _logger.Info("微信文章自动任务异常结束！", ex);
                }

            });
        }
    }
}