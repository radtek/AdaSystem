using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Framework.UploadFile;
using Ada.Services.Resource;
using ClosedXML.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Resource.Controllers
{
    public class UpdateController : BaseController
    {
        private readonly IMediaService _mediaService;
        private readonly IRepository<MediaPrice> _mediaPriceRepository;
        private readonly IDbContext _dbContext;
        public UpdateController(IMediaService mediaService, IRepository<MediaPrice> mediaPriceRepository, IDbContext dbContext)
        {
            _mediaService = mediaService;
            _mediaPriceRepository = mediaPriceRepository;
            _dbContext = dbContext;
        }

        public ActionResult Index()
        {
            MediaView viewModel = new MediaView();
            viewModel.limit = 100;
            return View(viewModel);
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Export(MediaView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _mediaService.LoadEntitiesFilter(viewModel).ToList();
            if (!result.Any())
            {
                ModelState.AddModelError("message", "没有查询到相关媒体信息！");
                return View("Index", viewModel);
            }
            //找到没有的
            if (!string.IsNullOrWhiteSpace(viewModel.MediaNames))
            {
                var names = viewModel.MediaNames.Split(',').ToList();
                foreach (var name in names)
                {
                    if (result.All(d => d.MediaName != name))
                    {
                        result.Add(new Media
                        {
                            MediaName = name
                        });
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaIDs))
            {
                var ids = viewModel.MediaIDs.Split(',').ToList();
                foreach (var id in ids)
                {
                    if (result.All(d => d.MediaID != id))
                    {
                        result.Add(new Media
                        {
                            MediaID = id
                        });
                    }
                }
            }
            JArray jObjects = new JArray();
            foreach (var media in result)
            {
                var jo = new JObject();
                jo.Add("Id", media.Id ?? "不存在的资源");
                jo.Add("结算人", media.LinkMan?.Name);
                jo.Add("媒体类型", media.MediaType?.TypeName);
                jo.Add("平台", media.Platform);
                jo.Add("媒体名称", media.MediaName);
                jo.Add("媒体ID", media.MediaID);
                jo.Add("粉丝数", media.FansNum ?? 0);
                foreach (var mediaMediaPrice in media.MediaPrices)
                {
                    jo.Add(mediaMediaPrice.AdPositionName, mediaMediaPrice.PurchasePrice);
                    //jo.Add(mediaMediaPrice.AdPositionName + "更新日期", mediaMediaPrice.PriceDate);
                    //jo.Add(mediaMediaPrice.AdPositionName + "失效日期", mediaMediaPrice.InvalidDate);
                }
                jObjects.Add(jo);
            }

            var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());
            byte[] bytes;
            using (var workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dt, "江西微广");
                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    bytes = ms.ToArray();
                }
            }
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
        }
        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Import()
        {
            UEditorModel uploadConfig = new UEditorModel()
            {
                AllowExtensions = UEditorConfig.GetStringList("fileAllowFiles"),
                PathFormat = UEditorConfig.GetString("filePathFormat"),
                SizeLimit = UEditorConfig.GetInt("fileMaxSize"),
                UploadFieldName = UEditorConfig.GetString("fileFieldName")
            };
            var file = Request.Files[uploadConfig.UploadFieldName];
            if (file == null)
            {
                return Json(new { State = 0, Msg = "请选择要导入的文件" });
            }
            var uploadFileName = file.FileName;
            var fileExtension = Path.GetExtension(uploadFileName).ToLower();
            if (!uploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension))
            {
                return Json(new { State = 0, Msg = "文件类型不匹配" });
            }
            if (!(file.ContentLength < uploadConfig.SizeLimit))
            {
                return Json(new { State = 0, Msg = "上传的文件最大只能为：" + uploadConfig.SizeLimit + "B" });
            }
            //创建工作薄
            IWorkbook wk = new XSSFWorkbook(file.InputStream);
            //1.获取第一个工作表
            ISheet sheet = wk.GetSheetAt(0);
            if (sheet.LastRowNum <= 1)
            {
                return Json(new { State = 0, Msg = "此文件没有导入数据，请填充数据再进行导入" });
            }
            //拿到广告位的名称
            IRow headRow = sheet.GetRow(0);
            List<string> adpostionNames = new List<string>();
            int startPrice = 7;//价格所在位置
            for (int i = startPrice; i < headRow.LastCellNum; i++)
            {
                var adpostionName = headRow.GetCell(i).StringCellValue;
                adpostionNames.Add(adpostionName);
            }
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                var id = row.GetCell(0).StringCellValue;
                if (string.IsNullOrWhiteSpace(id) || id == "不存在的资源")
                {
                    continue;
                }
                for (int j = 0; j < adpostionNames.Count; j++)
                {

                    var name = adpostionNames[j];
                    var mediaPrice = _mediaPriceRepository
                        .LoadEntities(d => d.MediaId == id && d.AdPositionName == name).FirstOrDefault();
                    if (mediaPrice == null) continue;
                    decimal.TryParse(row.GetCell(startPrice + j).ToString(), out var price);
                    mediaPrice.PurchasePrice = price;
                    mediaPrice.PriceDate = DateTime.Now;
                    mediaPrice.InvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    //修改粉丝
                    int.TryParse(row.GetCell(startPrice - 1).ToString(), out var fansNum);
                    mediaPrice.Media.FansNum = fansNum;
                }
            }
            _dbContext.SaveChanges();
            return Json(new { State = 1, Msg = "导入成功" });
        }
    }
}