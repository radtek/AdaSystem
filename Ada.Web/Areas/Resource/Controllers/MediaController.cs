using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.API;
using Ada.Core.Domain.Customer;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Core.ViewModel.Resource;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Filter;
using Ada.Framework.UploadFile;
using Ada.Services.Admin;
using Ada.Services.API;
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
        private readonly IiDataAPIService _iDataAPIService;
        private readonly ISettingService _settingService;
        private readonly IRepository<Media> _repository;
        private readonly IRepository<APIRequestRecord> _apiRequestRecordRepository;
        private readonly IRepository<MediaType> _mediaTypeRepository;
        private readonly IRepository<MediaPrice> _mediaPriceRepository;
        private readonly IRepository<MediaTag> _mediaTagRepository;
        private readonly IDbContext _dbContext;
        private readonly IFieldService _fieldService;
        public MediaController(IMediaPriceService mediaPriceService,
            IMediaService mediaService,
            IRepository<MediaPrice> mediaPriceRepository,
            IDbContext dbContext,
            IRepository<Media> repository,
            IRepository<MediaTag> mediaTagRepository,
            IRepository<MediaType> mediaTypeRepository,
            ISettingService settingService,
            IiDataAPIService iDataAPIService,
            IRepository<APIRequestRecord> apiRequestRecordRepository,
            IFieldService fieldService)
        {
            _mediaPriceService = mediaPriceService;
            _mediaService = mediaService;
            _mediaPriceRepository = mediaPriceRepository;
            _dbContext = dbContext;
            _repository = repository;
            _mediaTagRepository = mediaTagRepository;
            _mediaTypeRepository = mediaTypeRepository;
            _settingService = settingService;
            _iDataAPIService = iDataAPIService;
            _apiRequestRecordRepository = apiRequestRecordRepository;
            _fieldService = fieldService;
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
            if (string.IsNullOrWhiteSpace(viewModel.MediaTypeIndex))
            {
                ModelState.AddModelError("message", "请先选择媒体类型！");
                return View(viewModel);
            }
            var setting = _settingService.GetSetting<WeiGuang>();
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            viewModel.Status = Consts.StateNormal;
            var isExport = export == "export";
            viewModel.limit = isExport ? setting.BusinessExportRows : setting.BusinessSeachRows;
            var results = _mediaService.LoadEntitiesFilter(viewModel).AsNoTracking().ToList();
            watcher.Stop();
            if (isExport)
            {
                var jObjects = ExprotTemplate(viewModel, results);
                return File(ExportData(jObjects.ToString()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
            }
            viewModel.Medias = results.OrderBy(d => d.Taxis).ToList();
            if (!results.Any())
            {
                ModelState.AddModelError("message", "没有查询到相关媒体信息！");
                return View(viewModel);
            }
            ModelState.AddModelError("message", "本次查询查询耗时：" + watcher.ElapsedMilliseconds + "毫秒，共查询结果为" + viewModel.total + "条。注：查询结果最多显示" + setting.BusinessSeachRows + "条。");
            return View(viewModel);
        }

        private JArray ExprotTemplate(MediaView viewModel, List<Media> results)
        {
            List<Media> noDatas = new List<Media>();
            //找到没有的
            if (!string.IsNullOrWhiteSpace(viewModel.MediaBatch))
            {
                var names = viewModel.MediaBatch.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
                int i = 0;
                foreach (var name in names)
                {
                    var mediaInfo = name.Trim();
                    var temp = results.FirstOrDefault(d =>
                        d.MediaName == mediaInfo || d.MediaID == mediaInfo);
                    if (temp == null)
                    {
                        noDatas.Add(new Media
                        {
                            MediaName = name,
                            Taxis = i
                        });
                        //noDatas.Add(name);
                    }
                    else
                    {
                        temp.Taxis = i;
                    }

                    i++;
                }
            }
            JArray jObjects = new JArray();
            var priceTypeStr = string.IsNullOrWhiteSpace(viewModel.PriceType) ? "0" : viewModel.PriceType;
            var priceTypeList = priceTypeStr.Split(',');
            if (noDatas.Any())
            {
                results.AddRange(noDatas);
            }
            foreach (var media in results.OrderBy(d => d.Taxis))
            {
                var jo = new JObject();
                jo.Add("主键", string.IsNullOrWhiteSpace(media.Id) ? "不存在的资源" : media.Id);
                jo.Add("媒体分类", string.Join(",", media.MediaTags.Select(d => d.TagName)));
                jo.Add("媒体名称", media.MediaName);
                jo.Add("粉丝数(万)", Utils.ShowFansNum(media.FansNum));
                switch (viewModel.MediaTypeIndex)
                {
                    case "weixin":
                        jo.Add("媒体ID", media.MediaID);
                        jo.Add("认证情况", media.IsAuthenticate == null ? "" : media.IsAuthenticate == true ? "已认证" : "未认证");
                        
                        jo.Add("平均头条浏览数", media.AvgReadNum);
                        jo.Add("月发布频次", media.PublishFrequency);
                        jo.Add("月发文总数", media.MonthPostNum);
                        jo.Add("最近发布日期", media.LastPushDate?.ToString("yyyy-MM-dd"));
                        break;
                    case "sinablog":
                        jo.Add("媒体链接", media.MediaLink);
                        jo.Add("性别", media.Sex);
                        jo.Add("地区", media.Area);
                        jo.Add("认证情况", media.IsAuthenticate == null ? "" : media.IsAuthenticate == true ? "已认证" : "未认证");
                        jo.Add("认证类型", media.AuthenticateType);
                        jo.Add("平均转发数", media.TransmitNum);
                        jo.Add("平均评论数", media.CommentNum);
                        jo.Add("平均点赞数", media.LikesNum);
                        jo.Add("最近发布日期", media.LastPushDate?.ToString("yyyy-MM-dd"));
                        break;
                    case "douyin":
                        jo.Add("媒体链接", media.MediaLink);
                        jo.Add("性别", media.Sex);
                        jo.Add("地区", media.Area);
                        jo.Add("认证情况", media.IsAuthenticate == null ? "" : media.IsAuthenticate == true ? "已认证" : "未认证");
                        jo.Add("平均转发数", media.TransmitNum);
                        jo.Add("平均浏览数", media.AvgReadNum);
                        jo.Add("平均评论数", media.CommentNum);
                        jo.Add("平均点赞数", media.LikesNum);
                        break;
                    case "redbook":
                        jo.Add("媒体链接", media.MediaLink);
                        jo.Add("地区", media.Area);
                        jo.Add("等级", media.AuthenticateType);
                        jo.Add("平均收藏数", media.TransmitNum);
                        jo.Add("平均点赞数", media.AvgReadNum);
                        jo.Add("平均评论数", media.CommentNum);
                        jo.Add("赞与收藏", media.LikesNum);
                        jo.Add("关注数", media.FriendNum);
                        jo.Add("笔记总数", media.PostNum);
                        break;
                    case "zhihu":
                        jo.Add("媒体链接", media.MediaLink);
                        jo.Add("地区", media.Area);
                        break;
                    default:
                        jo.Add("平台", media.Platform);
                        if (!string.IsNullOrWhiteSpace(media.Client))
                        {
                            jo.Add("客户端", media.Client);
                        }
                        if (!string.IsNullOrWhiteSpace(media.Channel))
                        {
                            jo.Add("媒体频道", media.Channel);
                        }
                        jo.Add("媒体链接", media.MediaLink);
                        jo.Add("性别", media.Sex);
                        jo.Add("地区", media.Area);
                        if (!string.IsNullOrWhiteSpace(media.ResourceType))
                        {
                            jo.Add("资源类型", media.ResourceType);
                        }
                        if (!string.IsNullOrWhiteSpace(media.Efficiency))
                        {
                            jo.Add("出稿速度", media.Efficiency);
                        }
                        if (!string.IsNullOrWhiteSpace(media.SEO))
                        {
                            jo.Add("收录效果", media.SEO);
                        }
                        break;
                }
                foreach (var priceType in priceTypeList)
                {
                    foreach (var mediaMediaPrice in media.MediaPrices)
                    {
                        var price = mediaMediaPrice.PurchasePrice ?? 0;
                        var priceStr = "【成本】";
                        if (priceType == "1")
                        {
                            price = mediaMediaPrice.MarketPrice ?? 0;
                            priceStr = "【销售】";
                        }
                        if (priceType == "2")
                        {
                            price = mediaMediaPrice.SellPrice ?? 0;
                            priceStr = "【零售】";
                        }
                        jo.Add(priceStr + mediaMediaPrice.AdPositionName, price);
                    }
                }
                jo.Add("价格日期", media.MediaPrices.FirstOrDefault()?.InvalidDate?.ToString("yyyy-MM-dd"));
                jo.Add("媒体说明", media.Content);
                jo.Add("备注说明", media.Remark);
                jo.Add("经办媒介", media.Transactor);
                jObjects.Add(jo);
            }

            return jObjects;
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
                var names = viewModel.MediaNames.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
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
                var ids = viewModel.MediaIDs.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
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
                jo.Add("地区", media.Area);
                jo.Add("媒体分类", string.Join(",", media.MediaTags.Select(d => d.TagName)));
                jo.Add("媒体说明", media.Content);
                jo.Add("备注说明", media.Remark);
                var date = media.MediaPrices.FirstOrDefault()?.InvalidDate;
                jo.Add("价格有效期", date?.ToString("yyyy-MM-dd") ?? "");
                foreach (var mediaMediaPrice in media.MediaPrices)
                {
                    jo.Add(mediaMediaPrice.AdPositionName, mediaMediaPrice.PurchasePrice);
                }
                jObjects.Add(jo);
            }
            return jObjects.ToString();
        }
        [HttpPost]
        public ActionResult GetList(MediaView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _mediaService.LoadEntitiesFilter(viewModel).AsNoTracking().ToList();
            return Json(new
            {
                viewModel.total,
                //rows = result
                rows = result.Select(d => new MediaView
                {
                    Id = d.Id,
                    MediaName = d.MediaName,
                    MediaID = d.MediaID,
                    MediaTypeIndex = d.MediaType.CallIndex,
                    MediaTypeLogo = d.MediaType.Image,
                    MediaTypeName = d.MediaType.TypeName,
                    IsAuthenticate = d.IsAuthenticate,
                    IsOriginal = d.IsOriginal,
                    IsComment = d.MediaType.IsComment,
                    FansNum = d.FansNum,
                    ChannelType = d.ChannelType,
                    LastReadNum = d.LastReadNum,
                    AvgReadNum = d.AvgReadNum,
                    PublishFrequency = d.PublishFrequency,
                    Areas = d.Area,
                    Sex = d.Sex,
                    Client = d.Client,
                    SEO = d.SEO,
                    Abstract = d.Abstract,
                    PostNum = d.PostNum,
                    MonthPostNum = d.MonthPostNum,
                    FriendNum = d.FriendNum,
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
                    IsHot = d.IsHot,
                    IsRecommend = d.IsRecommend,
                    IsTop = d.IsTop,
                    ApiUpDate = d.ApiUpDate,
                    MediaLink = d.MediaLink,
                    MediaLogo = d.MediaLogo,
                    MediaQR = d.MediaQR,
                    LinkManId = d.LinkManId,
                    LinkManName = d.LinkMan.Name,
                    Transactor = d.Transactor,
                    MediaGroups = d.MediaGroups.Where(m=>m.GroupType==Consts.StateNormal).Select(g => new MediaGroupView() { Id = g.Id, GroupName = g.GroupName }).ToList(),
                    MediaTags = d.MediaTags.Select(t => new MediaTagView() { Id = t.Id, TagName = t.TagName }).ToList(),
                    MediaPrices = d.MediaPrices.Select(p => new MediaPriceView() { AdPositionName = p.AdPositionName, PriceDate = p.PriceDate, InvalidDate = p.InvalidDate, PurchasePrice = p.PurchasePrice }).ToList()
                })
            });
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
            if (sheet.LastRowNum < 1)
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
            int startPrice = 12;//价格所在位置
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
                if (media == null)
                {
                    continue;
                }
                //粉丝
                decimal.TryParse(row.GetCell(6)?.ToString(), out var fansNum);
                media.FansNum = Utils.SetFansNum(fansNum);
                //地区
                media.Area = row.GetCell(7)?.ToString();
                //标签
                if (row.GetCell(8) != null)
                {
                    media.MediaTags.Clear();
                    var tags = row.GetCell(8).ToString().Trim().Replace("，", ",").Split(',');
                    foreach (var tag in tags)
                    {
                        var mediaTag = _mediaTagRepository.LoadEntities(d => d.IsDelete == false && d.TagName == tag)
                            .FirstOrDefault();
                        media.MediaTags.Add(mediaTag);
                    }
                }
                //备注
                media.Remark = row.GetCell(10)?.ToString();
                DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                    DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                if (row.GetCell(11) != null)
                {
                    var cellType = row.GetCell(11).CellType;
                    if (cellType == CellType.String)
                    {
                        DateTime.TryParse(row.GetCell(11)?.ToString(), out date);
                    }
                    if (cellType == CellType.Numeric)
                    {
                        date = row.GetCell(11).DateCellValue;
                    }
                }
                var sellKey = "SellPriceRange";
                var marketKey = "ExportPrice";
                if (media.MediaType.CallIndex == "douyin" || media.MediaType.CallIndex == "redbook")
                {
                    sellKey = "SellPriceRangeByR&D";
                    marketKey = "SellPriceRangeByR&D";
                }
                else
                {
                    if (media.IsHot == true)
                    {
                        sellKey = "HotSellPriceRange";
                    }
                }
                var sellPriceRange = _fieldService.GetFieldsByKey(sellKey).ToList();
                var marketPriceRange = _fieldService.GetFieldsByKey(marketKey).ToList();
                for (int j = 0; j < adpostionNames.Count; j++)
                {
                    var name = adpostionNames[j];
                    var mediaPrice = _mediaPriceRepository
                        .LoadEntities(d => d.MediaId == id && d.AdPositionName == name).FirstOrDefault();
                    var tempCell = row.GetCell(startPrice + j);
                    double tempPrice = 0;
                    if (tempCell != null)
                    {
                        if (tempCell.CellType == CellType.Formula || tempCell.CellType == CellType.Numeric)
                        {
                            tempPrice = tempCell.NumericCellValue;
                        }
                        if (tempCell.CellType == CellType.String)
                        {
                            double.TryParse(tempCell.ToString(), out tempPrice);
                        }
                    }
                    if (mediaPrice == null)
                    {
                        var newPrice = media.MediaType.AdPositions.FirstOrDefault(d => d.Name == name);
                        if (newPrice != null)
                        {
                            media.MediaPrices.Add(new MediaPrice()
                            {
                                Id = IdBuilder.CreateIdNum(),
                                PurchasePrice = Convert.ToDecimal(tempPrice),
                                SellPrice = SetSalePrice(Convert.ToDecimal(tempPrice), sellPriceRange),
                                MarketPrice = SetSalePrice(Convert.ToDecimal(tempPrice), marketPriceRange),
                                PriceDate = DateTime.Now,
                                InvalidDate = date,
                                AdPositionName = newPrice.Name,
                                AdPositionId = newPrice.Id
                            });
                        }
                    }
                    else
                    {
                        //判断，如果价格更新了，就更新市场价格
                        mediaPrice.PurchasePrice = Convert.ToDecimal(tempPrice);
                        mediaPrice.SellPrice = media.Cooperation == Consts.StateNormal ? mediaPrice.SellPrice : SetSalePrice(Convert.ToDecimal(tempPrice), sellPriceRange);
                        mediaPrice.MarketPrice = media.Cooperation == Consts.StateNormal ? mediaPrice.SellPrice : SetSalePrice(Convert.ToDecimal(tempPrice), marketPriceRange);
                        mediaPrice.PriceDate = DateTime.Now;
                        mediaPrice.InvalidDate = date;
                    }



                }
            }
        }
        /// <summary>
        /// 导入更新价格
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Confirm()
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
            if (sheet.LastRowNum < 1)
            {
                return Json(new { State = 0, Msg = "此文件没有导入数据，请填充数据再进行导入" });
            }
            List<string> losts = new List<string>();
            List<string> heightPrice = new List<string>();
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                var mediaName = row.GetCell(0)?.ToString();
                var client = row.GetCell(1)?.ToString();
                var channal = row.GetCell(2)?.ToString();
                var adpositon = row.GetCell(5)?.ToString();
                //根据资源找价格
                if (string.IsNullOrWhiteSpace(mediaName) || string.IsNullOrWhiteSpace(client) || string.IsNullOrWhiteSpace(channal) || string.IsNullOrWhiteSpace(adpositon))
                {
                    continue;
                }

                mediaName = mediaName.Trim();
                client = client.Trim();
                channal = channal.Trim();
                adpositon = adpositon.Trim();
                var mediaPrice = _mediaPriceRepository.LoadEntities(d =>
                    d.Media.MediaName.Equals(mediaName, StringComparison.CurrentCultureIgnoreCase) &&
                    d.Media.Client.Equals(client, StringComparison.CurrentCultureIgnoreCase) &&
                    d.Media.Channel.Equals(channal, StringComparison.CurrentCultureIgnoreCase) &&
                    d.Media.IsDelete == false && d.AdPositionName == adpositon && d.IsDelete == false).FirstOrDefault();
                if (mediaPrice == null)
                {
                    losts.Add(mediaName + " - " + client + " - " + channal);
                    continue;
                }
                decimal.TryParse(row.GetCell(4)?.ToString(), out var pmoney);
                //判断采购金额是否超出成本
                if (pmoney > mediaPrice.PurchasePrice)
                {
                    heightPrice.Add(mediaName + " - " + client + " - " + channal);
                }
            }
            var lostStr = string.Empty;
            if (losts.Count > 0)
            {
                lostStr = "共" + losts.Count + "条资源不存在：【" + string.Join("，", losts) + "】";
            }
            var heightPriceStr = string.Empty;
            if (heightPrice.Count > 0)
            {
                heightPriceStr = "\r\n\r\n共" + heightPrice.Count + "条超出参考成本：【" + string.Join("，", heightPrice) + "】";
            }

            return Json(string.IsNullOrWhiteSpace(lostStr + heightPriceStr) ? new { State = 1, Msg = "资源全部存在！" } : new { State = 1, Msg = lostStr + heightPriceStr });
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
            viewModel.PriceUpdateDate = DateTime.Now.Date;
            viewModel.Cooperation = Consts.StateLock;
            viewModel.PriceInvalidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).Date;
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
            if (viewModel.MediaTypeIndex == "zhihu")
            {
                entity.MediaLink = "https://www.zhihu.com/people/" + entity.MediaID;
            }
            else
            {
                entity.MediaLink = viewModel.MediaLink;
            }


            entity.Sex = viewModel.Sex;
            entity.Platform = viewModel.Platform;
            entity.IsAuthenticate = viewModel.IsAuthenticate;
            entity.IsOriginal = viewModel.IsOriginal;
            entity.IsComment = viewModel.IsComment;
            entity.FansNum = Utils.SetFansNum(viewModel.FansNum);
            entity.Area = viewModel.Areas;
            entity.ChannelType = viewModel.ChannelType;
            entity.AuthenticateType = viewModel.AuthenticateType;
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
            entity.IsSlide = true;
            entity.MediaTypeId = viewModel.MediaTypeId;
            entity.LinkManId = viewModel.LinkManId;
            entity.IsHot = false;
            entity.IsRecommend = false;
            entity.IsTop = false;
            //媒体价格
            var sellKey = "SellPriceRange";
            var marketKey = "ExportPrice";
            if (viewModel.MediaTypeIndex == "douyin" || viewModel.MediaTypeIndex == "redbook")
            {
                sellKey = "SellPriceRangeByR&D";
                marketKey = "SellPriceRangeByR&D";
            }
            var sellPriceRange = _fieldService.GetFieldsByKey(sellKey).ToList();
            var marketPriceRange = _fieldService.GetFieldsByKey(marketKey).ToList();
            foreach (var viewModelMediaPrice in viewModel.MediaPrices)
            {
                MediaPrice price = new MediaPrice();
                price.Id = IdBuilder.CreateIdNum();
                price.AdPositionId = viewModelMediaPrice.AdPositionId;
                price.AdPositionName = viewModelMediaPrice.AdPositionName;
                price.InvalidDate = viewModel.PriceInvalidDate;
                price.PurchasePrice = Convert.ToDecimal(viewModelMediaPrice.PurchasePrice);
                price.SellPrice = SetSalePrice(Convert.ToDecimal(viewModelMediaPrice.PurchasePrice), sellPriceRange);
                price.MarketPrice = SetSalePrice(Convert.ToDecimal(viewModelMediaPrice.PurchasePrice), marketPriceRange);
                price.PriceDate = viewModel.PriceUpdateDate;
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
            entity.PostNum = item.PostNum;
            entity.MonthPostNum = item.MonthPostNum;
            entity.FriendNum = item.FriendNum;

            entity.Content = item.Content;
            entity.Remark = item.Remark;
            entity.Status = item.Status;
            entity.ClickNum = item.ClickNum;

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
                SellPrice = d.SellPrice,
                PurchasePrice = d.PurchasePrice,
                MarketPrice = d.MarketPrice
            }).ToList();
            entity.PriceUpdateDate = item.MediaPrices.FirstOrDefault()?.PriceDate;
            entity.PriceInvalidDate = item.MediaPrices.FirstOrDefault()?.InvalidDate;
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
            if (viewModel.MediaTypeIndex == "zhihu")
            {
                entity.MediaLink = "https://www.zhihu.com/people/" + entity.MediaID;
            }
            else
            {
                entity.MediaLink = viewModel.MediaLink;
            }

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
            entity.Area = viewModel.Areas;
            entity.ChannelType = viewModel.ChannelType;
            entity.AuthenticateType = viewModel.AuthenticateType;
            entity.Content = viewModel.Content;
            entity.Remark = viewModel.Remark;
            entity.Status = viewModel.Status;
            entity.ClickNum = viewModel.ClickNum;
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
            var sellKey = "SellPriceRange";
            var marketKey = "ExportPrice";
            if (viewModel.MediaTypeIndex == "douyin" || viewModel.MediaTypeIndex == "redbook")
            {
                sellKey = "SellPriceRangeByR&D";
                marketKey = "SellPriceRangeByR&D";
            }
            else
            {
                if (entity.IsHot == true)
                {
                    sellKey = "HotSellPriceRange";
                }
            }
            var sellPriceRange = _fieldService.GetFieldsByKey(sellKey).ToList();
            var marketPriceRange = _fieldService.GetFieldsByKey(marketKey).ToList();
            foreach (var viewModelMediaPrice in viewModel.MediaPrices)
            {
                if (string.IsNullOrWhiteSpace(viewModelMediaPrice.Id))
                {
                    MediaPrice price = new MediaPrice();
                    price.Id = IdBuilder.CreateIdNum();
                    price.AdPositionId = viewModelMediaPrice.AdPositionId;
                    price.AdPositionName = viewModelMediaPrice.AdPositionName;
                    price.InvalidDate = viewModel.PriceInvalidDate;
                    price.PurchasePrice = Convert.ToDecimal(viewModelMediaPrice.PurchasePrice);
                    price.SellPrice = SetSalePrice(Convert.ToDecimal(viewModelMediaPrice.PurchasePrice), sellPriceRange);
                    price.MarketPrice = SetSalePrice(Convert.ToDecimal(viewModelMediaPrice.PurchasePrice), marketPriceRange);
                    price.PriceDate = viewModel.PriceUpdateDate;
                    entity.MediaPrices.Add(price);
                }
                else
                {
                    var price = _mediaPriceRepository.LoadEntities(d => d.Id == viewModelMediaPrice.Id).FirstOrDefault();
                    price.InvalidDate = viewModel.PriceInvalidDate;
                    price.PriceDate = viewModel.PriceUpdateDate;
                    price.PurchasePrice = Convert.ToDecimal(viewModelMediaPrice.PurchasePrice);
                    price.SellPrice = entity.Cooperation == Consts.StateNormal ? price.SellPrice : SetSalePrice(Convert.ToDecimal(viewModelMediaPrice.PurchasePrice), sellPriceRange);
                    price.MarketPrice = entity.Cooperation == Consts.StateNormal ? price.MarketPrice : SetSalePrice(Convert.ToDecimal(viewModelMediaPrice.PurchasePrice), marketPriceRange);
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
        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Top(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.IsTop == null || entity.IsTop == false)
            {
                entity.IsTop = true;
            }
            else
            {
                entity.IsTop = false;
            }
            _mediaService.Update(entity);
            return Json(new { State = 1, Msg = "操作成功" });
        }
        /// <summary>
        /// 推荐
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Recommend(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.IsRecommend == null || entity.IsRecommend == false)
            {
                entity.IsRecommend = true;
            }
            else
            {
                entity.IsRecommend = false;
            }
            _mediaService.Update(entity);
            return Json(new { State = 1, Msg = "操作成功" });
        }
        /// <summary>
        /// 优质
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Hot(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();

            if (entity.IsHot == null || entity.IsHot == false)
            {
                entity.IsHot = true;
            }
            else
            {
                entity.IsHot = false;
            }

            if (entity.Cooperation != Consts.StateNormal)
            {
                //小红书和抖音不影响
                if (entity.MediaType.CallIndex != "redbook" || entity.MediaType.CallIndex != "douyin")
                {
                    var sellPriceRange = _fieldService.GetFieldsByKey(entity.IsHot == true ? "HotSellPriceRange" : "SellPriceRange").ToList();
                    foreach (var entityMediaPrice in entity.MediaPrices)
                    {
                        entityMediaPrice.SellPrice = SetSalePrice(Convert.ToDecimal(entityMediaPrice.PurchasePrice), sellPriceRange);
                    }
                }
            }
            _mediaService.Update(entity);
            return Json(new { State = 1, Msg = "操作成功" });
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

        public ActionResult WeiXinProCollection(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            WeiXinProParams view = new WeiXinProParams();
            view.UID = entity.MediaID;
            view.CallIndex = "weixinpro";
            view.PageNum = 1;
            return PartialView("WeiXinProCollection", view);
        }
        public ActionResult Detail(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return View(entity);
        }
        [AllowAnonymous]
        public ActionResult Articles(string id, string kw = null)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(kw))
            {
                return PartialView("Articles", entity.MediaArticles.OrderByDescending(d => d.PublishDate).Take(20).ToList());
            }
            return PartialView("Articles", entity.MediaArticles.Where(d => !string.IsNullOrWhiteSpace(d.Content) && d.Content.Contains(kw)).OrderByDescending(d => d.PublishDate).Take(20).ToList());

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WeiXinProCollection(WeiXinProParams viewModel)
        {
            var premission = PremissionData();
            if (premission.Count > 0)
            {
                if (CheckRequest(viewModel.CallIndex, true))
                {
                    return Json(new { State = 0, Msg = "今日请求采集微信文章次数已用完" });
                }
            }
            viewModel.TransactorId = CurrentManager.Id;
            viewModel.Transactor = CurrentManager.UserName;
            var msg = _iDataAPIService.GetWeiXinArticles(viewModel);
            return Json(new { State = 1, Msg = msg.Message });
        }

        public ActionResult WeiboCollection(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            WeiBoParams view = new WeiBoParams();
            view.UID = entity.MediaID;
            view.CallIndex = "weibo";
            view.PageNum = 1;
            return PartialView("WeiboCollection", view);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WeiboCollection(WeiBoParams viewModel)
        {
            var premission = PremissionData();
            if (premission.Count > 0)
            {
                if (CheckRequest(viewModel.CallIndex))
                {
                    return Json(new { State = 0, Msg = "今日请求采集微博文章次数已用完" });
                }
            }
            viewModel.TransactorId = CurrentManager.Id;
            viewModel.Transactor = CurrentManager.UserName;
            var msg = _iDataAPIService.GetWeiBoArticles(viewModel);
            return Json(new { State = 1, Msg = msg.Message });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult WeixinCollection(WeiXinProParams baseParams)
        {
            var premission = PremissionData();
            if (premission.Count > 0)
            {
                if (CheckRequest(baseParams.CallIndex))
                {
                    return Json(new { State = 0, Msg = "今日请求采集微信信息次数已用完" });
                }
            }
            baseParams.TransactorId = CurrentManager.Id;
            baseParams.Transactor = CurrentManager.UserName;
            var msg = _iDataAPIService.GetWeiXinInfoPro(baseParams);
            return Json(new { State = 1, Msg = msg.Message });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult DouYinInfoCollection(DouYinParams baseParams)
        {
            var premission = PremissionData();
            if (premission.Count > 0)
            {
                if (CheckRequest(baseParams.CallIndex))
                {
                    return Json(new { State = 0, Msg = "今日请求采集抖音信息次数已用完" });
                }
            }
            baseParams.TransactorId = CurrentManager.Id;
            baseParams.Transactor = CurrentManager.UserName;
            var msg = _iDataAPIService.GetDouYinInfo(baseParams);
            return Json(new { State = 1, Msg = msg.Message });
        }
        [AllowAnonymous]
        public ActionResult FenPei()
        {
            string path = Server.MapPath("~/upload/fenpei.xlsx");
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
                    var id = row.GetCell(0)?.ToString();
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        continue;
                    }

                    var media = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
                    if (media != null)
                    {
                        var t1 = row.GetCell(1)?.ToString();
                        var t2 = row.GetCell(2)?.ToString();
                        if (!string.IsNullOrWhiteSpace(t1) && !string.IsNullOrWhiteSpace(t2))
                        {
                            media.Transactor = t1;
                            media.TransactorId = t2;
                        }
                        count++;
                    }



                }

                _dbContext.SaveChanges();
            }
            return Content("成功更新" + count + "条资源");
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
                case "douyin":
                case "redbook":
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
            msg = "此媒体资源已存在！媒体信息：" + media.MediaName + " - " + media.MediaID + "，经办媒介：" + media.Transactor;
            return media;
        }

        private bool CheckRequest(string apiCallIndex, bool isArticle = false)
        {
            var setting = _settingService.GetSetting<WeiGuang>();
            var start = DateTime.Now.Date;
            var end = DateTime.Now.Date.AddDays(1);
            var count = _apiRequestRecordRepository.LoadEntities(d =>
                  d.AddedById == CurrentManager.Id && d.APIInterfaces.CallIndex == apiCallIndex &&
                  d.ReponseDate >= start && d.ReponseDate < end).Count();
            if (isArticle)
            {
                return count > setting.RequestArticleCount;
            }
            return count > setting.RequestMediaCount;
        }
        private decimal SetSalePrice(decimal price, IEnumerable<Field> priceRanges)
        {
            if (price <= 0) return 0;
            foreach (var range in priceRanges)
            {
                var qj = range.Text.Split('-');
                if (price >= decimal.Parse(qj[0]) && price <= decimal.Parse(qj[1]))
                {
                    var value = decimal.Parse(range.Value);
                    return value <= 5 ? PriceZero(value * price) : PriceZero(value + price);
                }
            }
            return 0;
        }
        private decimal PriceZero(decimal a)
        {
            if (a >= 100000)
            {
                return (int)a / 1000 * 1000;
            }
            if (a >= 10000)
            {
                return (int)a / 1000 * 1000;
            }
            if (a >= 1000)
            {
                return (int)a / 100 * 100;
            }
            if (a >= 100)
            {
                return (int)a / 100 * 100;
            }
            return a;
        }
    }
}