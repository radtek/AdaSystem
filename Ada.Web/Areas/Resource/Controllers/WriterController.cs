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
    public class WriterController : BaseController
    {
        private readonly IMediaService _mediaService;
        private readonly IRepository<Media> _repository;


        public WriterController(IMediaService mediaService,
            IRepository<Media> repository
   
        )
        {
            _mediaService = mediaService;
            _repository = repository;
          
        }
        public ActionResult Index()
        {
            return View();
        }
        
      
        public ActionResult Import()
        {
            string path = Server.MapPath("~/upload/writer.xlsx");
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
                    media.MediaTypeId = "X1808010944567024";
                    media.LinkManId = linkid.Trim();
                    media.MediaName = row.GetCell(5)?.ToString();
                    //校验ID不能重复
                    var temp = _repository.LoadEntities(d =>
                        d.MediaName.Equals(media.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                        d.IsDelete == false &&
                        d.MediaTypeId == media.MediaTypeId).FirstOrDefault();
                    if (temp != null)
                    {
                       continue;
                    }
                    //价格
                    MediaPrice price1 = new MediaPrice();
                    price1.Id = IdBuilder.CreateIdNum();
                    price1.AdPositionId = "X1808010944567025";
                    price1.AdPositionName = "写稿价格";
                    price1.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    decimal.TryParse(row.GetCell(1)?.ToString(), out var pt1);
                    price1.PurchasePrice = pt1;
                    price1.PriceDate = DateTime.Now;
                    media.MediaPrices.Add(price1);
                    media.ResourceType = row.GetCell(11)?.ToString();
                    media.Efficiency = row.GetCell(12)?.ToString();
                    media.Content = row.GetCell(15)?.ToString();
                    media.Transactor = row.GetCell(16)?.ToString();
                    media.TransactorId = row.GetCell(17)?.ToString();
                    media.AddedDate=DateTime.Now;
                    media.Status = Consts.StateNormal;
                    media.IsSlide = true;
                    _mediaService.Add(media);
                    count++;
                }
            }
            return Content("导入成功"+count+"条资源");
        }
    }
}