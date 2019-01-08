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
    public class TaoBaoInfoJob : IJob
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
                      d.IsDelete == false && d.MediaType.CallIndex == "taobao" && d.Status == Consts.StateNormal &&
                      (d.CollectionDate == null || SqlFunctions.DateDiff("hour", d.CollectionDate, DateTime.Now) > hour));
                if (media != null)
                {
                    media.CollectionDate = DateTime.Now;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(media.MediaLink))
                        {
                            _webCrawler.OnCompleted += Crawler_OnCompleted;
                            _webCrawler.OnError += Crawler_OnError;
                            _webCrawler.Start(new Uri(media.MediaLink));
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
                    if (job != null) job.Remark = "None:" + DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
        private void Crawler_OnCompleted(object sender, OnCompletedEventArgs e)
        {
            var nick = _webCrawler.FindElementByXpath(e, "//h3[@class='nick']/span");
            var fans = _webCrawler.FindElementByXpath(e, "//div[@class='fans']/span[@class='nums']");
            var ability = _webCrawler.FindElementByXpath(e, "//div[@class='abilitynum']/span[@class='num']");
            var tags = _webCrawler.FindElementByXpath(e, "//div[@class='tags']/a[@class='tag']");
            var content = _webCrawler.FindElementByXpath(e, "//div[@class='IceEditorPreview v3vcom']");
            var imageStyle = _webCrawler.FindElementByClassName(e, "v3-userinfo-box", "style");
            string imgLogo = null;
            if (!string.IsNullOrWhiteSpace(imageStyle))
            {
                imgLogo= Regex.Match(imageStyle,
                    @"url\(""(.+)""\)").Groups[1].Value;
            }

            if (!string.IsNullOrWhiteSpace(nick))
            {
                var url = e.Uri.ToString();
                var fansNum = fans.Contains("万") ? Utils.SetFansNum(decimal.Parse(fans.Trim().Replace("万", ""))) : int.Parse(fans);
                var abilitynum = int.Parse(ability);
                var logo = imgLogo;
                var sevice = EngineContext.Current.Resolve<IMediaService>();
                sevice.Update(d => d.MediaLink == url, m => new Media()
                {
                    MediaName = nick,
                    FansNum = fansNum,
                    AvgReadNum = abilitynum,
                    Content = content,
                    MediaLogo = logo,
                    Abstract = tags
                });
            }

        }
        private void Crawler_OnError(object sender, OnErrorEventArgs e)
        {
            _logger.Error("淘宝用户工作任务爬取异常：" + e.Uri, e.Exception);
        }
    }
}