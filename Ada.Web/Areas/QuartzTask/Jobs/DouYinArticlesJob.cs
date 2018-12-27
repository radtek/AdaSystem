using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.Linq;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.API;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Admin;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Data;
using log4net;
using Newtonsoft.Json;
using Quartz;
using QuartzTask.Models;


namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class DouYinArticlesJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Execute(IJobExecutionContext context)
        {
            using (var db = new AdaEFDbcontext())
            {
                var name = context.JobDetail.Key.Name;
                var group = context.JobDetail.Key.Group;
                var job = db.Set<Job>().FirstOrDefault(d => d.GroupName == group && d.JobName == name);
                int hour = 24;
                if (job != null)
                {
                    hour = job.Taxis ?? 24;
                }
                var media = db.Set<Media>().FirstOrDefault(d =>
                      d.IsDelete == false && d.MediaType.CallIndex == "douyin" && d.IsSlide == true && d.Status == Consts.StateNormal &&
                      (d.LastPushDate == null || SqlFunctions.DateDiff("hour", d.LastPushDate, DateTime.Now) > hour));
                if (media != null)
                {
                    media.LastPushDate = DateTime.Now;
                    var viewCount = media.MediaArticles.OrderByDescending(d => d.PublishDate).Take(50)
                        .Average(d => d.ViewCount);
                    media.AvgReadNum = Convert.ToInt32(viewCount);
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(media.MediaID))
                        {
                            if (Utils.IsNum(media.MediaID.Trim()))
                            {
                                //获取api信息
                                var apiInfo = db.Set<APIInterfaces>().FirstOrDefault(d => d.CallIndex == "douyinvideo");
                                if (apiInfo != null)
                                {
                                    string url = string.Format(apiInfo.APIUrl + "?apikey={0}&uid={1}", apiInfo.Token,
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
                                        var result = JsonConvert.DeserializeObject<DouYinArticleJSON>(htmlstr);
                                        if (result.data.Count > 0)
                                        {
                                            
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
                                                    article.Content = articleData.description;
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
                                                    article.Content = articleData.description;
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
                                job.Remark = "获取抖音文章任务正在运行中，本次成功更新：" + media.MediaName + "-" + media.MediaID;
                            }
                        }
                        media.AvgReadNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate)
                            .Take(50).Average(aaa => aaa.ViewCount));
                        media.CommentNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate)
                            .Take(50).Average(aaa => aaa.CommentCount));
                        media.LikesNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate)
                            .Take(50).Average(aaa => aaa.LikeCount));
                        media.TransmitNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate)
                            .Take(50).Average(aaa => aaa.ShareCount));
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        context.Scheduler.PauseJob(context.JobDetail.Key);
                        if (ex is DbEntityValidationException exception)
                        {
                            var error = JobHelper.GetFullErrorText(exception);
                            _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】抖音文章任务异常，任务停止:" + error, exception);
                        }
                        else
                        {
                            _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】抖音文章任务异常，任务停止", ex);
                        }
                    }
                }
                else
                {
                    if (job != null) job.Remark = "获取抖音文章任务暂无可更新的资源数据！更新时间：" + DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
    }
}