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
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Resource;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Resource.Controllers
{
    /// <summary>
    /// 头条
    /// </summary>
    public class HeadLineController : BaseController
    {
        private readonly IMediaService _mediaService;
        private readonly IRepository<Media> _repository;
        private readonly IRepository<LinkMan> _linkManRepository;
        private readonly IRepository<MediaTag> _mediaTagRepository;

        public HeadLineController(IMediaService mediaService,
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
            string path = Server.MapPath("~/upload/headline.xlsx");
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
                    media.MediaTypeId = "X1712180955530001";
                    media.LinkManId = linkid.Trim();
                    media.MediaName = row.GetCell(1)?.ToString();
                    media.MediaLink = row.GetCell(2)?.ToString();
                  
                    media.Platform = row.GetCell(3)?.ToString();
                    decimal.TryParse(row.GetCell(4)?.ToString(), out var fans);
                    media.FansNum = Utils.SetFansNum(fans);
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
                    price1.AdPositionId = "X1801181040250014";
                    price1.AdPositionName = "普通发布";
                    price1.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(7)?.ToString(), out var pt1);
                    price1.PurchasePrice = pt1;
                    price1.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price1);

                    MediaPrice price2 = new MediaPrice();
                    price2.Id = IdBuilder.CreateIdNum();
                    price2.AdPositionId = "X1801181040250015";
                    price2.AdPositionName = "智能推荐";
                    price2.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(8)?.ToString(), out var pt2);
                    price2.PurchasePrice = pt2;
                    price2.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price2);


                    var tags = row.GetCell(5)?.ToString();
                    if (!string.IsNullOrWhiteSpace(tags))
                    {
                        var mediaTag = _mediaTagRepository.LoadEntities(d => d.IsDelete == false && d.TagName == tags)
                            .FirstOrDefault();
                        if (mediaTag != null)
                        {
                            media.MediaTags.Add(mediaTag);
                        }
                    }

                    if (row.GetCell(6)?.ToString()=="是")
                    {
                        media.IsAuthenticate = true;
                    }
                    if (row.GetCell(6)?.ToString() == "否")
                    {
                        media.IsAuthenticate = false;
                    }
                    media.Remark = row.GetCell(9)?.ToString();
                    media.Content = row.GetCell(10)?.ToString();
                    
                    media.Transactor = row.GetCell(11)?.ToString();
                    media.TransactorId = row.GetCell(12)?.ToString();
                    media.Status = Consts.StateNormal;
                    media.AddedDate=DateTime.Now;
                    media.IsSlide = true;
                    _mediaService.Add(media);
                    count++;

                }
            }
            return Content("导入成功" + count + "条资源");
        }


    }
}