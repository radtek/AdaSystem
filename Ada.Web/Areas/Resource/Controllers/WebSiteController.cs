using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Resource;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Resource.Controllers
{
    /// <summary>
    /// 网站资源
    /// </summary>
    public class WebSiteController : BaseController
    {
        private readonly IMediaService _mediaService;
        private readonly IRepository<Media> _repository;
        private readonly IRepository<MediaTag> _mediaTagRepository;

        public WebSiteController(IMediaService mediaService,
            IRepository<Media> repository,

            IRepository<MediaTag> mediaTagRepository
   
        )
        {
            _mediaService = mediaService;
            _repository = repository;
          
            _mediaTagRepository = mediaTagRepository;
          
        }
        public ActionResult Index()
        {
            return View();
        }
        
      
        public ActionResult Import()
        {
            string path = Server.MapPath("~/upload/website.xlsx");
            int count = 0;
            using (FileStream ms =new FileStream(path, FileMode.Open))
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
                    media.MediaTypeId = "X1712271411590013";
                    media.LinkManId = linkid.Trim();
                    media.MediaName = row.GetCell(5)?.ToString();
                    media.Client = row.GetCell(6)?.ToString();
                    media.Channel = row.GetCell(7)?.ToString();
                    //校验ID不能重复
                    var temp = _repository.LoadEntities(d =>
                        d.MediaName.Equals(media.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                        d.Client.Equals(media.Client, StringComparison.CurrentCultureIgnoreCase) &&
                        d.Channel.Equals(media.Channel, StringComparison.CurrentCultureIgnoreCase) &&
                        d.IsDelete == false &&
                        d.MediaTypeId == media.MediaTypeId).FirstOrDefault();
                    if (temp != null)
                    {
                       continue;
                    }
                    //价格
                    MediaPrice price1 = new MediaPrice();
                    price1.Id = IdBuilder.CreateIdNum();
                    price1.AdPositionId = "X1712271412080018";
                    price1.AdPositionName = "普通入口";
                    price1.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(1)?.ToString(), out var pt1);
                    price1.PurchasePrice = pt1;
                    price1.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price1);

                    MediaPrice price2 = new MediaPrice();
                    price2.Id = IdBuilder.CreateIdNum();
                    price2.AdPositionId = "X1712271412080019";
                    price2.AdPositionName = "频道首页";
                    price2.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(2)?.ToString(), out var pt2);
                    price2.PurchasePrice = pt2;
                    price2.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price2);

                    MediaPrice price3 = new MediaPrice();
                    price3.Id = IdBuilder.CreateIdNum();
                    price3.AdPositionId = "X1712271412080020";
                    price3.AdPositionName = "首页文字链";
                    price3.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(3)?.ToString(), out var pt3);
                    price3.PurchasePrice = pt3;
                    price3.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price3);

                    MediaPrice price4 = new MediaPrice();
                    price4.Id = IdBuilder.CreateIdNum();
                    price4.AdPositionId = "X1712271412080021";
                    price4.AdPositionName = "首页焦点图";
                    price4.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(4)?.ToString(), out var pt4);
                    price4.PurchasePrice = pt4;
                    price4.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price4);
                    var tags = row.GetCell(8)?.ToString();
                    if (!string.IsNullOrWhiteSpace(tags))
                    {
                        var mediaTag = _mediaTagRepository.LoadEntities(d => d.IsDelete == false && d.TagName == tags)
                            .FirstOrDefault();
                        if (mediaTag != null)
                        {
                            media.MediaTags.Add(mediaTag);
                        }
                    }
                    media.MediaLink = row.GetCell(9)?.ToString();
                    media.Area = row.GetCell(10)?.ToString();
                    media.ResourceType = row.GetCell(11)?.ToString();
                    media.Efficiency = row.GetCell(12)?.ToString();
                    media.SEO = row.GetCell(13)?.ToString();
                    media.Remark = row.GetCell(14)?.ToString();
                    media.Content = row.GetCell(15)?.ToString();
                    media.Transactor = row.GetCell(16)?.ToString();
                    media.TransactorId = row.GetCell(17)?.ToString();
                    
                    media.Status = Consts.StateNormal;
                    _mediaService.Add(media);
                    count++;
                }
            }
            return Content("导入成功"+count+"条资源");
        }
    }
}