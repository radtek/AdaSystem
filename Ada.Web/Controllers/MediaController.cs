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

        public MediaController(IRepository<Media> repository,
            IMediaService service,
            IOrderDetailCommentService orderDetailCommentService,
            IMediaCommentService mediaCommentService,
            ISettingService settingService,
            ICacheService cacheService,
            IMediaDevelopService mediaDevelopService)
        {
            _repository = repository;
            _service = service;
            _orderDetailCommentService = orderDetailCommentService;
            _mediaCommentService = mediaCommentService;
            _settingService = settingService;
            _cacheService = cacheService;
            _mediaDevelopService = mediaDevelopService;
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
        [AdaValidateAntiForgeryToken]
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
                    IsTop = d.IsTop,
                    MediaLogo = d.MediaLogo,
                    //CommentCount = d.MediaComments.Count+d.MediaPrices.Count(c=>c.BusinessOrderDetails.Count(o=>o.OrderDetailComments.Count>0)>0),
                    MediaTags = d.MediaTags.Select(t => new MediaTagView() { Id = t.Id, TagName = t.TagName }).Take(6).ToList(),
                    MediaPrices = d.MediaPrices.Select(p => new MediaPriceView() { AdPositionName = p.AdPositionName, PriceDate = p.PriceDate, InvalidDate = p.InvalidDate, SellPrice = p.SellPrice }).OrderByDescending(c => c.AdPositionName).ToList()
                })
            });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Export(MediaView search)
        {
            var setting = _settingService.GetSetting<WeiGuang>();
            //验证导出次数
            int times = 1;
            var obj = _cacheService.GetObject<int>(CurrentUser.Id + "UserExportTimes");
            if (obj != null)
            {
                times = (int)obj;
            }
            if (times > setting.UserExportTimes)
            {
                return Json(new { State = 0, Msg = "抱歉，今日导出的次数已用完！" });
            }

            search.Status = Consts.StateNormal;
            search.limit = setting.UserExportRows;
            if (!string.IsNullOrWhiteSpace(search.MediaBatch))
            {
                search.MediaBatch = search.MediaBatch.Trim().Replace("\r\n", ",").Replace("\n", ",").Replace("，", ",").Replace(" ", ",");
                var mediaNames = search.MediaBatch.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).Take(setting.UserExportRows).ToList();
                search.MediaBatch = string.Join(",", mediaNames);
            }
            var results = _service.LoadEntitiesFilter(search).AsNoTracking().ToList();
            var jObjects = ExprotTemplate(search, results);
            times++;
            var timeSpan = DateTime.Now.Date.AddDays(1) - DateTime.Now;
            _cacheService.Put(CurrentUser.Id + "UserExportTimes", times, timeSpan);
            return Json(new { State = 1, Msg = ExportData(jObjects.ToString()) });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Develop(MediaDevelopView viewModel)
        {
            var medias = viewModel.MediaName.Replace("\r\n", ",").Trim(',').Split(',').ToList();
            if (medias.Any())
            {
                List<MediaDevelop> list = new List<MediaDevelop>();
                foreach (var name in medias)
                {
                    MediaDevelop entity = new MediaDevelop();
                    entity.Id = IdBuilder.CreateIdNum();
                    entity.AddedById = CurrentUser.TransactorId;
                    entity.AddedBy = CurrentUser.Transactor;
                    entity.AddedDate = DateTime.Now;
                    entity.SubBy = entity.AddedBy;
                    entity.SubById = entity.AddedById;
                    entity.MediaName = name;
                    entity.Content = "来自网站申请";
                    entity.Status = Consts.StateLock;//待开发
                    entity.MediaTypeId = viewModel.MediaTypeId;
                    entity.SubDate = DateTime.Now;
                    //进度记录
                    MediaDevelopProgress progress = new MediaDevelopProgress();
                    progress.Id = IdBuilder.CreateIdNum();
                    progress.ProgressContent = "已提交申请";
                    progress.Remark = "等待媒介认领资源。";
                    progress.ProgressDate = DateTime.Now;
                    entity.MediaDevelopProgresses.Add(progress);
                    list.Add(entity);
                }
                _mediaDevelopService.AddRange(list);
            }
            return Json(new { State = 1, Msg = "申请成功" });
        }
        [HttpGet]
        [DeleteFile] //Action Filter, 下載完后自動刪除文件，這個屬性稍後解釋
        public ActionResult Download(string file)
        {
            string fullPath = Path.Combine(Server.MapPath("~/upload"), file);
            return File(fullPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file);
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
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
        [AdaValidateAntiForgeryToken]
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
                jo.Add("媒体名称", media.MediaName);
                if (fields.Contains("MediaID")) jo.Add("媒体ID", media.MediaID);
                jo.Add("粉丝数(万)", Utils.ShowFansNum(media.FansNum));

                if (fields.Contains("MediaLink")) jo.Add("媒体链接", media.MediaLink);
                if (fields.Contains("Sex")) jo.Add("性别", media.Sex);
                if (fields.Contains("Area")) jo.Add("地区", media.Area);
                if (fields.Contains("IsAuthenticate")) jo.Add("认证情况", media.IsAuthenticate == null ? "" : media.IsAuthenticate == true ? "已认证" : "未认证");
                if (fields.Contains("AuthenticateType")) jo.Add("等级", media.AuthenticateType);
                if (fields.Contains("AvgReadNum")) jo.Add("平均阅读数", media.AvgReadNum);
                if (fields.Contains("TransmitNum")) jo.Add("平均转发数", media.TransmitNum);
                if (fields.Contains("CommentNum")) jo.Add("平均评论数", media.CommentNum);
                if (fields.Contains("LikesNum")) jo.Add("平均点赞数", media.LikesNum);
                if (fields.Contains("PostNum")) jo.Add("发布总数", media.PostNum);
                //小红书
                if (fields.Contains("FriendNum")) jo.Add("关注数", media.FriendNum);
                if (fields.Contains("RedBookAvgReadNum")) jo.Add("平均点赞数", media.AvgReadNum);
                if (fields.Contains("RedBookTransmitNum")) jo.Add("平均收藏数", media.TransmitNum);
                if (fields.Contains("RedBookLikesNum")) jo.Add("赞与收藏", media.LikesNum);
                if (fields.Contains("RedBookPostNum")) jo.Add("笔记数", media.PostNum);
                
                if (fields.Contains("PublishFrequency")) jo.Add("月发布频次", media.PublishFrequency);
                if (fields.Contains("MonthPostNum")) jo.Add("最近月发文数", media.MonthPostNum);
                if (fields.Contains("LastPushDate")) jo.Add("最近发布日期", media.LastPushDate?.ToString("yyyy-MM-dd"));
                
                if (fields.Contains("SellPrice"))
                {
                    foreach (var mediaMediaPrice in media.MediaPrices)
                    {
                        var price = mediaMediaPrice.SellPrice ?? 0;
                        jo.Add(mediaMediaPrice.AdPositionName, price);
                    }
                    jo.Add("价格日期", media.MediaPrices.FirstOrDefault()?.InvalidDate?.ToString("yyyy-MM-dd"));
                }

                jObjects.Add(jo);
            }
            return jObjects;
        }

    }
}