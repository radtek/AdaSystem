using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Ada.Core.Domain;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Domain.Resource;
using Ada.Core.Infrastructure;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Data;
using Ada.Services.Resource;
using Crawler.Models;
using Crawler.Services;
using log4net;
using OpenQA.Selenium;
using Quartz;

namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class WeiXinBizJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IHttpHelper _httpHelper = EngineContext.Current.Resolve<IHttpHelper>();
        private string _currentWx;

        public void Execute(IJobExecutionContext context)
        {
            using (var db = new AdaEFDbcontext())
            {
                var name = context.JobDetail.Key.Name;
                var group = context.JobDetail.Key.Group;
                var job = db.Set<Job>().FirstOrDefault(d => d.GroupName == group && d.JobName == name);
                int hour = job.Taxis ?? 66;
                
                var media = db.Set<Media>().FirstOrDefault(d =>
                      d.IsDelete == false && d.MediaType.CallIndex == "weixin" && d.Status == Consts.StateNormal && (d.MediaLink == null || d.MediaLink == "") &&
                      (d.ApiUpDate == null || SqlFunctions.DateDiff("hour", d.ApiUpDate, DateTime.Now) > hour));
                if (media != null)
                {
                    media.ApiUpDate = DateTime.Now;
                    _currentWx = media.Id;
                    try
                    {
                        _httpHelper.Get<WeiXinInfosJSON>(job.ApiUrl).ContinueWith(d =>
                        {

                            if (d.Result.data.Any())
                            {
                                var result = d.Result.data.FirstOrDefault();
                                if (result!=null)
                                {
                                    if (!string.IsNullOrWhiteSpace(result.biz))
                                    {
                                        media.MediaLink = result.biz;
                                    }
                                }
                            }
                        });
                        //改变工作计划时间
                        if (context.NextFireTimeUtc != null)
                        {
                            job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
                            job.Remark = "获取微信BIZ信息任务正在运行中，本次成功更新：" + media.MediaName + "-" + media.MediaID;
                        }

                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】微信BIZ信息任务异常", ex);
                        db.SaveChanges();
                    }
                }
                else
                {
                    job.Remark = "获取微信BIZ信息任务暂无可更新的资源数据！更新时间：" + DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
        
    }
}