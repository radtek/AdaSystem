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
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Resource.Controllers
{
    /// <summary>
    /// 小红书
    /// </summary>
    public class RedBookController : BaseController
    {
        private readonly IMediaService _mediaService;
        private readonly IRepository<Media> _repository;
        private readonly IRepository<LinkMan> _linkManRepository;
        private readonly IRepository<MediaTag> _mediaTagRepository;
        private readonly IWebCrawler _webCrawler;
        public RedBookController(IMediaService mediaService,
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
            string path = Server.MapPath("~/upload/redbook.xlsx");
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
                    media.MediaTypeId = "X1712181059130008";
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
                    price1.AdPositionId = "X1805281009515493";
                    price1.AdPositionName = "普通笔记";
                    price1.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(7)?.ToString(), out var pt1);
                    price1.PurchasePrice = pt1;
                    price1.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price1);

                    MediaPrice price2 = new MediaPrice();
                    price2.Id = IdBuilder.CreateIdNum();
                    price2.AdPositionId = "X1805281009515495";
                    price2.AdPositionName = "代投";
                    price2.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(8)?.ToString(), out var pt2);
                    price2.PurchasePrice = pt2;
                    price2.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price2);

                    MediaPrice price3 = new MediaPrice();
                    price3.Id = IdBuilder.CreateIdNum();
                    price3.AdPositionId = "X1805281009515494";
                    price3.AdPositionName = "视频";
                    price3.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(6)?.ToString(), out var pt3);
                    price3.PurchasePrice = pt3;
                    price3.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price3);

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
                    media.Remark = row.GetCell(9)?.ToString();
                    media.Transactor = row.GetCell(10)?.ToString();
                    media.TransactorId = row.GetCell(11)?.ToString();
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
            _webCrawler.Start(new Uri("https://www.xiaohongshu.com/user/profile/" + media.MediaID),
                new Operation() { Timeout = 5000, Condition = d => d.PageSource.Contains("UserDetail") });
            return Json(new { State = 1, Msg = "请求成功，请稍候刷新，查看结果。" }, JsonRequestBehavior.AllowGet);
        }
        private void Crawler_OnCompleted(object sender, OnCompletedEventArgs e)
        {
            var jsonStr = Regex.Match(e.PageSource, @"{""UserDetail"":(.+),""notesDetail""").Groups[1].Value;
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
        private void Crawler_OnError(object sender, OnErrorEventArgs e)
        {
            Log.Error("爬取异常：" + e.Uri, e.Exception);
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