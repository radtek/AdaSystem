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
    public class RedBookInfoJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Execute(IJobExecutionContext context)
        {
            using (var db = new AdaEFDbcontext())
            {
                var name = context.JobDetail.Key.Name;
                var group = context.JobDetail.Key.Group;
                var job = db.Set<Job>().FirstOrDefault(d => d.GroupName == group && d.JobName == name);
                int hour = 72;
                if (job != null)
                {
                    hour = job.Taxis ?? 72;
                }
                var media = db.Set<Media>().FirstOrDefault(d =>
                      d.IsDelete == false && d.MediaType.CallIndex == "redbook" && d.IsSlide == true && d.Status == Consts.StateNormal &&
                      (d.CollectionDate == null || SqlFunctions.DateDiff("hour", d.CollectionDate, DateTime.Now) > hour));
                if (media != null)
                {
                    media.CollectionDate = DateTime.Now;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(media.MediaID))
                        {
                            //获取api信息
                            var apiInfo = db.Set<APIInterfaces>().FirstOrDefault(d => d.CallIndex == "redbookinfo");
                            if (apiInfo != null)
                            {
                                string url = string.Format(apiInfo.APIUrl + "?apikey={0}&id={1}", apiInfo.Token,
                                    media.MediaID.Trim());
                                int times = apiInfo.TimeOut ?? 3;
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
                                            APIRequestRecord exrecord = new APIRequestRecord();
                                            exrecord.Id = IdBuilder.CreateIdNum();
                                            exrecord.RequestParameters = media.MediaName;
                                            exrecord.IsSuccess = false;
                                            exrecord.Retcode = "500";
                                            exrecord.ReponseContent = ex.Message;
                                            exrecord.Retmsg = "请求异常";
                                            exrecord.ReponseDate = DateTime.Now;
                                            apiInfo.APIRequestRecords.Add(exrecord);
                                        }
                                        request++;
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(htmlstr))
                                {
                                    var result = JsonConvert.DeserializeObject<RedBookJSON>(htmlstr);
                                    if (result.data.Any())
                                    {
                                        //找到ID匹配的进行更新
                                        var mediaInfo = result.data[0];
                                        if (mediaInfo != null)
                                        {
                                            if (!string.IsNullOrWhiteSpace(mediaInfo.screenName))
                                            {
                                                media.MediaName = Utils.FilterEmoji(mediaInfo.screenName);
                                                media.Content = mediaInfo.biography;
                                                media.FansNum = mediaInfo.fansCount;
                                                media.PostNum = mediaInfo.postCount;
                                                media.MediaLink = mediaInfo.url;
                                                media.FriendNum = mediaInfo.followCount;
                                                media.LikesNum = mediaInfo.likeCount;
                                                media.MediaLogo = mediaInfo.avatarUrl;
                                                media.AuthenticateType = mediaInfo.idGrade;
                                                if (!string.IsNullOrWhiteSpace(mediaInfo.location))
                                                {
                                                    if (mediaInfo.location.Length<=30)
                                                    {
                                                        media.Area = mediaInfo.location;
                                                    }
                                                }
                                                
                                            }
                                            
                                           
                                        }


                                    }
                                }
                            }
                        }
                        //改变工作计划时间
                        if (context.NextFireTimeUtc != null)
                        {
                            if (job != null)
                            {
                                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
                                job.Remark = "获取小红书用户信息任务正在运行中，本次成功更新：" + media.MediaName + "-" + media.MediaID;
                            }
                        }

                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        context.Scheduler.PauseJob(context.JobDetail.Key);
                        if (ex is DbEntityValidationException exception)
                        {
                           var error= JobHelper.GetFullErrorText(exception);
                            _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】小红书用户信息任务异常，任务停止:"+ error, exception);
                        }
                        else
                        {
                            _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】小红书用户信息任务异常，任务停止", ex);
                        }
                        
                        
                    }
                }
                else
                {
                    if (job != null) job.Remark = "获取小红书用户信息任务暂无可更新的资源数据！更新时间：" + DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
        
    }
}