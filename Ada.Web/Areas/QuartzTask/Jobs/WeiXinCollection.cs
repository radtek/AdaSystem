using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.API;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Data;
using log4net;
using Newtonsoft.Json;
using Quartz;
using HttpUtility = Ada.Core.Tools.HttpUtility;

namespace QuartzTask.Jobs
{
    public class WeiXinCollection
    {
        private const string WxInfoApi = "http://120.76.205.241:8000/profile/weixin";
        private const string WxArticleApi = "http://120.76.205.241:8000/post/weixinpro";
        private const string Token = "aHkIQg6KZL5nKgqhcAbrT7AYq484DkAfmFzd8rBgYDrK6CItsvAAOWwz7BiFkoQx";
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void GetInfoAndArticle(IJobExecutionContext context, string[] where)
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
                      d.IsDelete == false &&
                      where.Contains(d.TransactorId) &&
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
                            string url = string.Format(WxInfoApi + "?apikey={0}&id={1}", Token,
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
                                    if (DateTime.TryParse(weixinInfo.lastPost?.date, out var date))
                                    {
                                        media.LastPushDate = date;
                                        if (!isUpdateArticle)
                                        {
                                            if (date != lastPost.Value)
                                            {
                                                isUpdateArticle = true;
                                            }
                                        }
                                    }
                                    //更新文章
                                    if (isUpdateArticle&& weixinInfo.lastPost != null)
                                    {
                                        //更新日期范围
                                        var start = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                                        var end = DateTime.Now.ToString("yyyy-MM-dd");
                                        var timeSpan = DateTime.Now - date;
                                        if (timeSpan.TotalDays > 2)
                                        {
                                            start = date.AddDays(-2).ToString("yyyy-MM-dd");
                                            end = date.ToString("yyyy-MM-dd");
                                        }
                                        string urlArticle = string.Format(WxArticleApi + "?apikey={0}&uid={1}&pageToken=1&beginDate={2}&endDate={3}", Token,
                                            media.MediaID, start, end);
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
                                                foreach (var articleData in resultArticle.data)
                                                {
                                                    var article = media.MediaArticles.FirstOrDefault(d => d.ArticleId == articleData.id);
                                                    if (article != null)
                                                    {
                                                        article.ArticleIdx = articleData.idx;
                                                        article.ArticleUrl = articleData.url;
                                                        article.IsOriginal = articleData.original;
                                                        article.Biz = articleData.biz;
                                                        article.CommentCount = articleData.commentCount;
                                                        article.Content = articleData.content;
                                                        article.IsTop = articleData.isTop;
                                                        article.PublishDate = string.IsNullOrWhiteSpace(articleData.publishDateStr)
                                                            ? (DateTime?)null
                                                            : DateTime.Parse(articleData.publishDateStr);
                                                        article.LikeCount = articleData.likeCount;
                                                        article.ViewCount = articleData.viewCount;
                                                        article.Title = articleData.title;
                                                    }
                                                    else
                                                    {
                                                        article = new MediaArticle();
                                                        article.Id = IdBuilder.CreateIdNum();
                                                        article.ArticleId = articleData.id;
                                                        article.ArticleIdx = articleData.idx;
                                                        article.ArticleUrl = articleData.url;
                                                        article.IsOriginal = articleData.original;
                                                        article.Biz = articleData.biz;
                                                        article.CommentCount = articleData.commentCount;
                                                        article.Content = articleData.content;
                                                        article.IsTop = articleData.isTop;
                                                        article.PublishDate = string.IsNullOrWhiteSpace(articleData.publishDateStr)
                                                            ? (DateTime?)null
                                                            : DateTime.Parse(articleData.publishDateStr);
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
                        }
                        //改变工作计划时间
                        if (context.NextFireTimeUtc != null)
                        {
                            if (job != null)
                            {
                                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
                                job.Remark = "获取微信基本信息任务正在运行中，本次成功更新：" + media.MediaName + "-" + media.MediaID;
                            }
                        }
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】微信基本信息任务异常", ex);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (job != null) job.Remark = "获取微信基本信息任务暂无可更新的资源数据！更新时间：" + DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
    }
}