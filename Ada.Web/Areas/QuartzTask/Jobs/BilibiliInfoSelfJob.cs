using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Ada.Core.Domain;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Domain.Resource;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Data;
using Ada.Services.Resource;
using Crawler.Models;
using Crawler.Services;
using log4net;
using OpenQA.Selenium;
using Quartz;

namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class BilibiliInfoSelfJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IWebCrawler _webCrawler = EngineContext.Current.Resolve<IWebCrawler>();
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
                      d.IsDelete == false && d.MediaType.CallIndex == "bilibili" && d.Status == Consts.StateNormal &&
                      (d.CollectionDate == null || SqlFunctions.DateDiff("hour", d.CollectionDate, DateTime.Now) > hour));
                if (media != null)
                {
                    media.CollectionDate = DateTime.Now;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(media.MediaID))
                        {
                            _webCrawler.OnCompleted += CrawlerBilibili_OnCompleted;
                            _webCrawler.OnError += Crawler_OnError;
                            _webCrawler.Start(new Uri("https://space.bilibili.com/" + media.MediaID),
                                new Operation() { Timeout = 30000, Condition = d => d.FindElements(By.XPath("//div[@id='navigator']")).Any() });
                        }
                        //改变工作计划时间
                        if (context.NextFireTimeUtc != null)
                        {
                            if (job != null)
                            {
                                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
                                job.Remark = "获取B站用户信息任务正在运行中，本次成功更新：" + media.MediaName + "-" + media.MediaID;
                            }
                        }

                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】B站用户信息任务异常", ex);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (job != null) job.Remark = "获取B站用户信息任务暂无可更新的资源数据！更新时间：" + DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
        private void CrawlerBilibili_OnCompleted(object sender, OnCompletedEventArgs e)
        {
            var nick = e.WebDriver.FindElement(By.XPath("//span[@id='h-name']")).Text;
            var image = e.WebDriver.FindElement(By.XPath("//img[@id='h-avatar']")).GetAttribute("src");
            var abstr = e.WebDriver.FindElement(By.XPath("//h4[@class='h-sign']")).Text;
            //var content = e.WebDriver.FindElement(By.XPath("//div[@class='content']/div[@id='i-ann-display']")).Text;

            var fans = e.WebDriver.FindElement(By.XPath("//div[@id='navigator']/div[@class='wrapper']/div[@class='n-inner clearfix']/div[@class='n-statistics']/a[@class='n-data n-fs']")).GetAttribute("title");
            var friends = e.WebDriver.FindElement(By.XPath("//div[@id='navigator']/div[@class='wrapper']/div[@class='n-inner clearfix']/div[@class='n-statistics']/a[@class='n-data n-gz']")).GetAttribute("title");
            var list = e.WebDriver.FindElements(By.XPath("//div[@id='navigator']/div[@class='wrapper']/div[@class='n-inner clearfix']/div[@class='n-statistics']/div[@class='n-data n-bf']"));
            string plays = "", reads = "";
            if (list.Any())
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == 0)
                    {
                        plays = list[i].GetAttribute("title");
                    }
                    else
                    {
                        reads = list[i].GetAttribute("title");
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(nick))
            {
                var uid = e.Uri.ToString().Replace("https://space.bilibili.com/", "").Trim();
                int.TryParse(fans.Replace(",", ""), out var fansNum);
                int.TryParse(friends.Replace(",", ""), out var friendsNum);
                int.TryParse(plays.Replace(",", ""), out var playsNum);
                int.TryParse(reads.Replace(",", ""), out var readsNum);
                var sevice = EngineContext.Current.Resolve<IMediaService>();
                sevice.Update(d => d.MediaID == uid && d.MediaType.CallIndex == "bilibili", m => new Media()
                {
                    MediaName = nick,
                    FansNum = fansNum,
                    FriendNum = friendsNum,
                    AvgReadNum = readsNum,
                    LikesNum = playsNum,
                    //Content = string.IsNullOrWhiteSpace(content) ? null : content.Trim(),
                    MediaLogo = image,
                    Content = string.IsNullOrWhiteSpace(abstr) ? null : abstr.Trim(),
                    MediaLink = e.Uri.ToString()
                });
            }

        }
        private void Crawler_OnError(object sender, OnErrorEventArgs e)
        {
            _logger.Error("B站用户工作任务爬取异常：" + e.Uri, e.Exception);
        }
    }
}