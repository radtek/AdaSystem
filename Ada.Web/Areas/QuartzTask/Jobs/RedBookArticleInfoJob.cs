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
    public class RedBookArticleInfoJob : IJob
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
                var media = db.Set<MediaArticle>().FirstOrDefault(d =>
                      d.Media.IsDelete == false && d.Remark == "xiaohongshu_ids"  && d.Media.Status == Consts.StateNormal &&
                      (d.ModifiedDate == null || SqlFunctions.DateDiff("hour", d.ModifiedDate, DateTime.Now) > hour));
                if (media != null)
                {
                    media.ModifiedDate = DateTime.Now;
                    
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(media.ArticleId))
                        {
                            //获取api信息
                            var apiInfo = db.Set<APIInterfaces>().FirstOrDefault(d => d.CallIndex == "redbookdatainfo");
                            if (apiInfo != null)
                            {
                                string url = string.Format(apiInfo.APIUrl + "?apikey={0}&id={1}", apiInfo.Token,
                                    media.ArticleId);
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
                                            exrecord.RequestParameters = media.ArticleId;
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
                                    var result = JsonConvert.DeserializeObject<RedBookArticleJSON>(htmlstr);
                                    if (result.data.Any())
                                    {
                                        var articleData = result.data[0];
                                        media.LikeCount = articleData.likeCount;
                                        if (DateTime.TryParse(articleData.publishDateStr, out var date))
                                        {
                                            media.PublishDate = date;
                                        }
                                        media.ShareCount = articleData.favoriteCount;
                                        media.CommentCount = articleData.commentCount;
                                        media.ArticleUrl = articleData.url;
                                        media.Content = articleData.content;
                                        
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
                                job.Remark = "Success:" + media.Media.MediaName + "-" + media.ArticleId;
                            }
                        }
                        
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        context.Scheduler.PauseJob(context.JobDetail.Key);
                        if (ex is DbEntityValidationException exception)
                        {
                            //var error = JobHelper.GetFullErrorText(exception);
                            _logger.Error("小红书文章内容API,Error:" + media.Media.MediaName + "-" + media.ArticleId , exception);
                        }
                        else
                        {
                            _logger.Error("小红书文章内容API,Error:" + media.Media.MediaName + "-" + media.ArticleId, ex);
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