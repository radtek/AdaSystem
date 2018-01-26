using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Resource;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Resource.Controllers
{
    public class WeiXinController : BaseController
    {
        private readonly IMediaService _mediaService;
        private readonly IRepository<Media> _repository;
        private readonly IRepository<MediaTag> _mediaTagRepository;
        
        public WeiXinController(IMediaService mediaService,
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
            string path = Server.MapPath("~/upload/weixin.xlsx");
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
                    var linkid = row.GetCell(9)?.ToString();
                    if (string.IsNullOrWhiteSpace(linkid))
                    {
                        continue;
                    }
                    Media media = new Media();
                    media.Id = IdBuilder.CreateIdNum();
                    media.MediaTypeId = "X1711091747220001";
                    media.LinkManId = linkid;
                    media.MediaName = row.GetCell(0)?.ToString();
                    media.MediaID = row.GetCell(1)?.ToString();
                    decimal.TryParse(row.GetCell(2)?.ToString(), out var fans);
                    media.FansNum = Utils.SetFansNum(fans);
                    //校验ID不能重复
                    var temp = _repository.LoadEntities(d =>
                        d.MediaID.Equals(media.MediaID.Trim(), StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                        d.MediaTypeId == media.MediaTypeId).FirstOrDefault();
                    if (temp != null)
                    {
                        continue;
                    }
                    //价格
                    MediaPrice price1 = new MediaPrice();
                    price1.Id = IdBuilder.CreateIdNum();
                    price1.AdPositionId = "X1712191029260002";
                    price1.AdPositionName = "头条";
                    price1.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(3)?.ToString(), out var pt1);
                    price1.PurchasePrice = pt1;
                    price1.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price1);

                    MediaPrice price2 = new MediaPrice();
                    price2.Id = IdBuilder.CreateIdNum();
                    price2.AdPositionId = "X1712191029260003";
                    price2.AdPositionName = "头条（原创）";
                    price2.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(4)?.ToString(), out var pt2);
                    price2.PurchasePrice = pt2;
                    price2.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price2);

                    MediaPrice price3 = new MediaPrice();
                    price3.Id = IdBuilder.CreateIdNum();
                    price3.AdPositionId = "X1712191029260004";
                    price3.AdPositionName = "二条";
                    price3.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(5)?.ToString(), out var pt3);
                    price3.PurchasePrice = pt3;
                    price3.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price3);

                    MediaPrice price4 = new MediaPrice();
                    price4.Id = IdBuilder.CreateIdNum();
                    price4.AdPositionId = "X1712191029260005";
                    price4.AdPositionName = "二条（原创）";
                    price4.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(6)?.ToString(), out var pt4);
                    price4.PurchasePrice = pt4;
                    price4.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price4);
                    var tags = row.GetCell(7)?.ToString();
                    if (!string.IsNullOrWhiteSpace(tags))
                    {
                        var mediaTag = _mediaTagRepository.LoadEntities(d => d.IsDelete == false && d.TagName == tags)
                            .FirstOrDefault();
                        if (mediaTag != null)
                        {
                            media.MediaTags.Add(mediaTag);
                        }
                    }
                   
                    media.Remark = row.GetCell(8)?.ToString();
                    media.Transactor = row.GetCell(10)?.ToString();
                    media.TransactorId = row.GetCell(11)?.ToString();
                    media.Status = Consts.StateNormal;
                    _mediaService.Add(media);
                    count++;
                }
            }
            return Content("导入成功" + count + "条资源");
        }
    }
}