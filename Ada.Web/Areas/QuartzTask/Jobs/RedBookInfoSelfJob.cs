using System;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text.RegularExpressions;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.API;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Domain.Resource;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Data;
using Ada.Services.Resource;
using Crawler.Models;
using Crawler.Services;
using log4net;
using Newtonsoft.Json;
using Quartz;


namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class RedBookInfoSelfJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IWebCrawler _webCrawler= EngineContext.Current.Resolve<IWebCrawler>();
        public void Execute(IJobExecutionContext context)
        {
            using (var db = new AdaEFDbcontext())
            {
                var name = context.JobDetail.Key.Name;
                var group = context.JobDetail.Key.Group;
                var job = db.Set<Job>().FirstOrDefault(d => d.GroupName == group && d.JobName == name);
                int hour = 36;
                if (job != null)
                {
                    hour = job.Taxis ?? 36;
                }
                var media = db.Set<Media>().FirstOrDefault(d =>
                      d.IsDelete == false && d.MediaType.CallIndex == "redbook" && d.Status == Consts.StateNormal &&
                      (d.ApiUpDate == null || SqlFunctions.DateDiff("hour", d.CollectionDate, DateTime.Now) > hour));
                if (media != null)
                {
                    media.ApiUpDate = DateTime.Now;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(media.MediaID))
                        {
                            _webCrawler.OnCompleted += Crawler_OnCompleted;
                            _webCrawler.OnError += Crawler_OnError;
                            _webCrawler.Start(new Uri("https://www.xiaohongshu.com/user/profile/" + media.MediaID),new Operation(){SleepTime = 800});
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
                        _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】淘宝用户信息任务异常", ex);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (job != null) job.Remark = "获取小红书用户信息任务暂无可更新的资源数据！更新时间：" + DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
        private void Crawler_OnCompleted(object sender, OnCompletedEventArgs e)
        {
            var jsonStr = Regex.Match(e.PageSource, @"{""UserDetail"":(.+),""notesDetail""").Groups[1].Value;
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                var user = JsonConvert.DeserializeObject<RedBookUser>(jsonStr);
                if (user != null)
                {
                    if (!string.IsNullOrWhiteSpace(user.id))
                    {
                        var sevice = EngineContext.Current.Resolve<IMediaService>();
                        sevice.Update(d => d.MediaID == user.id, m => new Media()
                        {
                            MediaName = user.nickname,
                            FansNum = user.fans,
                            PostNum = user.notes,
                            FriendNum = user.follows,
                            LikesNum = user.collected + user.liked,
                            Content = user.desc,
                            MediaLogo = user.images,
                            Area = user.location,
                            AuthenticateType = user.level.name,
                            MediaLink = "https://www.xiaohongshu.com/user/profile/" + user.id
                        });
                    }
                }
            }
            

        }
        private void Crawler_OnError(object sender, OnErrorEventArgs e)
        {
            _logger.Error("小红书用户工作任务爬取异常：" + e.Uri, e.Exception);
        }
    }
    class RedBookUser
    {
        public int collected { get; set; }
        public int fans { get; set; }
        public int follows { get; set; }
        public int gender { get; set; }
        public int liked { get; set; }
        public int notes { get; set; }
        public string id { get; set; }
        public string images { get; set; }
        public string desc { get; set; }
        public string location { get; set; }
        public string nickname { get; set; }
        public RedBookUserLevel level { get; set; }
    }

    class RedBookUserLevel
    {
        public string image { get; set; }
        public string name { get; set; }
    }
}