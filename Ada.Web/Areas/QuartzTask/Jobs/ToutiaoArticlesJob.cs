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
    public class ToutiaoArticlesJob : IJob
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
                    hour = job.Taxis ?? 168;
                    var media = db.Set<Media>().FirstOrDefault(d =>
                     d.IsDelete == false && d.MediaType.CallIndex == "toutiao" && d.IsSlide == true && d.Status == Consts.StateNormal &&
                     (d.ApiUpDate == null || SqlFunctions.DateDiff("hour", d.ApiUpDate, DateTime.Now) > hour));
                    if (media != null)
                    {
                        media.ApiUpDate = DateTime.Now;
                        var viewCount = media.MediaArticles.OrderByDescending(d => d.PublishDate).Take(50)
                            .Average(d => d.ViewCount);
                        media.AvgReadNum = Convert.ToInt32(viewCount);
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(media.MediaID))
                            {
                                string url = job.ApiUrl+ media.MediaID.Trim();
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
                                            _logger.Error("采集今日头条文章工作任务抓取" + times + "次都失败", ex);
                                        }
                                        request++;
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(htmlstr))
                                {
                                    var result = JsonConvert.DeserializeObject<ToutiaoArticleJSON>(htmlstr);
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
                                                if (DateTime.TryParse(articleData.publishDateStr, out var date))
                                                {
                                                    article.PublishDate = date;
                                                }
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
                                                if (DateTime.TryParse(articleData.publishDateStr, out var date))
                                                {
                                                    article.PublishDate = date;
                                                }
                                                article.ViewCount = articleData.viewCount;
                                                article.Title = articleData.title;
                                                media.MediaArticles.Add(article);
                                            }
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
                            media.AvgReadNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate)
                                .Take(100).Average(aaa => aaa.ViewCount));
                            media.CommentNum = Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate)
                                .Take(100).Average(aaa => aaa.CommentCount));
                            media.LastPushDate = media.MediaArticles.OrderByDescending(a => a.PublishDate)
                                .FirstOrDefault()?.PublishDate;
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            context.Scheduler.PauseJob(context.JobDetail.Key);
                            if (ex is DbEntityValidationException exception)
                            {
                                var error = JobHelper.GetFullErrorText(exception);
                                _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】今日头条文章任务异常，任务停止:" + error, exception);
                            }
                            else
                            {
                                _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】今日头条文章任务异常，任务停止", ex);
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
                    _logger.Error("今日头条文章工作任务不存在，工作任务停止");
                }
               
            }
        }
    }
}