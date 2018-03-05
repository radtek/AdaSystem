using System;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading.Tasks;
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

namespace QuartzTask.Jobs
{
    //抓微信文章
    [DisallowConcurrentExecution]
    public class WeiXinArticlesJob : IJob
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
                      d.IsDelete == false && d.MediaType.CallIndex == "weixin" && d.IsSlide == true && d.Status == Consts.StateNormal &&
                      (d.CollectionDate == null || SqlFunctions.DateDiff("hour", d.CollectionDate, DateTime.Now) > hour));
                if (media != null)
                {
                    media.CollectionDate = DateTime.Now;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(media.MediaID))
                        {
                            //获取api信息
                            var apiInfo = db.Set<APIInterfaces>().FirstOrDefault(d => d.CallIndex == "weixinpro");
                            if (apiInfo != null)
                            {
                                string url = string.Format(apiInfo.APIUrl + "?apikey={0}&uid={1}&range=d", apiInfo.Token,
                                    media.MediaID);
                                int times = apiInfo.TimeOut ?? 3;
                                int page = 1;
                                for (int i = 1; i <= page; i++)
                                {
                                    var apiUrl = url + "&pageToken=" + i;
                                    string htmlstr = string.Empty;
                                    int request = 1;
                                    while (request <= times)
                                    {
                                        try
                                        {
                                            htmlstr = HttpUtility.Get(apiUrl);
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
                                    if (string.IsNullOrWhiteSpace(htmlstr))
                                    {
                                        break;
                                    }
                                    var result = JsonConvert.DeserializeObject<WeiXinProJSON>(htmlstr);
                                    //失败日志
                                    if (result.retcode != ReturnCode.请求成功)
                                    {
                                        //APIRequestRecord record = new APIRequestRecord();
                                        //record.Id = IdBuilder.CreateIdNum();
                                        //record.IsSuccess = false;
                                        //record.RequestParameters = media.MediaID;
                                        //record.Retcode = result.retcode.GetHashCode().ToString();
                                        //record.Retmsg = result.message;
                                        //record.ReponseContent = htmlstr;
                                        //record.ReponseDate = DateTime.Now;
                                        //record.AddedById = "系统自动";
                                        //record.AddedBy = "系统自动";
                                        //apiInfo.APIRequestRecords.Add(record);
                                        break;
                                    }
                                    if (result.data.Count > 0)
                                    {
                                        foreach (var articleData in result.data)
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
                                    //如果没有下一页就退出
                                    if (result.hasNext == false)
                                    {
                                        break;
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
                                job.Remark = "获取微信文章任务正在运行中，本次成功更新：" + media.MediaName + "-" + media.MediaID;
                            }
                        }
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】微信文章任务异常", ex);
                        db.SaveChanges();
                    }
                    
                }
                else
                {
                    if (job != null) job.Remark = "获取微信文章任务暂无可更新的资源数据！更新时间：" + DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
    }
}