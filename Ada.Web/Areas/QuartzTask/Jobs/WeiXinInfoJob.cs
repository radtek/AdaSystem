using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Caching;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Data;
using log4net;
using Newtonsoft.Json;
using Quartz;
using QuartzTask.Models;
using HttpUtility = Ada.Core.Tools.HttpUtility;

namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class WeiXinInfoJob1 : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            WeiXinJob.GetInfoAndArticle(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinInfoJob2 : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            WeiXinJob.GetInfoAndArticle(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinInfoJob3 : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            WeiXinJob.GetInfoAndArticle(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinInfoJob4 : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            WeiXinJob.GetInfoAndArticle(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinInfoJob5 : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            WeiXinJob.GetInfoAndArticle(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinInfoJob6 : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            WeiXinJob.GetInfoAndArticle(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinInfoJob7 : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            WeiXinJob.GetInfoAndArticle(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinInfoJob8 : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            WeiXinJob.GetInfoAndArticle(context);
        }
    }
    public static class WeiXinJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly MemoryCache Cache = MemoryCache.Default;
        public static void GetInfoAndArticle(IJobExecutionContext context)
        {
            using (var db = new AdaEFDbcontext())
            {
                var name = context.JobDetail.Key.Name;
                var group = context.JobDetail.Key.Group;
                var job = db.Set<Job>().FirstOrDefault(d => d.GroupName == group && d.JobName == name);
                if (job != null)
                {
                    var hour = job.Taxis ?? 24;
                    //获取参数
                    var where = job.Params.Split(',');
                    var token = job.Token;
                    var apiUrls = job.ApiUrl.Split(',');
                    var wxInfoApi = apiUrls[0];
                    var wxArticleApi = apiUrls[1];
                    var now = DateTime.Now.AddMonths(-6);
                    var media = db.Set<Media>().FirstOrDefault(d =>
                      d.IsDelete == false &&
                      where.Contains(d.TransactorId) &&
                      (d.LastPushDate > now || d.LastPushDate==null)&&
                      d.MediaType.CallIndex == "weixin" &&
                      d.IsSlide == true &&
                      d.Status == Consts.StateNormal &&
                      (d.CollectionDate == null || SqlFunctions.DateDiff("hour", d.CollectionDate, DateTime.Now) > hour));
                    if (media != null)
                    {
                        media.CollectionDate = DateTime.Now;

                        var lastPost = media.LastPushDate;
                        //是否要更新文章
                        var isUpdateArticle = lastPost == null;
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(media.MediaID))
                            {
                                string url = string.Format(wxInfoApi + "?apikey={0}&id={1}", token,
                                        media.MediaID);
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
                                    catch (Exception)
                                    {
                                        request++;
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(htmlstr))
                                {
                                    var result = JsonConvert.DeserializeObject<WeiXinInfosJSON>(htmlstr);
                                    if (result.data.Count > 0)
                                    {
                                        var weixinInfo = result.data[0];
                                        media.IsAuthenticate = weixinInfo.idVerified;
                                        media.MediaName = weixinInfo.screenName;
                                        media.MonthPostNum = weixinInfo.monthPostCount;
                                        media.MediaLogo = weixinInfo.avatarUrl;
                                        media.MediaQR = weixinInfo.qrcodeUrl;
                                        media.Content = weixinInfo.biography;
                                        media.Abstract = weixinInfo.idVerifiedInfo;
                                        if (DateTime.TryParse(weixinInfo.lastPost?.date, out var date))
                                        {
                                            media.LastPushDate = date;
                                            if (!isUpdateArticle)
                                            {
                                                //如果最后一次发布日期和存储的最后一次发布日期不一致，就更新
                                                if (date != lastPost.Value)
                                                {
                                                    isUpdateArticle = true;
                                                }
                                                else
                                                {
                                                    //如果最后一次发布日期与存储的最后一次发布日期一致，但最后一次发布日期与当前时间在2天范围内，也更新
                                                    var now2Day = DateTime.Now.Date.AddDays(-2);
                                                    if (date >= now2Day)
                                                    {
                                                        isUpdateArticle = true;
                                                    }
                                                }


                                            }
                                        }
                                        //更新文章
                                        if (isUpdateArticle && weixinInfo.lastPost != null)
                                        {
                                            //更新最新
                                            //var start = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                                            //var end = DateTime.Now.ToString("yyyy-MM-dd");
                                            //var timeSpan = DateTime.Now - date;
                                            //if (timeSpan.TotalDays > 2)
                                            //{
                                            //    start = date.AddDays(-2).ToString("yyyy-MM-dd");
                                            //    end = date.ToString("yyyy-MM-dd");
                                            //}
                                            //string urlArticle = string.Format(wxArticleApi + "?apikey={0}&uid={1}&pageToken=1&beginDate={2}&endDate={3}", token,
                                            //    media.MediaID, start, end);
                                            string urlArticle;
                                            if (string.IsNullOrWhiteSpace(media.MediaLink))
                                            {
                                                urlArticle = string.Format(wxArticleApi + "?apikey={0}&uid={1}&sort=1&strictID=1", token,
                                                    media.MediaID);
                                            }
                                            else
                                            {
                                                urlArticle = string.Format(wxArticleApi + "?apikey={0}&biz={1}&sort=1", token,
                                                    media.MediaLink.Trim());
                                            }
                                            string htmlstrArticle = string.Empty;
                                            int requestArticle = 1;
                                            while (requestArticle <= times)
                                            {
                                                try
                                                {
                                                    htmlstrArticle = HttpUtility.Get(urlArticle);
                                                    requestArticle = 9999;
                                                }
                                                catch (Exception)
                                                {
                                                    requestArticle++;
                                                }
                                            }
                                            if (!string.IsNullOrWhiteSpace(htmlstrArticle))
                                            {
                                                var resultArticle = JsonConvert.DeserializeObject<WeiXinProJSON>(htmlstrArticle);
                                                if (resultArticle.data.Count > 0)
                                                {
                                                    //var cacheObj = Cache.Get("Brand");
                                                    if (Cache.Get("Brand") == null)
                                                    {
                                                        var temp = db.Set<Field>().Where(d => d.FieldType.CallIndex == "Brand").Select(d => d.Text).ToList();
                                                        var policy = new CacheItemPolicy() { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration };
                                                        Cache.Set("Brand", temp, policy);
                                                    }
                                                    List<string> brands = Cache.Get("Brand") as List<string>;
                                                    foreach (var articleData in resultArticle.data)
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
                                                            if (string.IsNullOrWhiteSpace(articleData.publishDateStr))
                                                                continue;
                                                            article.ArticleIdx = articleData.idx;
                                                            article.ArticleUrl = articleData.url;
                                                            article.IsOriginal = articleData.original;
                                                            article.Biz = articleData.biz;
                                                            article.CommentCount = articleData.commentCount;
                                                            article.Content = GetBrands(articleData.content, brands);
                                                            article.IsTop = articleData.isTop;
                                                            if (DateTime.TryParse(articleData.publishDateStr, out var date1))
                                                            {
                                                                article.PublishDate = date1;
                                                            }
                                                            article.LikeCount = articleData.likeCount;
                                                            article.ViewCount = articleData.viewCount;
                                                            article.Title = articleData.title;
                                                            media.MediaLink = articleData.biz;
                                                            //if (!string.IsNullOrWhiteSpace(articleData.posterId))
                                                            //{
                                                            //    media.MediaID = articleData.posterId;
                                                            //}
                                                        }
                                                        else
                                                        {
                                                            if (string.IsNullOrWhiteSpace(articleData.publishDateStr))
                                                                continue;
                                                            article = new MediaArticle();
                                                            article.Id = IdBuilder.CreateIdNum();
                                                            article.ArticleId = articleData.id;
                                                            article.ArticleIdx = articleData.idx;
                                                            article.ArticleUrl = articleData.url;
                                                            article.IsOriginal = articleData.original;
                                                            article.Biz = articleData.biz;
                                                            article.CommentCount = articleData.commentCount;
                                                            article.Content = GetBrands(articleData.content, brands);
                                                            article.IsTop = articleData.isTop;
                                                            if (DateTime.TryParse(articleData.publishDateStr, out var date1))
                                                            {
                                                                article.PublishDate = date1;
                                                            }
                                                            article.LikeCount = articleData.likeCount;
                                                            article.ViewCount = articleData.viewCount;
                                                            article.Title = articleData.title;
                                                            media.MediaArticles.Add(article);
                                                            media.MediaLink = articleData.biz;
                                                            //if (!string.IsNullOrWhiteSpace(articleData.posterId))
                                                            //{
                                                            //    media.MediaID = articleData.posterId;
                                                            //}
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
                                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
                                job.Remark = "Success:" + media.MediaName + "-" + media.MediaID;
                            }

                            //更新统计数据
                            var viewCount = media.MediaArticles.Where(d => d.IsTop == true).OrderByDescending(d => d.PublishDate).Take(10)
                                .Average(d => d.ViewCount);
                            media.AvgReadNum = Convert.ToInt32(viewCount);
                            media.LastReadNum = media.MediaArticles.Where(l => l.IsTop == true)
                                .OrderByDescending(a => a.PublishDate).FirstOrDefault()?.ViewCount;
                            var start30 = DateTime.Now.Date.AddDays(-30);
                            var end30 = DateTime.Now.AddDays(1).Date;
                            var count = media.MediaArticles.Where(d => d.PublishDate >= start30 && d.PublishDate < end30).Select(d => new
                            {
                                Date = d.PublishDate.Value.ToString("yyyy-MM-dd")
                            }).GroupBy(d => d.Date).Count();
                            media.PublishFrequency = count;
                            db.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                            context.Scheduler.PauseJob(context.JobDetail.Key);
                            if (ex is DbEntityValidationException exception)
                            {
                                var error = JobHelper.GetFullErrorText(exception);
                                Logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】微信基本信息任务异常，任务停止:" + error, exception);
                            }
                            else
                            {
                                Logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】微信基本信息任务异常，任务停止", ex);
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
                    Logger.Error("工作任务不存在！");
                }

            }
        }

        private static string GetBrands(string content, List<string> brands)
        {
            string htmlstr = string.Empty;
            int request = 1;
            while (request <= 3)
            {
                try
                {
                    htmlstr = HttpUtility.Post(
                        "http://120.76.205.241:8000/nlp/segment/bitspaceman?apikey=aHkIQg6KZL5nKgqhcAbrT7AYq484DkAfmFzd8rBgYDrK6CItsvAAOWwz7BiFkoQx",
                        new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("text", content) });
                    request = 9999;
                }
                catch (Exception)
                {
                    request++;
                }
            }
            if (!string.IsNullOrWhiteSpace(htmlstr))
            {
                var resultJson = JsonConvert.DeserializeObject<BitspacemanJSON>(htmlstr);
                if (resultJson.wordList.Any())
                {
                    var words = resultJson.wordList.Where(d => d.length > 1 && brands.Contains(d.word)).Select(d => d.word);
                    words = words.Distinct().ToList();
                    return string.Join(",", words);
                }
            }
            return null;
        }

        

    }
}