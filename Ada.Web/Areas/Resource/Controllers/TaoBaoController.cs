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
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Resource;
using Crawler.Models;
using Crawler.Services;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Resource.Controllers
{
    /// <summary>
    /// 知乎
    /// </summary>
    public class TaoBaoController : BaseController
    {
        private readonly IMediaService _mediaService;
        private readonly IRepository<Media> _repository;
        private readonly IRepository<LinkMan> _linkManRepository;
        private readonly IRepository<MediaTag> _mediaTagRepository;
        private readonly IWebCrawler _webCrawler;

        public TaoBaoController(IMediaService mediaService,
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
            string path = Server.MapPath("~/upload/taobao.xlsx");
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
                    media.MediaTypeId = "X1810170906012783";
                    media.LinkManId = linkid.Trim();
                    media.MediaName = row.GetCell(1)?.ToString();
                    media.MediaID = row.GetCell(2)?.ToString();
                    //校验ID不能重复
                    var temp = _repository.LoadEntities(d =>
                        d.MediaID.Equals(media.MediaID.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
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

                    media.MediaLink = "https://v.taobao.com/v/home/?userId=" + media.MediaID;
                    decimal.TryParse(row.GetCell(3)?.ToString(), out var fans);
                    media.FansNum = Utils.SetFansNum(fans);
                    //价格
                    MediaPrice price1 = new MediaPrice();
                    price1.Id = IdBuilder.CreateIdNum();
                    price1.AdPositionId = "X1810251617060016";
                    price1.AdPositionName = "淘宝头条视频";
                    price1.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(6)?.ToString(), out var pt1);
                    price1.PurchasePrice = pt1;
                    price1.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price1);

                    MediaPrice price2 = new MediaPrice();
                    price2.Id = IdBuilder.CreateIdNum();
                    price2.AdPositionId = "X1810251617060017";
                    price2.AdPositionName = "淘宝头条图文";
                    price2.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(7)?.ToString(), out var pt2);
                    price2.PurchasePrice = pt2;
                    price2.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price2);

                    MediaPrice price3 = new MediaPrice();
                    price3.Id = IdBuilder.CreateIdNum();
                    price3.AdPositionId = "X1810251617060018";
                    price3.AdPositionName = "微淘视频";
                    price3.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(8)?.ToString(), out var pt3);
                    price3.PurchasePrice = pt3;
                    price3.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price3);

                    MediaPrice price4 = new MediaPrice();
                    price4.Id = IdBuilder.CreateIdNum();
                    price4.AdPositionId = "X1810251617060019";
                    price4.AdPositionName = "微淘图文";
                    price4.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(9)?.ToString(), out var pt4);
                    price4.PurchasePrice = pt4;
                    price4.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price4);

                    MediaPrice price5 = new MediaPrice();
                    price5.Id = IdBuilder.CreateIdNum();
                    price5.AdPositionId = "X1810251617060020";
                    price5.AdPositionName = "有好货视频";
                    price5.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(10)?.ToString(), out var pt5);
                    price5.PurchasePrice = pt5;
                    price5.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price5);

                    MediaPrice price6 = new MediaPrice();
                    price6.Id = IdBuilder.CreateIdNum();
                    price6.AdPositionId = "X1810251617060021";
                    price6.AdPositionName = "有好货图文";
                    price6.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(11)?.ToString(), out var pt6);
                    price6.PurchasePrice = pt6;
                    price6.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price6);

                    MediaPrice price7 = new MediaPrice();
                    price7.Id = IdBuilder.CreateIdNum();
                    price7.AdPositionId = "X1810251617060022";
                    price7.AdPositionName = "专场直播";
                    price7.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(12)?.ToString(), out var pt7);
                    price7.PurchasePrice = pt7;
                    price7.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price7);

                    MediaPrice price8 = new MediaPrice();
                    price8.Id = IdBuilder.CreateIdNum();
                    price8.AdPositionId = "X1810251617060023";
                    price8.AdPositionName = "混场直播";
                    price8.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(13)?.ToString(), out var pt8);
                    price8.PurchasePrice = pt8;
                    price8.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price8);
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
                    media.Remark = row.GetCell(14)?.ToString();
                    media.Content = row.GetCell(15)?.ToString();

                    media.Transactor = row.GetCell(16)?.ToString();
                    media.TransactorId = row.GetCell(17)?.ToString();
                    media.Status = Consts.StateNormal;
                    media.AddedDate = DateTime.Now;
                    media.IsSlide = true;
                    _mediaService.Add(media);
                    count++;

                }
            }
            return Content("导入成功" + count + "条资源");
        }
        public ActionResult CrawlerUserInfo(string url)
        {
            _webCrawler.OnCompleted += Crawler_OnCompleted;
            _webCrawler.OnError += Crawler_OnError;
            _webCrawler.Start(new Uri(url));
            return Json(new { State = 1, Msg = "请求成功，请稍候刷新，查看结果。" }, JsonRequestBehavior.AllowGet);
        }
        private void Crawler_OnCompleted(object sender, OnCompletedEventArgs e)
        {
            var nick = _webCrawler.FindElementByXpath(e, "//h3[@class='nick']/span");
            var fans = _webCrawler.FindElementByXpath(e, "//div[@class='fans']/span[@class='nums']");
            var ability = _webCrawler.FindElementByXpath(e, "//div[@class='abilitynum']/span[@class='num']");
            var tags = _webCrawler.FindElementByXpath(e, "//div[@class='tags']/a[@class='tag']");
            var content = _webCrawler.FindElementByXpath(e, "//div[@class='IceEditorPreview v3vcom']");
            var imageStyle = _webCrawler.FindElementByClassName(e, "v3-userinfo-box", "style");
            var imageReg = Regex.Match(imageStyle,
                @"url\(""(.+)""\)");
            if (!string.IsNullOrWhiteSpace(nick))
            {
                var url = e.Uri.ToString();
                var fansNum = Utils.SetFansNum(decimal.Parse(fans.Trim().Replace("万", "")));
                var abilitynum = int.Parse(ability);
                var logo = imageReg.Groups[1].Value;
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
            Log.Error("爬取异常：" + e.Uri, e.Exception);
        }
    }
}