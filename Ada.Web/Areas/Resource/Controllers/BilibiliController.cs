using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Customer;
using Ada.Core.Domain.Resource;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Framework.Filter;
using Ada.Services.Resource;
using Crawler.Models;
using Crawler.Services;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OpenQA.Selenium;

namespace Resource.Controllers
{
    /// <summary>
    /// B站
    /// </summary>
    public class BilibiliController : BaseController
    {
        private readonly IMediaService _mediaService;
        private readonly IRepository<Media> _repository;
        private readonly IRepository<LinkMan> _linkManRepository;
        private readonly IRepository<MediaTag> _mediaTagRepository;
        private readonly IWebCrawler _webCrawler;
        public BilibiliController(IMediaService mediaService,
            IRepository<Media> repository,
            IRepository<LinkMan> linkManRepository,
            IRepository<MediaTag> mediaTagRepository,
            IWebCrawler webCrawler

        )
        {
            _mediaService = mediaService;
            _repository = repository;
            _linkManRepository = linkManRepository;
            _mediaTagRepository = mediaTagRepository;
            _webCrawler = webCrawler;

        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Import()
        {
            string path = Server.MapPath("~/upload/bilibili.xlsx");
            int count = 0;
            using (FileStream ms = new FileStream(path, FileMode.Open))
            {
                //创建工作薄
                IWorkbook wk = new XSSFWorkbook(ms);
                //1.获取第一个工作表
                ISheet sheet = wk.GetSheetAt(0);
                if (sheet.LastRowNum <= 1)
                {
                    return Content("此文件没有导入数据，请填充数据再进行导入");
                }

                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    var linkid = row.GetCell(0)?.ToString();
                    if (string.IsNullOrWhiteSpace(linkid))
                    {
                        continue;
                    }
                    Media media = new Media();
                    media.Id = IdBuilder.CreateIdNum();
                    media.MediaTypeId = "X1901070950142728";
                    media.LinkManId = linkid.Trim();
                    media.MediaName = row.GetCell(1)?.ToString();
                    media.MediaID = row.GetCell(2)?.ToString();
                    //校验ID不能重复
                    var temp = _repository.LoadEntities(d =>
                        d.MediaID == media.MediaID &&
                        d.IsDelete == false &&
                        d.MediaTypeId == media.MediaTypeId).FirstOrDefault();
                    if (temp != null)
                    {
                        continue;
                    }

                    if (_linkManRepository.LoadEntities(d => d.Id == linkid).FirstOrDefault() == null)
                    {
                        continue;
                    }

                    decimal.TryParse(row.GetCell(3)?.ToString(), out var fans);
                    media.FansNum = Utils.SetFansNum(fans);
                    //价格
                    MediaPrice price1 = new MediaPrice();
                    price1.Id = IdBuilder.CreateIdNum();
                    price1.AdPositionId = "X1901070950142729";
                    price1.AdPositionName = "软植入视频";
                    price1.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(6)?.ToString(), out var pt1);
                    price1.PurchasePrice = pt1;
                    price1.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price1);

                    MediaPrice price2 = new MediaPrice();
                    price2.Id = IdBuilder.CreateIdNum();
                    price2.AdPositionId = "X1901070950142730";
                    price2.AdPositionName = "定制视频";
                    price2.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(7)?.ToString(), out var pt2);
                    price2.PurchasePrice = pt2;
                    price2.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price2);

                    MediaPrice price3 = new MediaPrice();
                    price3.Id = IdBuilder.CreateIdNum();
                    price3.AdPositionId = "X1901070950142731";
                    price3.AdPositionName = "代投视频";
                    price3.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(8)?.ToString(), out var pt3);
                    price3.PurchasePrice = pt3;
                    price3.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price3);

                    MediaPrice price4 = new MediaPrice();
                    price4.Id = IdBuilder.CreateIdNum();
                    price4.AdPositionId = "X1901070950142732";
                    price4.AdPositionName = "线上直播";
                    price4.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(9)?.ToString(), out var pt4);
                    price4.PurchasePrice = pt4;
                    price4.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price4);

                    MediaPrice price5 = new MediaPrice();
                    price5.Id = IdBuilder.CreateIdNum();
                    price5.AdPositionId = "X1901070950142733";
                    price5.AdPositionName = "动态转发";
                    price5.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(10)?.ToString(), out var pt5);
                    price5.PurchasePrice = pt5;
                    price5.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price5);

                    var tags = row.GetCell(5)?.ToString();
                    if (!string.IsNullOrWhiteSpace(tags))
                    {
                        var arr = tags.Trim().Replace("，", ",").Split(',').ToList();
                        var mediaTag =
                            _mediaTagRepository.LoadEntities(d => d.IsDelete == false && arr.Contains(d.TagName));
                        if (mediaTag.Any())
                        {
                            foreach (var tag in mediaTag)
                            {
                                media.MediaTags.Add(tag);
                            }
                        }
                    }
                    media.Area = row.GetCell(4)?.ToString();
                    media.Remark = row.GetCell(11)?.ToString();
                    media.Transactor = row.GetCell(12)?.ToString();
                    media.TransactorId = row.GetCell(13)?.ToString();
                    media.Status = Consts.StateNormal;
                    media.IsSlide = true;
                    media.AddedDate = DateTime.Now;
                    _mediaService.Add(media);
                    count++;

                }
            }
            return Content("导入成功" + count + "条资源");
        }

        public ActionResult CrawlerUserInfo(string id)
        {
            var media = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _webCrawler.OnCompleted += Crawler_OnCompleted;
            _webCrawler.OnError += Crawler_OnError;
            _webCrawler.Start(new Uri("https://space.bilibili.com/" + media.MediaID),
                new Operation() { Timeout = 5000, Condition = d => d.FindElements(By.XPath("//div[@id='navigator']")).Any() });
            return Json(new { State = 1, Msg = "请求成功，请稍候刷新，查看结果。" }, JsonRequestBehavior.AllowGet);
        }
        private void Crawler_OnCompleted(object sender, OnCompletedEventArgs e)
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
                var uid = e.Uri.ToString().Replace("https://space.bilibili.com/","").Trim();
                int.TryParse(fans.Replace(",", ""), out var fansNum);
                int.TryParse(friends.Replace(",", ""), out var friendsNum);
                int.TryParse(plays.Replace(",", ""), out var playsNum);
                int.TryParse(reads.Replace(",", ""), out var readsNum);
                var sevice = EngineContext.Current.Resolve<IMediaService>();
                sevice.Update(d => d.MediaID == uid&&d.MediaType.CallIndex== "bilibili", m => new Media()
                {
                    MediaName = nick,
                    FansNum = fansNum,
                    FriendNum = friendsNum,
                    AvgReadNum = readsNum,
                    LikesNum = playsNum,
                    //Content = string.IsNullOrWhiteSpace(content)?null:content.Trim(),
                    MediaLogo = image,
                    Content = string.IsNullOrWhiteSpace(abstr) ? null : abstr.Trim(),
                    MediaLink = e.Uri.ToString()
                });
            }

        }
        private void Crawler_OnError(object sender, OnErrorEventArgs e)
        {
            Log.Error("爬取异常：" + e.Uri, e.Exception);
        }
    }
    
}