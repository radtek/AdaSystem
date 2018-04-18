using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Ada.Services.Business;
using Ada.Services.Resource;
using Ada.Web.Models;

namespace Ada.Web.Controllers
{
    public class MediaController : UserController
    {
        private readonly IRepository<Media> _repository;
        private readonly IMediaService _service;
        private readonly IMediaCommentService _mediaCommentService;
        private readonly IOrderDetailCommentService _orderDetailCommentService;

        public MediaController(IRepository<Media> repository,
            IMediaService service, 
            IOrderDetailCommentService orderDetailCommentService,
            IMediaCommentService mediaCommentService)
        {
            _repository = repository;
            _service = service;
            _orderDetailCommentService = orderDetailCommentService;
            _mediaCommentService = mediaCommentService;
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
        public ActionResult Detail(string id)
        {
            var media = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return View(media);
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
                rows = result.Select(d => new MediaView
                {
                    Id = d.Id,
                    MediaName = d.MediaName,
                    MediaTypeIndex = d.MediaType.CallIndex,
                    MediaID = d.MediaID,
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
                    MediaPrices = d.MediaPrices.Select(p => new MediaPriceView() { AdPositionName = p.AdPositionName, PriceDate = p.PriceDate, InvalidDate = p.InvalidDate, SellPrice = p.SellPrice }).ToList()
                })
            });
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


    }
}