using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Customer;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Resource;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Filter;
using Ada.Framework.UploadFile;
using Ada.Services.Resource;
using Ada.Services.Setting;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Resource.Controllers
{
    public class MediaController : BaseController
    {
        private readonly IMediaPriceService _mediaPriceService;
        private readonly IMediaService _mediaService;
        private readonly ISettingService _settingService;
        private readonly IRepository<Media> _repository;
        private readonly IRepository<MediaType> _mediaTypeRepository;
        private readonly IRepository<MediaPrice> _mediaPriceRepository;
        private readonly IRepository<MediaTag> _mediaTagRepository;
        private readonly IDbContext _dbContext;
        public MediaController(IMediaPriceService mediaPriceService,
            IMediaService mediaService,
            IRepository<MediaPrice> mediaPriceRepository,
            IDbContext dbContext,
            IRepository<Media> repository,
            IRepository<MediaTag> mediaTagRepository,
            IRepository<MediaType> mediaTypeRepository,
            ISettingService settingService)
        {
            _mediaPriceService = mediaPriceService;
            _mediaService = mediaService;
            _mediaPriceRepository = mediaPriceRepository;
            _dbContext = dbContext;
            _repository = repository;
            _mediaTagRepository = mediaTagRepository;
            _mediaTypeRepository = mediaTypeRepository;
            _settingService = settingService;
        }

        public ActionResult Index()
        {
            MediaView media = new MediaView();
            return View(media);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MediaView viewModel)
        {
            var export = Request.Form["Submit.Export"];
            if (string.IsNullOrWhiteSpace(viewModel.MediaTypeId))
            {
                ModelState.AddModelError("message", "请先选择媒体类型！");
                return View(viewModel);
            }
            viewModel.Managers = PremissionData();
            var setting = _settingService.GetSetting<WeiGuang>();
            if (export == "export")
            {
                viewModel.limit = setting.PurchaseExportRows;
                return File(ExportData(ExportExcel(viewModel)), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
            }

            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            viewModel.limit = setting.PurchaseSeachRows;
            var result = _mediaService.LoadEntitiesFilter(viewModel).ToList();
            viewModel.Medias = result;
            watcher.Stop();
            if (!result.Any())
            {
                ModelState.AddModelError("message", "没有查询到相关媒体信息！");
                return View(viewModel);
            }
            ModelState.AddModelError("message", "本次查询查询耗时：" + watcher.ElapsedMilliseconds + "毫秒，共查询结果为" + viewModel.total + "条。注：查询结果最多显示" + setting.PurchaseSeachRows + "条");
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Export(MediaView viewModel)
        {
            viewModel.Managers = PremissionData();
            var setting = _settingService.GetSetting<WeiGuang>();
            viewModel.limit = setting.PurchaseExportRows;
            return File(ExportData(ExportExcel(viewModel)), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
        }

        private string ExportExcel(MediaView viewModel)
        {
            var result = _mediaService.LoadEntitiesFilter(viewModel).ToList();
            if (!string.IsNullOrWhiteSpace(viewModel.MediaNames))
            {
                var names = viewModel.MediaNames.Split(',').ToList();
                int i = 0;
                foreach (var name in names)
                {
                    var temp = result.FirstOrDefault(d =>
                        d.MediaName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                    if (temp == null)
                    {
                        result.Add(new Media
                        {
                            MediaName = name,
                            Taxis = i
                        });
                    }
                    else
                    {
                        temp.Taxis = i;
                    }

                    i++;
                }
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaIDs))
            {
                var ids = viewModel.MediaIDs.Split(',').ToList();
                int i = 0;
                foreach (var id in ids)
                {
                    var temp = result.FirstOrDefault(d =>
                        d.MediaID.Equals(id, StringComparison.CurrentCultureIgnoreCase));
                    if (temp == null)
                    {
                        result.Add(new Media
                        {
                            MediaID = id,
                            Taxis = i
                        });
                    }
                    else
                    {
                        temp.Taxis = i;
                    }
                    i++;
                }
            }
            JArray jObjects = new JArray();
            foreach (var media in result.OrderBy(d => d.Taxis))
            {
                var jo = new JObject();
                jo.Add("Id", media.Id ?? "不存在的资源");
                jo.Add("结算人", media.LinkMan?.Name);
                jo.Add("媒体类型", media.MediaType?.TypeName);
                jo.Add("平台", media.Platform);
                string website = string.Empty;
                if (!string.IsNullOrWhiteSpace(media.Client) && !string.IsNullOrWhiteSpace(media.Channel))
                {
                    website = "-" + media.Client + "-" + media.Channel;
                }
                jo.Add("媒体名称", media.MediaName + website);
                jo.Add("媒体ID", media.MediaID);
                jo.Add("粉丝数", Utils.ShowFansNum(media.FansNum));
                jo.Add("媒体分类", string.Join(",",media.MediaTags.Select(d=>d.TagName)));
                jo.Add("媒体说明", media.Content);
                jo.Add("备注说明", media.Remark);
                jo.Add("价格有效期", "");
                foreach (var mediaMediaPrice in media.MediaPrices)
                {
                    jo.Add(mediaMediaPrice.AdPositionName, mediaMediaPrice.PurchasePrice);
                    //jo.Add(mediaMediaPrice.AdPositionName + "更新日期", mediaMediaPrice.PriceDate);
                    //jo.Add(mediaMediaPrice.AdPositionName + "失效日期", mediaMediaPrice.InvalidDate);
                }
                jObjects.Add(jo);
            }

            return jObjects.ToString();
        }
        public ActionResult GetList(MediaView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _mediaService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new MediaView
                {
                    Id = d.Id,
                    MediaName = SetMediaName(d),
                    MediaID = d.MediaID,
                    IsAuthenticate = d.IsAuthenticate,
                    IsOriginal = d.IsOriginal,
                    IsComment = d.IsComment,
                    FansNum = Utils.ShowFansNum(d.FansNum),
                    ChannelType = d.ChannelType,
                    LastReadNum = d.LastReadNum,
                    AvgReadNum = d.AvgReadNum,
                    PublishFrequency = d.PublishFrequency,
                    Areas = d.Area,
                    Sex = d.Sex,
                    Client = d.Client,
                    SEO = d.SEO,
                    Efficiency = d.Efficiency,
                    ResourceType = d.ResourceType,
                    Channel = d.Channel,
                    LastPushDate = d.LastPushDate,
                    AuthenticateType = d.AuthenticateType,
                    Platform = d.Platform,
                    TransmitNum = d.TransmitNum,
                    CommentNum = d.CommentNum,
                    LikesNum = d.LikesNum,
                    Content = d.Content,
                    Remark = d.Remark,
                    Status = d.Status,
                    ApiUpDate = d.ApiUpDate,
                    MediaLink = d.MediaLink,
                    MediaLogo = d.MediaLogo,
                    MediaQR = d.MediaQR,
                    LinkManId = d.LinkManId,
                    LinkManName = d.LinkMan.Name,
                    Transactor = d.Transactor,
                    MediaGroups = d.MediaGroups.Select(g => new MediaGroupView() { Id = g.Id, GroupName = g.GroupName }).ToList(),
                    MediaTagStr = string.Join(",", d.MediaTags.Select(t => t.TagName)),
                    MediaPrices = d.MediaPrices.Select(p => new MediaPriceView() { AdPositionName = p.AdPositionName, PriceDate = p.PriceDate, InvalidDate = p.InvalidDate, PurchasePrice = p.PurchasePrice }).ToList()
                })
            }, JsonRequestBehavior.AllowGet);
        }

        
        /// <summary>
        /// 导入更新价格
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
            UpdateMedia(sheet);
            _dbContext.SaveChanges();
            return Json(new { State = 1, Msg = "导入成功" });
        }

        private void UpdateMedia(ISheet sheet)
        {
            //拿到广告位的名称
            IRow headRow = sheet.GetRow(0);
            List<string> adpostionNames = new List<string>();
            int startPrice = 11;//价格所在位置
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

                var media = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
                if (media==null)
                {
                    continue;
                }
                //媒体ID
                if (row.GetCell(5) != null)
                {
                    //校验ID
                    var mediaId = row.GetCell(5).ToString().Trim();
                    var temp = _repository.LoadEntities(d =>
                        d.MediaID.Equals(mediaId, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                        d.MediaTypeId == media.MediaTypeId && d.Id != media.Id).FirstOrDefault();
                    if (temp!=null)
                    {
                        media.IsDelete = true;
                    }
                    else
                    {
                        media.MediaID = row.GetCell(5).ToString().Trim();
                    }
                }
                //修改粉丝
                decimal.TryParse(row.GetCell(6)?.ToString(), out var fansNum);
                media.FansNum = Utils.SetFansNum(fansNum);
                //标签
                if (row.GetCell(7) != null)
                {
                    media.MediaTags.Clear();
                    var tags = row.GetCell(7).ToString().Trim().Replace("，", ",").Split(',');
                    foreach (var tag in tags)
                    {
                        var mediaTag = _mediaTagRepository.LoadEntities(d => d.IsDelete == false && d.TagName == tag)
                            .FirstOrDefault();
                        media.MediaTags.Add(mediaTag);
                    }
                }
                //备注
                media.Remark = row.GetCell(9)?.ToString();
                
                if (!DateTime.TryParse(row.GetCell(10).ToString(), out var date))
                {
                    date = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                        DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
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
                    mediaPrice.InvalidDate = date;
                   
                }
            }
        }
        public ActionResult GetMediaPrices(MediaView viewModel)
        {
            var result = _mediaPriceService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new
                {
                    d.Id,
                    MediaName = SetMediaName(d.Media),
                    d.Media.MediaType.TypeName,
                    d.Media.MediaType.CallIndex,
                    d.Media.MediaID,
                    d.AdPositionName,
                    d.Media.Platform,
                    d.Media.Client,
                    d.Media.Channel,
                    d.PurchasePrice,
                    d.Media.Transactor,
                    MediaTagStr = string.Join(",", d.Media.MediaTags.Select(t => t.TagName)),

                })
            }, JsonRequestBehavior.AllowGet);
        }

        private string SetMediaName(Media media)
        {
            string str = media.MediaName;
            switch (media.MediaType.CallIndex)
            {
                case "website":
                    str = media.MediaName + " - " + media.Client + " - " + media.Channel;
                    break;
                case "weixin":
                case "zhihu":
                    str = media.MediaName + " - " + media.MediaID;
                    break;
                case "headline":
                case "webcast":
                case "brush":
                    str = media.Platform + " - " + media.MediaName;
                    break;
            }

            return str;
        }
        public ActionResult GetMedias(MediaView viewModel)
        {
            var result = _mediaService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new
                {
                    d.Id,
                    d.MediaName,
                    d.MediaType.TypeName,
                })
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Add(string id)
        {
            var mediaType = _mediaTypeRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            MediaView viewModel = new MediaView();
            viewModel.MediaTypeId = id;
            viewModel.MediaTypeIndex = mediaType.CallIndex;
            viewModel.MediaTypeName = mediaType.TypeName;
            viewModel.Status = Consts.StateNormal;
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(MediaView viewModel)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            //校验ID不能重复
            if (IsExist(viewModel, out var msg) != null)
            {
                ModelState.AddModelError("message", msg);
                return View(viewModel);
            }
            ////查询删除的有没有，有就显示出来
            //var temp = IsExist(viewModel, out var msg1, false, true);

            Media entity = new Media();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedById = CurrentManager.Id;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedDate = DateTime.Now;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;


            entity.MediaName = viewModel.MediaName.Trim();
            entity.MediaID = string.IsNullOrWhiteSpace(viewModel.MediaID) ? null : viewModel.MediaID.Trim();
            entity.MediaLink = viewModel.MediaLink;
            entity.MediaLogo = viewModel.MediaLogo;
            entity.MediaQR = viewModel.MediaQR;
            entity.Sex = viewModel.Sex;
            entity.Platform = viewModel.Platform;
            entity.IsAuthenticate = viewModel.IsAuthenticate;
            entity.IsOriginal = viewModel.IsOriginal;
            entity.IsComment = viewModel.IsComment;
            
            entity.FansNum = Utils.SetFansNum(viewModel.FansNum);
            entity.LastReadNum = viewModel.LastReadNum;
            entity.AvgReadNum = viewModel.AvgReadNum;
            entity.PublishFrequency = viewModel.PublishFrequency;
            entity.Area = viewModel.Areas;
            entity.ChannelType = viewModel.ChannelType;
            entity.LastPushDate = viewModel.LastPushDate;
            entity.AuthenticateType = viewModel.AuthenticateType;
            entity.TransmitNum = viewModel.TransmitNum;
            entity.CommentNum = viewModel.CommentNum;
            entity.LikesNum = viewModel.LikesNum;
            entity.Client = viewModel.Client;
            entity.SEO = viewModel.SEO;
            entity.Efficiency = viewModel.Efficiency;
            entity.ResourceType = viewModel.ResourceType;
            entity.Channel = viewModel.Channel;
            entity.Cooperation = viewModel.Cooperation;

            entity.Content = viewModel.Content;
            entity.Remark = viewModel.Remark;
            entity.Status = viewModel.Status;
            entity.ClickNum = viewModel.ClickNum;
            entity.IsHot = viewModel.IsHot;
            entity.IsSlide = viewModel.IsSlide;
            entity.IsRecommend = viewModel.IsRecommend;
            entity.MediaTypeId = viewModel.MediaTypeId;
            entity.LinkManId = viewModel.LinkManId;

            //媒体价格
            foreach (var viewModelMediaPrice in viewModel.MediaPrices)
            {
                MediaPrice price = new MediaPrice();
                price.Id = IdBuilder.CreateIdNum();
                price.AdPositionId = viewModelMediaPrice.AdPositionId;
                price.AdPositionName = viewModelMediaPrice.AdPositionName;
                price.InvalidDate = viewModelMediaPrice.InvalidDate;
                price.PurchasePrice = viewModelMediaPrice.PurchasePrice;
                price.PriceDate = viewModelMediaPrice.PriceDate;
                entity.MediaPrices.Add(price);
            }
            //媒体分类
            if (viewModel.MediaTagIds != null)
            {
                foreach (var viewModelMediaTagId in viewModel.MediaTagIds)
                {
                    var tag = _mediaTagRepository.LoadEntities(d => d.Id == viewModelMediaTagId).FirstOrDefault();
                    entity.MediaTags.Add(tag);
                }
            }
            _mediaService.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Add", new { id = entity.MediaTypeId });
        }
        public ActionResult Update(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            MediaView entity = new MediaView();
            entity.Id = item.Id;
            entity.Transactor = item.Transactor;
            entity.TransactorId = item.TransactorId;
            entity.MediaTypeIndex = item.MediaType.CallIndex;
            entity.MediaTypeName = item.MediaType.TypeName;
            entity.MediaTypeId = item.MediaType.Id;

            entity.MediaName = item.MediaName;
            entity.MediaID = item.MediaID;
            entity.MediaLink = item.MediaLink;
            entity.MediaLogo = item.MediaLogo;
            entity.MediaQR = item.MediaQR;
            entity.Sex = item.Sex;
            entity.Platform = item.Platform;
            entity.IsAuthenticate = item.IsAuthenticate;
            entity.IsOriginal = item.IsOriginal;
            entity.IsComment = item.IsComment;
            entity.FansNum = Utils.ShowFansNum(item.FansNum);
            entity.LastReadNum = item.LastReadNum;
            entity.AvgReadNum = item.AvgReadNum;
            entity.PublishFrequency = item.PublishFrequency;
            entity.LastPushDate = item.LastPushDate;
            entity.Areas = item.Area;
            entity.ChannelType = item.ChannelType;
            entity.AuthenticateType = item.AuthenticateType;
            entity.TransmitNum = item.TransmitNum;
            entity.CommentNum = item.CommentNum;
            entity.LikesNum = item.LikesNum;
            entity.Client = item.Client;
            entity.SEO = item.SEO;
            entity.Efficiency = item.Efficiency;
            entity.ResourceType = item.ResourceType;
            entity.Channel = item.Channel;
            entity.Cooperation = item.Cooperation;

            entity.Content = item.Content;
            entity.Remark = item.Remark;
            entity.Status = item.Status;
            entity.ClickNum = item.ClickNum;
            entity.IsHot = item.IsHot;
            entity.IsSlide = item.IsSlide;
            entity.IsRecommend = item.IsRecommend;

            //联系人
            entity.LinkManId = item.LinkManId;
            entity.LinkManName = item.LinkMan.Name;
            //标签
            entity.MediaTagIds = item.MediaTags.Select(d => d.Id).ToList();
            //媒体价格
            entity.MediaPrices = item.MediaPrices.Select(d => new MediaPriceView()
            {
                Id = d.Id,
                AdPositionName = d.AdPositionName,
                AdPositionId = d.AdPositionId,
                PriceDate = d.PriceDate,
                InvalidDate = d.InvalidDate,
                PurchasePrice = d.PurchasePrice
            }).ToList();
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(MediaView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            //校验ID不能重复
            if (IsExist(viewModel, out var msg, true) != null)
            {
                ModelState.AddModelError("message", msg);
                return View(viewModel);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;

            entity.MediaName = viewModel.MediaName.Trim();
            entity.MediaID = string.IsNullOrWhiteSpace(viewModel.MediaID) ? null : viewModel.MediaID.Trim();
            entity.MediaLink = viewModel.MediaLink;
            entity.MediaLogo = viewModel.MediaLogo;
            entity.MediaQR = viewModel.MediaQR;
            entity.Client = viewModel.Client;
            entity.SEO = viewModel.SEO;
            entity.Efficiency = viewModel.Efficiency;
            entity.ResourceType = viewModel.ResourceType;
            entity.Channel = viewModel.Channel;
            entity.Sex = viewModel.Sex;
            entity.Platform = viewModel.Platform;
            entity.IsAuthenticate = viewModel.IsAuthenticate;
            entity.IsOriginal = viewModel.IsOriginal;
            entity.IsComment = viewModel.IsComment;
            entity.FansNum = Utils.SetFansNum(viewModel.FansNum);
            entity.LastReadNum = viewModel.LastReadNum;
            entity.AvgReadNum = viewModel.AvgReadNum;
            entity.PublishFrequency = viewModel.PublishFrequency;
            entity.Area = viewModel.Areas;
            entity.ChannelType = viewModel.ChannelType;
            entity.LastPushDate = viewModel.LastPushDate;
            entity.AuthenticateType = viewModel.AuthenticateType;
            entity.TransmitNum = viewModel.TransmitNum;
            entity.CommentNum = viewModel.CommentNum;
            entity.LikesNum = viewModel.LikesNum;
            entity.Content = viewModel.Content;
            entity.Remark = viewModel.Remark;
            entity.Status = viewModel.Status;
            entity.ClickNum = viewModel.ClickNum;
            entity.IsHot = viewModel.IsHot;
            entity.IsSlide = viewModel.IsSlide;
            entity.IsRecommend = viewModel.IsRecommend;
            entity.Cooperation = viewModel.Cooperation;
            //联系人
            entity.LinkManId = viewModel.LinkManId;
            //标签
            if (viewModel.MediaTagIds != null)
            {
                entity.MediaTags.Clear();
                foreach (var viewModelMediaTagId in viewModel.MediaTagIds)
                {
                    var tag = _mediaTagRepository.LoadEntities(d => d.Id == viewModelMediaTagId).FirstOrDefault();
                    entity.MediaTags.Add(tag);
                }
            }
            //价格
            foreach (var viewModelMediaPrice in viewModel.MediaPrices)
            {
                if (string.IsNullOrWhiteSpace(viewModelMediaPrice.Id))
                {
                    MediaPrice price = new MediaPrice();
                    price.Id = IdBuilder.CreateIdNum();
                    price.AdPositionId = viewModelMediaPrice.AdPositionId;
                    price.AdPositionName = viewModelMediaPrice.AdPositionName;
                    price.InvalidDate = viewModelMediaPrice.InvalidDate;
                    price.PurchasePrice = viewModelMediaPrice.PurchasePrice;
                    price.PriceDate = viewModelMediaPrice.PriceDate;
                    entity.MediaPrices.Add(price);
                }
                else
                {
                    var price = _mediaPriceRepository.LoadEntities(d => d.Id == viewModelMediaPrice.Id).FirstOrDefault();
                    price.InvalidDate = viewModelMediaPrice.InvalidDate;
                    price.PriceDate = viewModelMediaPrice.PriceDate;
                    price.PurchasePrice = viewModelMediaPrice.PurchasePrice;
                }
            }
            _mediaService.Update(entity);
            TempData["Msg"] = "更新成功";
            return View(viewModel);
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _mediaService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
        public ActionResult Comment(string id, int page = 1)
        {
            ViewBag.Page = page;
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comment(MediaCommentView viewModel)
        {

            var entity = _repository.LoadEntities(d => d.Id == viewModel.MediaId).FirstOrDefault();
            MediaComment comment = new MediaComment();
            comment.Id = IdBuilder.CreateIdNum();
            comment.CommentDate = DateTime.Now;
            comment.Transactor = CurrentManager.UserName;
            comment.TransactorId = CurrentManager.Id;
            comment.Content = viewModel.Content;
            comment.Score = viewModel.Score;
            entity.MediaComments.Add(comment);
            _mediaService.Update(entity);
            return View(entity);
        }

        private Media IsExist(MediaView viewModel, out string msg, bool isSelf = false, bool isDelete = false)
        {
            msg = string.Empty;


            Expression<Func<Media, bool>> whereLambda;
            switch (viewModel.MediaTypeIndex)
            {
                case "weixin":
                case "zhihu":
                case "sinablog":
                    whereLambda = d =>
                          d.MediaID.Equals(viewModel.MediaID.Trim(), StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == isDelete &&
                          d.MediaTypeId == viewModel.MediaTypeId;
                    if (isSelf)
                    {
                        whereLambda = d =>
                            d.MediaID.Equals(viewModel.MediaID.Trim(), StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == isDelete &&
                            d.MediaTypeId == viewModel.MediaTypeId && d.Id != viewModel.Id;
                    }

                    break;
                case "headline":
                case "webcast":
                case "brush":
                    whereLambda = d =>
                          d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                          d.Platform.Equals(viewModel.Platform, StringComparison.CurrentCultureIgnoreCase) &&
                          d.IsDelete == isDelete &&
                          d.MediaTypeId == viewModel.MediaTypeId;
                    if (isSelf)
                    {
                        whereLambda = d =>
                            d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                            d.Platform.Equals(viewModel.Platform, StringComparison.CurrentCultureIgnoreCase) &&
                            d.IsDelete == isDelete &&
                            d.MediaTypeId == viewModel.MediaTypeId && d.Id != viewModel.Id;
                    }
                    break;
                case "website":
                    whereLambda = d =>
                          d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                          d.Client.Equals(viewModel.Client, StringComparison.CurrentCultureIgnoreCase) &&
                          d.Channel.Equals(viewModel.Channel, StringComparison.CurrentCultureIgnoreCase) &&
                          d.IsDelete == isDelete &&
                          d.MediaTypeId == viewModel.MediaTypeId;
                    if (isSelf)
                    {
                        whereLambda = d =>
                            d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                            d.Client.Equals(viewModel.Client, StringComparison.CurrentCultureIgnoreCase) &&
                            d.Channel.Equals(viewModel.Channel, StringComparison.CurrentCultureIgnoreCase) &&
                            d.IsDelete == isDelete &&
                            d.MediaTypeId == viewModel.MediaTypeId && d.Id != viewModel.Id;
                    }
                    break;
                default:
                    whereLambda = d =>
                          d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == isDelete &&
                          d.MediaTypeId == viewModel.MediaTypeId;
                    if (isSelf)
                    {
                        whereLambda = d =>
                            d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == isDelete &&
                            d.MediaTypeId == viewModel.MediaTypeId && d.Id != viewModel.Id;
                    }

                    break;
            }

            var media = _repository.LoadEntities(whereLambda).FirstOrDefault();
            if (media == null) return null;
            msg = viewModel.MediaName + "，此媒体账号已存在！";
            return media;
        }
    }
}