using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;
using Ada.Services.Setting;
using Crawler.Models;
using Crawler.Services;
using log4net;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace Crawler.Controllers
{
    public class MediaSetController : BaseController
    {
        private readonly ISettingService _settingService;
        public MediaSetController(ISettingService settingService)
        {
            _settingService = settingService;
        }
        public ActionResult Index()
        {
            var entity = _settingService.GetSetting<MediaCrawlerSet>();
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken,ValidateInput(false)]
        public ActionResult Index(MediaCrawlerSet entity)
        {
            var setting = new Ada.Core.Domain.Admin.Setting
            {
                SettingName = typeof(MediaCrawlerSet).Name,
                Content = JsonConvert.SerializeObject(entity)
            };
            _settingService.AddOrUpdate(setting);
            TempData["Msg"] = "保存成功";
            return View(entity);

        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public async Task<ActionResult> TestBlog(string url)
        {
            var config = _settingService.GetSetting<MediaCrawlerSet>();
            var webCrawler = new WebCrawler("spider", "103.85.162.58:53281");
            webCrawler.OnStart += Crawler_OnStart;
            webCrawler.OnError += Crawler_OnError;
            webCrawler.OnCompleted += (o, e) =>
            {
                string like, comment, collction;
                if (url.Contains("https://m.weibo.cn"))
                {
                    like = Regex.Match(e.PageSource, @"attitudes_count.+\s+(\d+),").Groups[1].Value;
                    comment = Regex.Match(e.PageSource, @"comments_count.+\s+(\d+),").Groups[1].Value;
                    collction = Regex.Match(e.PageSource, @"reposts_count.+\s+(\d+),").Groups[1].Value;
                }
                else
                {
                    var likeReg = Regex.Match(e.PageSource, config.BlogLikeReg);
                    like = likeReg.Groups[1].Value;
                    //评论数
                    var commentReg = Regex.Match(e.PageSource, config.BlogCommentReg);
                    comment = commentReg.Groups[1].Value;
                    //转发
                    var collctionReg = Regex.Match(e.PageSource, config.BlogRelayReg);
                    collction = collctionReg.Groups[1].Value;
                }
                
                Log.Info(JsonConvert.SerializeObject(new
                {
                    页面标题 = e.WebDriver.Title,
                    点赞数 = like,
                    评论数 = comment,
                    转发数 = collction,
                    用时 = e.Milliseconds,
                    爬虫网址 = e.Uri.ToString(),

                }));
            };
            await webCrawler.Start(new Uri(url), null, new Operation() { Timeout = 5000 });
            return Json(new { State = 1, Msg = "提交成功" });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public async Task<ActionResult> TestRedbook(string url)
        {
            var webCrawler = new WebCrawler();
            webCrawler.OnStart += Crawler_OnStart;
            webCrawler.OnError += Crawler_OnError;
            webCrawler.OnCompleted += RedBook_OnCompleted;
            await webCrawler.Start(new Uri(url), null, new Operation() { Timeout = 5000 });
            return Json(new { State = 1, Msg = "提交成功" });
        }

        private void Blog_OnCompleted(object sender, OnCompletedEventArgs e)
        {
            
            
        }
        private void RedBook_OnCompleted(object sender, OnCompletedEventArgs e)
        {
            var config = _settingService.GetSetting<MediaCrawlerSet>();
            var like = e.WebDriver
                .FindElement(By.XPath(
                    config.RedbookLikeReg))
                .Text;
            //评论数
            var comment = e.WebDriver.FindElement(By.XPath(config.RedbookCommentReg)).Text;
            //收藏数
            var collction = e.WebDriver.FindElement(By.XPath(config.RedbookCollectionReg)).Text;
            Log.Info(JsonConvert.SerializeObject(new
            {
                页面标题 = e.WebDriver.Title,
                点赞数 = like,
                评论数 = comment,
                收藏数 = collction,
                用时 = e.Milliseconds,
                爬虫网址 = e.Uri.ToString(),

            }));
        }
        private void Crawler_OnError(object sender, OnErrorEventArgs e)
        {
            Log.Error("爬虫异常：" + e.Uri, e.Exception);
        }

        private void Crawler_OnStart(object sender, OnStartEventArgs e)
        {
            Log.Info("爬虫开始：" + e.Uri);
        }
    }
}