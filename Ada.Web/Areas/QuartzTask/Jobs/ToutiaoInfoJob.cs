using System;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.Linq;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.API;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Data;
using log4net;
using Newtonsoft.Json;
using Quartz;
using QuartzTask.Models;


namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class ToutiaoInfoJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Execute(IJobExecutionContext context)
        {
            using (var db = new AdaEFDbcontext())
            {
                var name = context.JobDetail.Key.Name;
                var group = context.JobDetail.Key.Group;
                var job = db.Set<Job>().FirstOrDefault(d => d.GroupName == group && d.JobName == name);
                int hour;
                if (job != null)
                {
                    hour = job.Taxis ?? 720;
                    var media = db.Set<Media>().FirstOrDefault(d =>
                      d.IsDelete == false && d.MediaType.CallIndex == "toutiao" && d.IsSlide == true && d.Status == Consts.StateNormal &&
                      (d.CollectionDate == null || SqlFunctions.DateDiff("hour", d.CollectionDate, DateTime.Now) > hour));
                    if (media != null)
                    {
                        media.CollectionDate = DateTime.Now;
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(media.MediaID))
                            {
                                string url = job.ApiUrl + media.MediaID;
                                int times = 3;
                                int request = 1;
                                string htmlstr = string.Empty;
                                while (request <= times)
                                {
                                    try
                                    {
                                        htmlstr = HttpUtility.Get(url);
                                        request = 9999;
                                    }
                                    catch (Exception ex)
                                    {
                                        if (request == times)
                                        {
                                            _logger.Error("采集今日头条用户信息工作任务抓取" + times + "次都失败", ex);
                                        }
                                        request++;
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(htmlstr))
                                {
                                    var result = JsonConvert.DeserializeObject<ToutiaoJSON>(htmlstr);
                                    if (result.data.Any())
                                    {
                                        //找到ID匹配的进行更新
                                        var mediaInfo = result.data.FirstOrDefault(d => d.id == media.MediaID.Trim());
                                        if (mediaInfo != null)
                                        {
                                            media.MediaName = Utils.FilterEmoji(mediaInfo.screenName);
                                            media.Abstract = mediaInfo.idVerifiedInfo;
                                            media.Content = mediaInfo.biography;
                                            media.FansNum = mediaInfo.fansCount;
                                            media.PostNum = mediaInfo.videoCount;
                                            media.MediaLink = mediaInfo.url;
                                            media.FriendNum = mediaInfo.followCount;
                                            media.TransmitNum = mediaInfo.shareCount;
                                            media.MediaLogo = mediaInfo.avatarUrl;
                                            media.IsAuthenticate = mediaInfo.idVerified;
                                        }
                                    }
                                }
                            }
                            //改变工作计划时间
                            if (context.NextFireTimeUtc != null)
                            {
                                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
                                job.Remark = "Success:" + media.MediaName + "-" + media.MediaID;
                            }

                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            context.Scheduler.PauseJob(context.JobDetail.Key);
                            if (ex is DbEntityValidationException exception)
                            {
                                var error = JobHelper.GetFullErrorText(exception);
                                _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】今日头条用户信息任务异常，任务停止:" + error, exception);
                            }
                            else
                            {
                                _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】今日头条用户信息任务异常，任务停止", ex);
                            }
                        }
                    }
                    else
                    {
                        job.Remark = "None:" + DateTime.Now;
                        db.SaveChanges();
                    }
                }
                else
                {
                    context.Scheduler.PauseJob(context.JobDetail.Key);
                    _logger.Error("今日头条用户工作任务不存在，工作任务停止");
                }

            }
        }
    }
}