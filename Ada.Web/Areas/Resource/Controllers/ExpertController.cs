using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Customer;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Framework.Filter;
using Ada.Services.Resource;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Resource.Controllers
{
    public class ExpertController : BaseController
    {
        private readonly IMediaService _mediaService;
        private readonly IRepository<Media> _repository;
        private readonly IRepository<LinkMan> _linkManRepository;
        private readonly IRepository<MediaTag> _mediaTagRepository;

        public ExpertController(IMediaService mediaService,
            IRepository<Media> repository,
            IRepository<LinkMan> linkManRepository,
            IRepository<MediaTag> mediaTagRepository

        )
        {
            _mediaService = mediaService;
            _repository = repository;
            _linkManRepository = linkManRepository;
            _mediaTagRepository = mediaTagRepository;

        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Import()
        {
            string path = Server.MapPath("~/upload/expert.xlsx");
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
                    media.MediaTypeId = "X1904190915292236";
                    media.LinkManId = linkid.Trim();
                    media.MediaName = row.GetCell(1)?.ToString();
                    media.Sex = row.GetCell(2)?.ToString();
                    //校验ID不能重复
                    var temp = _repository.LoadEntities(d =>
                        d.MediaName.Equals(media.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                        d.Platform.Equals(media.Platform, StringComparison.CurrentCultureIgnoreCase) &&
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
                    //价格
                    MediaPrice price1 = new MediaPrice();
                    price1.Id = IdBuilder.CreateIdNum();
                    price1.AdPositionId = "X1904190915292237";
                    price1.AdPositionName = "微信本地线下";
                    price1.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(5)?.ToString(), out var pt1);
                    price1.PurchasePrice = pt1;
                    price1.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price1);

                    MediaPrice price2 = new MediaPrice();
                    price2.Id = IdBuilder.CreateIdNum();
                    price2.AdPositionId = "X1904190915292238";
                    price2.AdPositionName = "微信外地线下";
                    price2.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(6)?.ToString(), out var pt2);
                    price2.PurchasePrice = pt2;
                    price2.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price2);

                    MediaPrice price3 = new MediaPrice();
                    price3.Id = IdBuilder.CreateIdNum();
                    price3.AdPositionId = "X1904190915292239";
                    price3.AdPositionName = "微博本地线下";
                    price3.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(7)?.ToString(), out var pt3);
                    price3.PurchasePrice = pt3;
                    price3.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price3);

                    MediaPrice price4 = new MediaPrice();
                    price4.Id = IdBuilder.CreateIdNum();
                    price4.AdPositionId = "X1904190915292240";
                    price4.AdPositionName = "微博外地线下";
                    price4.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(8)?.ToString(), out var pt4);
                    price4.PurchasePrice = pt4;
                    price4.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price4);

                    MediaPrice price5 = new MediaPrice();
                    price5.Id = IdBuilder.CreateIdNum();
                    price5.AdPositionId = "X1904190915292241";
                    price5.AdPositionName = "抖音本地线下";
                    price5.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(9)?.ToString(), out var pt5);
                    price5.PurchasePrice = pt5;
                    price5.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price5);

                    MediaPrice price6 = new MediaPrice();
                    price6.Id = IdBuilder.CreateIdNum();
                    price6.AdPositionId = "X1904190915292242";
                    price6.AdPositionName = "抖音外地线下";
                    price6.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(10)?.ToString(), out var pt6);
                    price6.PurchasePrice = pt6;
                    price6.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price6);

                    MediaPrice price7 = new MediaPrice();
                    price7.Id = IdBuilder.CreateIdNum();
                    price7.AdPositionId = "X1904191004530343";
                    price7.AdPositionName = "B站本地线下";
                    price7.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(11)?.ToString(), out var pt7);
                    price7.PurchasePrice = pt7;
                    price7.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price7);

                    MediaPrice price8 = new MediaPrice();
                    price8.Id = IdBuilder.CreateIdNum();
                    price8.AdPositionId = "X1904191004530344";
                    price8.AdPositionName = "B站外地线下";
                    price8.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(12)?.ToString(), out var pt8);
                    price8.PurchasePrice = pt8;
                    price8.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price8);

                    MediaPrice price9 = new MediaPrice();
                    price9.Id = IdBuilder.CreateIdNum();
                    price9.AdPositionId = "X1904190915292244";
                    price9.AdPositionName = "直播本地线下";
                    price9.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(13)?.ToString(), out var pt9);
                    price9.PurchasePrice = pt9;
                    price9.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price9);

                    MediaPrice price10 = new MediaPrice();
                    price10.Id = IdBuilder.CreateIdNum();
                    price10.AdPositionId = "X1904190915292244";
                    price10.AdPositionName = "直播外地线下";
                    price10.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(14)?.ToString(), out var pt10);
                    price10.PurchasePrice = pt10;
                    price10.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price10);

                    var tags = row.GetCell(4)?.ToString();
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

                    media.Abstract = row.GetCell(15)?.ToString();
                    media.Content = row.GetCell(16)?.ToString();
                    media.Remark = row.GetCell(17)?.ToString();
                    media.Area = row.GetCell(3)?.ToString();
                    media.Transactor = row.GetCell(18)?.ToString();
                    media.TransactorId = row.GetCell(19)?.ToString();
                    media.AddedDate = DateTime.Now;
                    media.Status = Consts.StateNormal;
                    media.IsSlide = true;
                    _mediaService.Add(media);
                    count++;

                }
            }
            return Content("导入成功" + count + "条资源");
        }
    }
}