using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Resource;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Filter;
using Ada.Framework.Messaging;
using Ada.Services.Admin;
using Ada.Services.Business;
using Ada.Services.Cache;
using Ada.Services.Resource;
using Ada.Services.Setting;
using Ada.Web.Models;
using Newtonsoft.Json.Linq;

namespace Ada.Web.Controllers
{
    public class MediaController : UserController
    {
        private readonly IRepository<Media> _repository;
        private readonly IMediaService _service;
        private readonly ISettingService _settingService;
        private readonly IMediaCommentService _mediaCommentService;
        private readonly IOrderDetailCommentService _orderDetailCommentService;
        private readonly ICacheService _cacheService;
        private readonly IMediaDevelopService _mediaDevelopService;
        private readonly IRepository<MediaGroup> _mediaGroupRepository;
        private readonly IRepository<Manager> _managerRepository;
        private readonly IRepository<MediaDevelop> _mediaDevelopRepository;
        private readonly IMediaGroupService _mediaGroupService;
        private readonly IMessageService _messageService;
        private readonly IManagerService _managerService;

        public MediaController(IRepository<Media> repository,
            IMediaService service,
            IOrderDetailCommentService orderDetailCommentService,
            IMediaCommentService mediaCommentService,
            ISettingService settingService,
            ICacheService cacheService,
            IMediaDevelopService mediaDevelopService,
            IRepository<MediaGroup> mediaGroupRepository,
            IMediaGroupService mediaGroupService,
            IMessageService messageService,
            IRepository<MediaDevelop> mediaDevelopRepository,
            IManagerService managerService,
            IRepository<Manager> managerRepository
            )
        {
            _repository = repository;
            _service = service;
            _orderDetailCommentService = orderDetailCommentService;
            _mediaCommentService = mediaCommentService;
            _settingService = settingService;
            _cacheService = cacheService;
            _mediaDevelopService = mediaDevelopService;
            _mediaGroupRepository = mediaGroupRepository;
            _mediaGroupService = mediaGroupService;
            _messageService = messageService;
            _mediaDevelopRepository = mediaDevelopRepository;
            _managerService = managerService;
            _managerRepository = managerRepository;

        }
        public ActionResult WeiXin()
        {
            return View();
        }
        public ActionResult WeiBo()
        {
            return View();
        }
        public ActionResult DouYin()
        {
            return View();
        }
        public ActionResult ZhiHu()
        {
            return View();
        }
        public ActionResult RedBook()
        {
            return View();
        }
        public ActionResult Taobao()
        {
            return View();
        }
        public ActionResult Bilibili()
        {
            return View();
        }
        public ActionResult Toutiao()
        {
            return View();
        }
        public ActionResult Other()
        {
            return View();
        }
        public ActionResult Writer()
        {
            return View();
        }
        public ActionResult Fans()
        {
            FansCalculator fansCalculator = new FansCalculator();
            fansCalculator.AreaGrade = 0;
            fansCalculator.Sex = 0;
            fansCalculator.FansTotal = 0;
            return View(fansCalculator);
        }
        [HttpPost]
        
        public ActionResult FansComputer(FansCalculator fansCalculator)
        {
            decimal price = 0;
            switch (fansCalculator.AreaGrade)
            {
                case 1:
                    {
                        switch (fansCalculator.Sex)
                        {
                            case 0:
                                price = 2 * fansCalculator.FansTotal;
                                break;
                            case 1:
                                price = 2.5M * fansCalculator.FansTotal;
                                break;
                            case 2:
                                price = 3 * fansCalculator.FansTotal;
                                break;
                        }

                        break;
                    }
                case 2:
                    {
                        switch (fansCalculator.Sex)
                        {
                            case 0:
                                price = 1.5M * fansCalculator.FansTotal;
                                break;
                            case 1:
                                price = 2 * fansCalculator.FansTotal;
                                break;
                            case 2:
                                price = 2.5M * fansCalculator.FansTotal;
                                break;
                        }

                        break;
                    }
                case 0:
                    {
                        switch (fansCalculator.Sex)
                        {
                            case 0:
                                price = 1 * fansCalculator.FansTotal;
                                break;
                            case 1:
                                price = 1.5M * fansCalculator.FansTotal;
                                break;
                            case 2:
                                price = 2 * fansCalculator.FansTotal;
                                break;
                        }

                        break;
                    }
            }

            return Json(new { State = 1, Msg = fansCalculator.Area+" " + fansCalculator.FansTotal + "个【" + fansCalculator.SexStr + "】粉丝 预估价格：" + price + "元" });
        }
        public ActionResult CommentDetail(string id)
        {
            var media = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return View(media);
        }
        public ActionResult Detail(string id)
        {
            var media = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return View(media);
        }
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
        
        public ActionResult GetList(MediaView viewModel)
        {
            viewModel.Status = Consts.StateNormal;
            var result = _service.LoadEntitiesFilter(viewModel).AsNoTracking().ToList();
            return Json(new
            {
                viewModel.total,
                no = viewModel.NoExistent,
                rows = result.Select(d => new MediaView
                {
                    Id = d.Id,
                    MediaName = d.MediaName,
                    MediaTypeIndex = d.MediaType.CallIndex,
                    MediaTypeName = d.MediaType.TypeName,
                    MediaID = d.MediaID,
                    MediaLink = d.MediaLink,
                    IsAuthenticate = d.IsAuthenticate,
                    IsOriginal = d.IsOriginal,
                    FansNum = Utils.ShowFansNum(d.FansNum),
                    ChannelType = d.ChannelType,
                    LastReadNum = d.LastReadNum,
                    AvgReadNum = d.AvgReadNum,
                    Areas = d.Area,
                    Sex = d.Sex,
                    Abstract = d.Abstract,
                    PostNum = d.PostNum,
                    MonthPostNum = d.MonthPostNum,
                    FriendNum = d.FriendNum,
                    Channel = d.Channel,
                    LastPushDate = d.LastPushDate,
                    AuthenticateType = d.AuthenticateType,
                    Platform = d.Platform,
                    TransmitNum = d.TransmitNum,
                    CommentNum = d.CommentNum,
                    LikesNum = d.LikesNum,
                    Content = d.Content,
                    Remark = d.Remark,
                    IsHot = d.IsHot,
                    PublishFrequency = d.PublishFrequency,
                    IsRecommend = d.IsRecommend,
                    IsComment = d.MediaType.IsComment,
                    IsTop = d.IsTop,
                    MediaLogo = d.MediaLogo,
                    ResourceType = d.ResourceType,
                    Efficiency = d.Efficiency,
                    PriceProtectionDate = d.PriceProtectionDate,
                    PriceProtectionIsPrePay = d.PriceProtectionIsPrePay,
                    PriceProtectionRemark = d.PriceProtectionRemark,
                    PriceProtectionIsBrand = d.PriceProtectionIsBrand,
                    IsGroup = d.MediaGroups.Any(g => g.GroupType == Consts.StateLock && g.AddedById == CurrentUser.Id),
                    MediaGroups = d.MediaGroups.Where(m => m.GroupType == Consts.StateNormal).Select(g => new MediaGroupView() { Id = g.Id, GroupName = g.GroupName }).ToList(),
                    MediaTags = d.MediaTags.Select(t => new MediaTagView() { Id = t.Id, TagName = t.TagName }).Take(6).ToList(),
                    MediaPrices = d.MediaPrices.Where(c => c.IsDelete == false).Select(p => new MediaPriceView() { AdPositionName = p.AdPositionName, PriceDate = p.PriceDate, InvalidDate = p.InvalidDate, SellPrice = p.SellPrice }).OrderByDescending(c => c.AdPositionName).ToList()
                })
            });
        }
        [HttpPost]
        
        public ActionResult Export(MediaView view)
        {
            var setting = _settingService.GetSetting<WeiGuang>();
            var configTimes = setting.UserExportTimes;
            var rows = setting.UserExportRows;
            //是否VIP
            var vips = setting.UserVIPGroup.Split(',').ToList();
            if (vips.Contains(CurrentUser.CommpanyName))
            {
                configTimes = configTimes * setting.UserVIPExportRatio;
                rows = rows * setting.UserVIPExportRatio;
            }
            //验证导出次数
            int times = 1;
            var obj = _cacheService.GetObject<int>(CurrentUser.Id + "UserExportTimes");
            if (obj != null)
            {
                times = (int)obj;
            }
            if (times > configTimes)
            {
                return Json(new { State = 0, Msg = "抱歉，今日导出的次数已用完！" });
            }

            view.Status = Consts.StateNormal;
            view.limit = rows;
            if (!string.IsNullOrWhiteSpace(view.MediaBatch))
            {
                view.MediaBatch = view.MediaBatch.Trim().Replace("\r\n", ",").Replace("\n", ",").Replace("\t", ",").Replace("，", ",");
                var mediaNames = view.MediaBatch.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).Take(rows).ToList();
                view.MediaBatch = string.Join(",", mediaNames);
            }
            var results = _service.LoadEntitiesFilter(view).AsNoTracking().ToList();
            var jObjects = ExprotTemplate(view, results);
            times++;
            var timeSpan = DateTime.Now.Date.AddDays(1) - DateTime.Now;
            _cacheService.Put(CurrentUser.Id + "UserExportTimes", times, timeSpan);
            return Json(new { State = 1, Msg = ExportData(jObjects.ToString()) });
        }
        [HttpPost]
        
        public ActionResult ExportGroup(string id, bool isData)
        {
            var setting = _settingService.GetSetting<WeiGuang>();
            var configTimes = setting.UserExportTimes;
            //是否VIP
            var vips = setting.UserVIPGroup.Split(',').ToList();
            if (vips.Contains(CurrentUser.CommpanyName))
            {
                configTimes = configTimes * setting.UserVIPExportRatio;
            }
            //验证导出次数
            int times = 1;
            var obj = _cacheService.GetObject<int>(CurrentUser.Id + "UserExportTimes");
            if (obj != null)
            {
                times = (int)obj;
            }
            if (times > configTimes)
            {
                return Json(new { State = 0, Msg = "抱歉，今日导出的次数已用完！" });
            }

            var group = _mediaGroupRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (group == null)
            {
                return Json(new { State = 0, Msg = "此分组不存在！" });
            }
            var rows = setting.UserExportGroupRows;
            var result = group.Medias.Where(d=>d.IsDelete==false&&d.Status==Consts.StateNormal).Take(rows).GroupBy(d => d.MediaType.TypeName);
            IDictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in result)
            {
                var jObjects = ExprotTemplate(item.ToList(), isData);
                dic.Add(item.Key, jObjects.ToString());
            }
            times++;
            var timeSpan = DateTime.Now.Date.AddDays(1) - DateTime.Now;
            _cacheService.Put(CurrentUser.Id + "UserExportTimes", times, timeSpan);
            return Json(new { State = 1, Msg = ExportData(dic) });
        }




        [HttpPost]
        
        public ActionResult Develop(MediaDevelopView viewModel)
        {
            var medias = viewModel.MediaName.Replace("\r\n", ",").Trim(',').Split(',').ToList();
            if (medias.Any())
            {
                List<MediaDevelop> list = new List<MediaDevelop>();
                foreach (var name in medias)
                {
                    var isExt = _mediaDevelopRepository
                        .LoadEntities(d => d.MediaTypeId == viewModel.MediaTypeId && d.MediaName == name).Any();
                    if (isExt)
                    {
                        continue;
                    }
                    MediaDevelop entity = new MediaDevelop
                    {
                        Id = IdBuilder.CreateIdNum(),
                        AddedById = CurrentUser.TransactorId,
                        AddedBy = CurrentUser.Transactor,
                        AddedDate = DateTime.Now
                    };
                    entity.SubBy = entity.AddedBy;
                    entity.SubById = entity.AddedById;
                    entity.MediaName = name;
                    entity.Content = "来自网站会员【" + CurrentUser.Name + "】的申请";
                    entity.Status = Consts.StateLock;//待开发
                    entity.MediaTypeId = viewModel.MediaTypeId;
                    entity.SubDate = DateTime.Now;
                    //进度记录
                    MediaDevelopProgress progress = new MediaDevelopProgress
                    {
                        Id = IdBuilder.CreateIdNum(),
                        ProgressContent = "已提交申请",
                        Remark = "等待媒介认领资源。",
                        ProgressDate = DateTime.Now
                    };
                    entity.MediaDevelopProgresses.Add(progress);
                    list.Add(entity);
                }

                if (list.Any())
                {
                    _mediaDevelopService.AddRange(list);
                    var config = _settingService.GetSetting<WeiGuang>();
                    var isPush = config.WebDevelopPush;
                    var names = string.Join(",", list.Select(d => d.MediaName));
                    if (list.Count > 5)
                    {
                        names = string.Join(",", list.Take(5).Select(d => d.MediaName)) + " 等";
                    }
                    if (isPush)
                    {
                        var openids = _managerService.GetByOrganizationName("媒介部")
                            .Where(d => !string.IsNullOrWhiteSpace(d.OpenId)).Select(d => d.OpenId).ToList();
                        var manager = _managerRepository.LoadEntities(d => d.Id == CurrentUser.TransactorId)
                            .FirstOrDefault();
                        if (manager != null)
                        {
                            if (!string.IsNullOrWhiteSpace(manager.OpenId))
                            {
                                openids.Add(manager.OpenId);
                            }
                        }
                        if (openids.Any())
                        {
                            var retrunUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                                      "/Resource/MediaDevelopAllot";
                            var url = Request.Url.Scheme + "://" + Request.Url.Authority +
                                     "/weixin/login/manager?returnUrl=" + Uri.EscapeDataString(retrunUrl);
                            var dic = new Dictionary<string, object>
                            {
                                {"Title", "来自会员VIP系统媒体开发申请，请及时认领处理\r\n"},
                                {"Remark", "\r\n点击详情查看"},
                                {"Url",url},
                                {"AppId", "wxcd1a304c25e0ea53"},
                                {"TemplateId", "y4eZb7aPr7tT8EXHi6r78jqsJx_Jw2EI_W7AYlc6D78"},
                                {"TemplateName", "开发申请处理提醒"},
                                {"OpenIds", string.Join(",",openids)},
                                {"KeyWord1", viewModel.MediaTypeName},
                                {"KeyWord2", names},
                                {"KeyWord3", CurrentUser.Name},
                                {"KeyWord4", DateTime.Now.ToString("yyyy-MM-dd HH:mm")}

                            };
                            _messageService.Send("Push", dic);
                        }

                    }
                }

            }
            return Json(new { State = 1, Msg = "申请成功" });
        }
        [HttpGet]
        [DeleteFile] //Action Filter, 下載完后自動刪除文件，這個屬性稍後解釋
        public ActionResult Download(string file)
        {

            string fullPath = Path.Combine(Server.MapPath("~/upload"), file);
            if (System.IO.File.Exists(fullPath))
            {
                return File(fullPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file);
            }
            return Content("无可用文件下载");
        }
        [HttpPost]
        
        public ActionResult GetOrderComments(MediaCommentView search)
        {
            var result = _orderDetailCommentService.LoadComments(search).ToList();
            return Json(new
            {
                search.total,
                avgScore = search.AvgScore,
                rows = result.Select(d => new
                {
                    d.Score,
                    Transactor = HideName(d.Transactor),
                    d.Content,
                    d.CommentDate
                })
            });
        }

        [HttpPost]
        
        public ActionResult GetMediaComments(MediaCommentView search)
        {
            var result = _mediaCommentService.LoadEntitiesFilter(search).ToList();
            return Json(new
            {
                search.total,
                avgScore = search.AvgScore,
                rows = result.Select(d => new
                {
                    d.Score,
                    Transactor = HideName(d.Transactor),
                    d.Content,
                    d.CommentDate
                })
            });
        }
        public ActionResult GroupDetail(string id)
        {
            var entity = _mediaGroupRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("GroupDetail", entity);
        }

        public ActionResult AddGroup(string name, string id)
        {
            ViewBag.MediaName = name;
            ViewBag.MediaId = id;
            var groups = _mediaGroupRepository.LoadEntities(d => d.AddedById == CurrentUser.Id);
            return PartialView("AddGroup", groups.ToList());
        }
        [HttpPost]
        
        public ActionResult AddGroup(MediaGroupView viewModel)
        {
            //组名不能超出10个字符
            if (viewModel.GroupName.Length > 10)
            {
                return Json(new { State = 0, Msg = "分组名称不能超出10个字符" });
            }
            //校验是否超出10个分组
            var count = _mediaGroupRepository.LoadEntities(d => d.AddedById == CurrentUser.Id && d.IsDelete == false).Count();
            if (count > 10)
            {
                return Json(new { State = 0, Msg = "最多只能建立10个分组" });
            }
            //校验唯一性
            var temp = _mediaGroupRepository
                .LoadEntities(d => d.GroupName.Equals(viewModel.GroupName, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false && d.AddedById == CurrentUser.Id && d.GroupType == Consts.StateLock)
                .FirstOrDefault();
            if (temp != null)
            {
                return Json(new { State = 0, Msg = viewModel.GroupName + "，此分组已存在" });
            }
            MediaGroup entity = new MediaGroup();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedById = CurrentUser.Id;
            entity.AddedBy = CurrentUser.Name;
            entity.AddedDate = DateTime.Now;
            entity.GroupName = viewModel.GroupName;
            entity.GroupType = Consts.StateLock;//用户分组
            _mediaGroupService.Add(entity);
            return Json(new { State = 1, Msg = entity.Id });
        }

        [HttpPost]
        
        public ActionResult JoinGroup(string mId, string gIds)
        {
            var groups = gIds.Split(',');
            var media = _repository.LoadEntities(d => d.Id == mId).FirstOrDefault();
            if (media != null)
            {
                _mediaGroupService.AddMedia(groups.ToList(), media,Consts.StateLock);
                return Json(new { State = 1, Msg = "加入成功" });
            }
            return Json(new { State = 0, Msg = "媒体资源不存在" });
        }
        [HttpPost]
        
        public ActionResult RemoveGroup(string mId, string gId)
        {

            var media = _repository.LoadEntities(d => d.Id == mId).FirstOrDefault();
            if (media != null)
            {
                _mediaGroupService.RemoveMedia(gId, media);
                return Json(new { State = 1, Msg = "移除成功" });
            }
            return Json(new { State = 0, Msg = "媒体资源不存在" });
        }
        [HttpPost]
        
        public ActionResult DeleteGroup(string id)
        {
            var group = _mediaGroupRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _mediaGroupService.Delete(group);
            return Json(new { State = 1, Msg = "删除成功" });
        }
        private string HideName(string name)
        {
            return name.Substring(0, 1) + "**";
        }
        private void GetData(MediaView viewModel)
        {
            viewModel.offset = viewModel.offset ?? 1;
            viewModel.limit = 10;
            var medias = _repository.LoadEntities(d =>
                d.MediaType.CallIndex == "weixin" && d.IsDelete == false && d.Status == Consts.StateNormal &&
                d.IsSlide == true);
            if (!string.IsNullOrWhiteSpace(viewModel.MediaName))
                medias = medias.Where(d =>
                    d.MediaName.Contains(viewModel.MediaName) || d.MediaID.Contains(viewModel.MediaName));
            if (viewModel.MediaTagIds != null)
                medias = medias.Include(d => d.MediaTags).Where(d => d.MediaTags.Any(t => viewModel.MediaTagIds.Contains(t.Id)));
            if (!string.IsNullOrWhiteSpace(viewModel.FansNumRange))
            {
                var temp = viewModel.FansNumRange.Split('-');
                int min = Convert.ToInt32(temp[0].Trim()) * 10000;
                viewModel.FansNumStart = Convert.ToInt32(temp[0].Trim());
                int max = Convert.ToInt32(temp[1].Trim()) * 10000;
                viewModel.FansNumEnd = Convert.ToInt32(temp[1].Trim());
                medias = medias.Where(d => d.FansNum >= min && d.FansNum <= max);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.PriceRange))
            {
                var temp = viewModel.PriceRange.Split('-');
                decimal min = Convert.ToDecimal(temp[0].Trim());
                decimal max = Convert.ToDecimal(temp[1].Trim());
                viewModel.PriceStart = min;
                viewModel.PriceEnd = max;
                medias = !string.IsNullOrWhiteSpace(viewModel.AdPositionName) ?
                    medias.Include(d => d.MediaPrices).Where(d => d.MediaPrices.Any(p => p.AdPositionName == viewModel.AdPositionName && p.PurchasePrice >= min && p.PurchasePrice <= max)) :
                    medias.Include(d => d.MediaPrices).Where(d => d.MediaPrices.Any(p => p.PurchasePrice >= min && p.PurchasePrice <= max));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AvgReadNumRange))
            {
                var temp = viewModel.AvgReadNumRange.Split('-');
                decimal min = Convert.ToDecimal(temp[0].Trim());
                decimal max = Convert.ToDecimal(temp[1].Trim());
                viewModel.AvgReadNumStart = (int?)min;
                viewModel.AvgReadNumEnd = (int?)max;
                medias = medias.Where(d => d.AvgReadNum >= min && d.AvgReadNum <= max);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaNames))
            {
                viewModel.MediaNames = viewModel.MediaNames.Trim().Replace("\r\n", ",").Replace("，", ",").Replace(" ", ",");
                var mediaNames = viewModel.MediaNames.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
                medias = medias.Where(d => mediaNames.Contains(d.MediaName) || mediaNames.Contains(d.MediaID));
            }
            viewModel.total = medias.Count();
            medias = medias.OrderByDescending(d => d.IsTop).ThenByDescending(d => d.IsHot).ThenByDescending(d => d.IsRecommend).ThenBy(d => d.Id).Skip(viewModel.limit.Value * (viewModel.offset.Value - 1))
                 .Take(viewModel.limit.Value);
            viewModel.Medias = medias.AsNoTracking().ToList();
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
            if (noDatas.Any())
            {
                results.AddRange(noDatas);
            }
            var fields = string.IsNullOrWhiteSpace(viewModel.ExcelField) ? new List<string>() : viewModel.ExcelField.Split(',').ToList();
            foreach (var media in results.OrderBy(d => d.Taxis))
            {
                var jo = new JObject();
                jo.Add("媒体状态", string.IsNullOrWhiteSpace(media.Id) ? "不存在" : "正常");
                if (fields.Contains("MediaTags")) jo.Add("媒体分类", string.Join(",", media.MediaTags.Select(d => d.TagName)));
                if (fields.Contains("Platform")) jo.Add("媒体平台", media.Platform);
                jo.Add("媒体名称", media.MediaName);
                if (fields.Contains("MediaID")) jo.Add("媒体ID", media.MediaID);
                jo.Add("粉丝数(万)", Utils.ShowFansNum(media.FansNum));
                if (fields.Contains("Sex")) jo.Add("性别", media.Sex);
                if (fields.Contains("Area")) jo.Add("地区", media.Area);
                if (fields.Contains("IsAuthenticate")) jo.Add("认证情况", media.IsAuthenticate == null ? "" : media.IsAuthenticate == true ? "已认证" : "未认证");
                if (fields.Contains("AuthenticateType")) jo.Add("等级", media.AuthenticateType);
                if (fields.Contains("AvgReadNum")) jo.Add("平均阅读数", media.AvgReadNum);
                if (fields.Contains("TransmitNum")) jo.Add("平均转发数", media.TransmitNum);
                if (fields.Contains("CommentNum")) jo.Add("平均评论数", media.CommentNum);
                if (fields.Contains("LikesNum")) jo.Add("平均点赞数", media.LikesNum);
                if (fields.Contains("PostNum")) jo.Add("发布总数", media.PostNum);
                //淘宝
                if (fields.Contains("TaobaoAvgReadNum")) jo.Add("综合能力指数", media.AvgReadNum);
                //小红书
                if (fields.Contains("FriendNum")) jo.Add("关注数", media.FriendNum);
                if (fields.Contains("RedBookAvgReadNum")) jo.Add("平均点赞数", media.AvgReadNum);
                if (fields.Contains("RedBookTransmitNum")) jo.Add("平均收藏数", media.TransmitNum);
                if (fields.Contains("RedBookLikesNum")) jo.Add("赞与收藏", media.LikesNum);
                if (fields.Contains("RedBookPostNum")) jo.Add("笔记数", media.PostNum);
                //B站
                if (fields.Contains("BilibiliLikesNum")) jo.Add("播放数", media.LikesNum);
                if (fields.Contains("BilibiliAvgReadNum")) jo.Add("阅读数", media.AvgReadNum);
                

                if (fields.Contains("PublishFrequency")) jo.Add("月发布频次", media.PublishFrequency);
                if (fields.Contains("MonthPostNum")) jo.Add("最近月发文数", media.MonthPostNum);
                if (fields.Contains("LastPushDate")) jo.Add("最近发布日期", media.LastPushDate?.ToString("yyyy-MM-dd"));
                //文案
                if (fields.Contains("ResourceType")) jo.Add("擅长类型", media.ResourceType);
                if (fields.Contains("Efficiency")) jo.Add("出稿速度", media.Efficiency);
                if (fields.Contains("SellPrice"))
                {
                    jo.Add("价格日期", media.MediaPrices.FirstOrDefault()?.InvalidDate?.ToString("yyyy-MM-dd"));
                    var priceList = media.MediaPrices.Where(d => d.IsDelete == false).ToList();
                    foreach (var mediaMediaPrice in priceList)
                    {
                        var price = mediaMediaPrice.SellPrice ?? 0;
                        jo.Add(mediaMediaPrice.AdPositionName + "[税前]", price);
                    }
                    foreach (var mediaMediaPrice in priceList)
                    {
                        var price = mediaMediaPrice.SellPrice ?? 0;
                        jo.Add(mediaMediaPrice.AdPositionName + "[税后]", price * 1.06M);
                    }
                }
                if (fields.Contains("MediaLink")) jo.Add("媒体链接", media.MediaLink);
                jObjects.Add(jo);
            }
            return jObjects;
        }
        private JArray ExprotTemplate(List<Media> results, bool isData)
        {

            JArray jObjects = new JArray();
            foreach (var media in results)
            {
                var jo = new JObject();
                jo.Add("媒体状态", string.IsNullOrWhiteSpace(media.Id) ? "不存在" : "正常");
                jo.Add("媒体分类", string.Join(",", media.MediaTags.Select(d => d.TagName)));
                if (!string.IsNullOrWhiteSpace(media.Platform))
                {
                    jo.Add("媒体平台", media.Platform);
                }
                jo.Add("媒体名称", media.MediaName);
                jo.Add("粉丝数(万)", Utils.ShowFansNum(media.FansNum));
                if (isData)
                {
                    switch (media.MediaType.CallIndex)
                    {
                        case "weixin":
                            jo.Add("微信号", media.MediaID);
                            jo.Add("认证情况", media.IsAuthenticate == null ? "" : media.IsAuthenticate == true ? "已认证" : "未认证");
                            jo.Add("平均阅读数", media.AvgReadNum);
                            jo.Add("月发布频次", media.PublishFrequency);
                            jo.Add("最近月发文数", media.MonthPostNum);
                            jo.Add("最近发布日期", media.LastPushDate?.ToString("yyyy-MM-dd"));
                            break;
                        case "sinablog":
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
                            jo.Add("性别", media.Sex);
                            jo.Add("地区", media.Area);
                            jo.Add("认证情况", media.IsAuthenticate == null ? "" : media.IsAuthenticate == true ? "已认证" : "未认证");
                            jo.Add("平均转发数", media.TransmitNum);
                            jo.Add("平均浏览数", media.AvgReadNum);
                            jo.Add("平均评论数", media.CommentNum);
                            jo.Add("平均点赞数", media.LikesNum);
                            break;
                        case "redbook":
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
                            jo.Add("地区", media.Area);
                            break;
                        case "taobao":
                            jo.Add("地区", media.Area);
                            jo.Add("综合能力指数", media.AvgReadNum);
                            break;
                        case "bilibili":
                            jo.Add("地区", media.Area);
                            jo.Add("关注数", media.FriendNum);
                            jo.Add("播放数", media.LikesNum);
                            jo.Add("阅读数", media.AvgReadNum);
                            break;
                        case "toutiao":
                            jo.Add("认证情况", media.IsAuthenticate == null ? "" : media.IsAuthenticate == true ? "已认证" : "未认证");
                            jo.Add("关注数", media.FriendNum);
                            jo.Add("平均阅读数", media.AvgReadNum);
                            jo.Add("平均评论数", media.CommentNum);
                            jo.Add("最近发布日期", media.LastPushDate?.ToString("yyyy-MM-dd"));
                            break;
                        case "writer":
                            jo.Add("擅长类型", media.ResourceType);
                            jo.Add("出稿速度", media.Efficiency);
                            break;
                       
                    }
                }
                foreach (var mediaMediaPrice in media.MediaPrices.Where(d => d.IsDelete == false))
                {
                    var price = mediaMediaPrice.SellPrice ?? 0;
                    jo.Add(mediaMediaPrice.AdPositionName, price);
                }
                jo.Add("价格日期", media.MediaPrices.FirstOrDefault()?.InvalidDate?.ToString("yyyy-MM-dd"));
                if (media.MediaType.CallIndex!="taobao")
                {
                    jo.Add("媒体链接", media.MediaLink);
                }
                jObjects.Add(jo);
            }
            return jObjects;
        }
        public ActionResult Price(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("MediaPrice", entity);
        }
    }
}