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
    public class WeiXinBizJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IWebCrawler _webCrawler = EngineContext.Current.Resolve<IWebCrawler>();
        private string _currentWx;
        private const string None = "<p class=\"p1\">呀！</p>";

        public void Execute(IJobExecutionContext context)
        {
            using (var db = new AdaEFDbcontext())
            {
                var name = context.JobDetail.Key.Name;
                var group = context.JobDetail.Key.Group;
                var job = db.Set<Job>().FirstOrDefault(d => d.GroupName == group && d.JobName == name);
                int hour = 66;
                if (job != null)
                {
                    hour = job.Taxis ?? 66;
                }
                var media = db.Set<Media>().FirstOrDefault(d =>
                      d.IsDelete == false && d.MediaType.CallIndex == "weixin" && d.Status == Consts.StateNormal && (d.MediaLink == null || d.MediaLink == "") &&
                      (d.ApiUpDate == null || SqlFunctions.DateDiff("hour", d.ApiUpDate, DateTime.Now) > hour));
                if (media != null)
                {
                    media.ApiUpDate = DateTime.Now;
                    _currentWx = media.Id;
                    try
                    {
                        _webCrawler.OnCompleted += Crawler_OnCompleted;
                        _webCrawler.OnError += Crawler_OnError;
                        var operation = new Operation();
                        operation.Timeout = 60000;
                        operation.WebAction = d =>
                        {
                            if (d.PageSource.Contains(None)) return;
                            var ul = d.FindElement(By.ClassName("news-list2"));
                            var lis = ul.FindElements(By.TagName("li"));
                            foreach (var li in lis)
                            {
                                var info = li.FindElement(By.XPath("//p[@class='info']/label")).Text;
                                if (!string.Equals(info, media.MediaID, StringComparison.CurrentCultureIgnoreCase)) continue;
                                var next = li.FindElement(By.XPath("//p[@class='tit']/a")).GetAttribute("href");
                                d.Navigate().GoToUrl(next);
                                break;
                            }

                        };
                        _webCrawler.Start(new Uri("https://weixin.sogou.com/weixin?type=1&query=" + media.MediaID), operation);
                        //改变工作计划时间
                        if (context.NextFireTimeUtc != null)
                        {
                            if (job != null)
                            {
                                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
                                job.Remark = "获取微信BIZ信息任务正在运行中，本次成功更新：" + media.MediaName + "-" + media.MediaID;
                            }
                        }

                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("获取【" + media.MediaName + "-" + media.MediaID + "】微信BIZ信息任务异常", ex);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (job != null) job.Remark = "获取微信BIZ信息任务暂无可更新的资源数据！更新时间：" + DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
        private void Crawler_OnCompleted(object sender, OnCompletedEventArgs e)
        {
            string biz = string.Empty;
            if (!e.PageSource.Contains(None))
            {
                biz = Regex.Match(e.PageSource, @"var biz = ""(.+?)""").Groups[1].Value;
            }

            if (string.IsNullOrWhiteSpace(biz)) return;
            if (string.IsNullOrWhiteSpace(_currentWx)) return;
            var sevice = EngineContext.Current.Resolve<IMediaService>();
            sevice.Update(d => d.Id == _currentWx, m => new Media()
            {
                MediaLink = biz
            });


        }
        private void Crawler_OnError(object sender, OnErrorEventArgs e)
        {
            _logger.Error("微信BIZ工作任务爬取异常：" + e.Uri, e.Exception);
        }
    }
}