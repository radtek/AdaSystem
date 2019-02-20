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
    public class DouYinInfoJob : IJob
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
                      d.IsDelete == false && d.MediaType.CallIndex == "douyin" && d.IsSlide == true && d.Status == Consts.StateNormal &&
                      (d.CollectionDate == null || SqlFunctions.DateDiff("hour", d.CollectionDate, DateTime.Now) > hour));
                if (media != null)
                {
                    media.CollectionDate = DateTime.Now;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(media.MediaID))
                        {
                            if (Utils.IsNum(media.MediaID.Trim()))
                            {
                                //获取api信息
                                var apiInfo = db.Set<APIInterfaces>().FirstOrDefault(d => d.CallIndex == "douyininfo");
                                if (apiInfo != null)
                                {
                                    
                                    string url = string.Format(apiInfo.APIUrl + "?apikey={0}&type=profile&id={1}", apiInfo.Token,media.MediaID);
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
                                        var result = JsonConvert.DeserializeObject<DouYinJSON>(htmlstr);
                                        if (result.data.Any())
                                        {
                                            //找到ID匹配的进行更新
                                            var mediaInfo = result.data.FirstOrDefault(d=>d.id==media.MediaID.Trim());
                                            if (mediaInfo!=null)
                                            {
                                                media.MediaName =Utils.FilterEmoji(mediaInfo.screenName);
                                                media.Abstract = string.IsNullOrWhiteSpace(mediaInfo.douyinID) ? mediaInfo.douyinID2 : mediaInfo.douyinID;
                                                media.Content = mediaInfo.biography;
                                                media.FansNum = mediaInfo.fansCount;
                                                media.PostNum = mediaInfo.videoCount;
                                                media.MediaLink = mediaInfo.url;
                                                //media.LikesNum = mediaInfo.favoriteCount;
                                                media.FriendNum = mediaInfo.followCount;
                                                //media.TransmitNum = mediaInfo.coinCount;
                                                media.MediaLogo = mediaInfo.avatarUrl;
                                                media.IsAuthenticate = mediaInfo.idVerified;
                                                var sex = Utils.BlogSex(mediaInfo.gender);
                                                if (!string.IsNullOrWhiteSpace(sex))
                                                {
                                                    media.Sex = sex;
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
                                job.Remark = "Success:" + media.MediaName + "-" + media.MediaID;
                            }
                        }

                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        context.Scheduler.PauseJob(context.JobDetail.Key);
                        if (ex is DbEntityValidationException exception)
                        {
                            var error = JobHelper.GetFullErrorText(exception);
                            _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】抖音用户信息任务异常，任务停止:" + error, exception);
                        }
                        else
                        {
                            _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】抖音用户信息任务异常，任务停止", ex);
                        }
                    }
                }
                else
                {
                    if (job != null) job.Remark = "None:" + DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
    }
}