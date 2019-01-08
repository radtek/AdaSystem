using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.API;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Domain.Resource;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Data;
using Ada.Services.API;
using log4net;
using Newtonsoft.Json;
using Quartz;
using QuartzTask.Models;
using HttpUtility = Ada.Core.Tools.HttpUtility;

namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class WeiboArticlesJob : IJob
    {

        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Execute(IJobExecutionContext context)
        {
            using (var db = new AdaEFDbcontext())
            {
                var name = context.JobDetail.Key.Name;
                var group = context.JobDetail.Key.Group;
                var job = db.Set<Job>().FirstOrDefault(d => d.GroupName == group && d.JobName == name);
                int hour = 16;
                if (job != null)
                {
                    hour = job.Taxis ?? 16;
                }
                var media = db.Set<Media>().FirstOrDefault(d =>
                      d.IsDelete == false && d.MediaType.CallIndex == "sinablog" && d.IsSlide == true && d.Status==Consts.StateNormal&&
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
                                var apiInfo = db.Set<APIInterfaces>().FirstOrDefault(d => d.CallIndex == "weibo");
                                if (apiInfo != null)
                                {
                                    string url = string.Format(apiInfo.APIUrl + "?apikey={0}&uid={1}&pageToken=1", apiInfo.Token,
                                        media.MediaID);
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
                                                exrecord.RequestParameters = media.MediaID;
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
                                        var result = JsonConvert.DeserializeObject<WeiBoJSON>(htmlstr);
                                        ////失败日志
                                        //if (result.retcode != ReturnCode.请求成功)
                                        //{
                                        //    APIRequestRecord record = new APIRequestRecord();
                                        //    record.Id = IdBuilder.CreateIdNum();
                                        //    record.IsSuccess = false;
                                        //    record.RequestParameters = media.MediaID;
                                        //    record.Retcode = result.retcode.GetHashCode().ToString();
                                        //    record.Retmsg = result.message;
                                        //    record.ReponseContent = htmlstr;
                                        //    record.ReponseDate = DateTime.Now;
                                        //    record.AddedById = "系统自动";
                                        //    record.AddedBy = "系统自动";
                                        //    apiInfo.APIRequestRecords.Add(record);
                                        //}

                                        if (result.data.Count > 0)
                                        {
                                            //根据第一个数据更新微博信息
                                            var mediaInfo = result.data[0].from;
                                            media.MediaName = mediaInfo.name;
                                            media.Content = mediaInfo.description;
                                            media.FansNum = mediaInfo.fansCount;
                                            media.PostNum = mediaInfo.postCount;
                                            media.MediaLink = mediaInfo.url;
                                            media.FriendNum = mediaInfo.friendCount;
                                            media.MediaLogo = mediaInfo.extend?.avatar_large;
                                            media.Area = mediaInfo.extend?.location;
                                            media.IsAuthenticate = mediaInfo.extend?.verified;
                                            media.Abstract = mediaInfo.extend?.verified_reason;
                                            var authType = Utils.BlogAuthenticateType(mediaInfo.extend?.verified_type);
                                            if (!string.IsNullOrWhiteSpace(authType))
                                            {
                                                media.AuthenticateType = authType;
                                            }
                                            var sex = Utils.BlogSex(mediaInfo.extend?.gender);
                                            if (!string.IsNullOrWhiteSpace(sex))
                                            {
                                                media.Sex = sex;
                                            }
                                            foreach (var articleData in result.data)
                                            {
                                                if (string.IsNullOrWhiteSpace(articleData.id))
                                                {
                                                    continue;
                                                }
                                                if (articleData.id.Length > 128)
                                                {
                                                    continue;
                                                }
                                                var article = media.MediaArticles.FirstOrDefault(d => d.ArticleId == articleData.id);
                                                if (article != null)
                                                {
                                                    article.ArticleUrl = articleData.url;
                                                    article.CommentCount = articleData.commentCount;
                                                    article.Content = articleData.content;
                                                    article.ShareCount = articleData.shareCount;
                                                    if (DateTime.TryParse(articleData.publishDateStr, out var date))
                                                    {
                                                        article.PublishDate = date;
                                                    }
                                                    article.LikeCount = articleData.likeCount;
                                                    article.ViewCount = articleData.viewCount;
                                                    article.Title = articleData.title;
                                                }
                                                else
                                                {
                                                    article = new MediaArticle();
                                                    article.Id = IdBuilder.CreateIdNum();
                                                    article.ArticleId = articleData.id;
                                                    article.ArticleUrl = articleData.url;
                                                    article.CommentCount = articleData.commentCount;
                                                    article.Content = articleData.content;
                                                    article.ShareCount = articleData.shareCount;
                                                    if (DateTime.TryParse(articleData.publishDateStr, out var date))
                                                    {
                                                        article.PublishDate = date;
                                                    }
                                                    article.LikeCount = articleData.likeCount;
                                                    article.ViewCount = articleData.viewCount;
                                                    article.Title = articleData.title;
                                                    media.MediaArticles.Add(article);
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

                        media.TransmitNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate).Take(50)
                            .Average(aaa => aaa.ShareCount));
                        media.CommentNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate)
                            .Take(50).Average(aaa => aaa.CommentCount));
                        media.LikesNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate).Take(50)
                            .Average(aaa => aaa.LikeCount));
                        media.LastPushDate = media.MediaArticles.OrderByDescending(a => a.PublishDate).FirstOrDefault()?.PublishDate;
                        var weekDate = DateTime.Now.Date.AddDays(-7);
                        media.MonthPostNum = media.MediaArticles.OrderByDescending(a => a.PublishDate)
                            .Count(l => l.PublishDate > weekDate);

                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        context.Scheduler.PauseJob(context.JobDetail.Key);
                        if (ex is DbEntityValidationException exception)
                        {
                            var error = JobHelper.GetFullErrorText(exception);
                            _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】微博文章任务异常，任务停止:" + error, exception);
                        }
                        else
                        {
                            _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】微博文章任务异常，任务停止", ex);
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