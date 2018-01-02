﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Resource;
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
                var result = _mediaService.LoadEntitiesFilter(viewModel).ToList();
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
                    string website = string.Empty;
                    if (!string.IsNullOrWhiteSpace(media.Client) && !string.IsNullOrWhiteSpace(media.Channel))
                    {
                        website = "-" + media.Client + "-" + media.Channel;
                    }
                    jo.Add("媒体名称", media.MediaName + website);
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
                return File(ExportData(jObjects.ToString()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
            }
            else
            {
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
                ModelState.AddModelError("message", "本次查询查询耗时：" + watcher.ElapsedMilliseconds + "毫秒，共查询结果为" + result.Count + "条。注：查询结果最多显示" + setting.PurchaseSeachRows + "条");
                return View(viewModel);
            }


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
            if (IsExist(viewModel, out var msg))
            {
                ModelState.AddModelError("message", msg);
                return View(viewModel);
            }
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
            entity.FansNum = viewModel.FansNum;
            entity.LastReadNum = viewModel.LastReadNum;
            entity.AvgReadNum = viewModel.AvgReadNum;
            entity.PublishFrequency = viewModel.PublishFrequency;
            entity.Area = viewModel.Area;
            entity.ChannelType = viewModel.ChannelType;
            //entity.LastPushDate = viewModel.LastPushDate;
            entity.AuthenticateType = viewModel.AuthenticateType;
            entity.TransmitNum = viewModel.TransmitNum;
            entity.CommentNum = viewModel.CommentNum;
            entity.LikesNum = viewModel.LikesNum;
            entity.Client = viewModel.Client;
            entity.SEO = viewModel.SEO;
            entity.Efficiency = viewModel.Efficiency;
            entity.ResourceType = viewModel.ResourceType;
            entity.Channel = viewModel.Channel;

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
            return RedirectToAction("Index");
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
            entity.FansNum = item.FansNum;
            entity.LastReadNum = item.LastReadNum;
            entity.AvgReadNum = item.AvgReadNum;
            entity.PublishFrequency = item.PublishFrequency;
            entity.Area = item.Area;
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
            if (IsExist(viewModel, out var msg, true))
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
            entity.FansNum = viewModel.FansNum;
            entity.LastReadNum = viewModel.LastReadNum;
            entity.AvgReadNum = viewModel.AvgReadNum;
            entity.PublishFrequency = viewModel.PublishFrequency;
            entity.Area = viewModel.Area;
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
            return RedirectToAction("Index");
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



        private bool IsExist(MediaView viewModel, out string msg, bool isSelf = false)
        {
            msg = string.Empty;
            bool result = false;

            Expression<Func<Media, bool>> whereLambda;
            switch (viewModel.MediaTypeIndex)
            {
                case "weixin":
                case "zhihu":

                    whereLambda = d =>
                          d.MediaID.Equals(viewModel.MediaID.Trim(), StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                          d.MediaTypeId == viewModel.MediaTypeId;
                    if (isSelf)
                    {
                        whereLambda = d =>
                            d.MediaID.Equals(viewModel.MediaID.Trim(), StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                            d.MediaTypeId == viewModel.MediaTypeId && d.Id != viewModel.Id;
                    }

                    break;
                case "headline":
                case "webcast":
                case "brush":
                    whereLambda = d =>
                          d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                          d.Platform.Equals(viewModel.Platform, StringComparison.CurrentCultureIgnoreCase) &&
                          d.IsDelete == false &&
                          d.MediaTypeId == viewModel.MediaTypeId;
                    if (isSelf)
                    {
                        whereLambda = d =>
                            d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                            d.Platform.Equals(viewModel.Platform, StringComparison.CurrentCultureIgnoreCase) &&
                            d.IsDelete == false &&
                            d.MediaTypeId == viewModel.MediaTypeId && d.Id != viewModel.Id;
                    }
                    break;
                case "website":
                    whereLambda = d =>
                          d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                          d.Client.Equals(viewModel.Client, StringComparison.CurrentCultureIgnoreCase) &&
                          d.Channel.Equals(viewModel.Channel, StringComparison.CurrentCultureIgnoreCase) &&
                          d.IsDelete == false &&
                          d.MediaTypeId == viewModel.MediaTypeId;
                    if (isSelf)
                    {
                        whereLambda = d =>
                            d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                            d.Client.Equals(viewModel.Client, StringComparison.CurrentCultureIgnoreCase) &&
                            d.Channel.Equals(viewModel.Channel, StringComparison.CurrentCultureIgnoreCase) &&
                            d.IsDelete == false &&
                            d.MediaTypeId == viewModel.MediaTypeId && d.Id != viewModel.Id;
                    }
                    break;
                default:
                    whereLambda = d =>
                          d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                          d.MediaTypeId == viewModel.MediaTypeId;
                    if (isSelf)
                    {
                        whereLambda = d =>
                            d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                            d.MediaTypeId == viewModel.MediaTypeId && d.Id != viewModel.Id;
                    }

                    break;
            }

            var media = _repository.LoadEntities(whereLambda).FirstOrDefault();
            if (media != null)
            {
                msg = viewModel.MediaName + "，此媒体账号已存在！";
                result = true;
            }
            return result;
        }
    }
}