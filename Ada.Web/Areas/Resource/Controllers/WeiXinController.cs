using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Resource;

namespace Resource.Controllers
{
    public class WeiXinController : BaseController
    {
        private readonly IMediaService _mediaService;
        private readonly IRepository<Media> _repository;
        private readonly IRepository<MediaTag> _mediaTagRepository;
        private readonly IMediaTypeService _mediaTypeService;
        private readonly IMediaTagService _mediaTagService;
        public WeiXinController(IMediaService mediaService,
            IRepository<Media> repository,
            IMediaTypeService mediaTypeService,
            IMediaTagService mediaTagService,
            IRepository<MediaTag> mediaTagRepository
        )
        {
            _mediaService = mediaService;
            _repository = repository;
            _mediaTypeService = mediaTypeService;
            _mediaTagService = mediaTagService;
            _mediaTagRepository = mediaTagRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(MediaView viewModel)
        {
            viewModel.MediaTypeIndex = "weixin";
            var result = _mediaService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new MediaView
                {
                    Id = d.Id,
                    MediaName = d.MediaName,
                    MediaID = d.MediaID,
                    IsAuthenticate = d.IsAuthenticate,
                    IsOriginal = d.IsOriginal,
                    IsComment = d.IsComment,
                    FansNum = d.FansNum,
                    ChannelType = d.ChannelType,
                    LastReadNum = d.LastReadNum,
                    AvgReadNum = d.AvgReadNum,
                    PublishFrequency = d.PublishFrequency,
                    Area = d.Area,
                    LastPushDate = d.LastPushDate,
                    AuthenticateType = d.AuthenticateType,
                    TransmitNum = d.TransmitNum,
                    CommentNum = d.CommentNum,
                    LikesNum = d.LikesNum,
                    Content = d.Content,
                    Status = d.Status,
                    ApiUpDate = d.ApiUpDate,
                    MediaLink = d.MediaLink,
                    MediaLogo = d.MediaLogo,
                    MediaQR = d.MediaQR,
                    LinkManId = d.LinkManId,
                    LinkManName = d.LinkMan.Name,
                    AddedBy = d.AddedBy,
                    MediaTagStr = string.Join(",", d.MediaTags.Select(t => t.TagName)),
                    MediaPrices = d.MediaPrices.Select(p => new MediaPriceView() { AdPositionName = p.AdPositionName, PriceDate = p.PriceDate, InvalidDate = p.InvalidDate, PurchasePrice = p.PurchasePrice }).ToList()
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            MediaView viewModel = new MediaView();
            var mediaType = _mediaTypeService.GetMediaTypeByCallIndex("weixin");
            viewModel.MediaTypeId = mediaType.Id;
            viewModel.Status = Consts.StateNormal;
            viewModel.MediaPrices = mediaType.AdPositions
                .Select(d => new MediaPriceView() { AdPositionName = d.Name, PurchasePrice = 0, AdPositionId = d.Id }).ToList();
            viewModel.MediaTags = _mediaTagService.GetTags()
                .Select(d => new SelectListItem() { Text = d.TagName, Value = d.Id }).ToList();
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(MediaView viewModel)
        {
            
            if (!ModelState.IsValid)
            {
                viewModel.MediaTags = _mediaTagService.GetTags()
                    .Select(d => new SelectListItem() { Text = d.TagName, Value = d.Id }).ToList();
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            //校验ID不能重复
            var temp = _repository.LoadEntities(d =>
                d.MediaID.Equals(viewModel.MediaID, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                d.MediaTypeId == viewModel.MediaTypeId).FirstOrDefault();
            if (temp != null)
            {
                viewModel.MediaTags = _mediaTagService.GetTags()
                    .Select(d => new SelectListItem() { Text = d.TagName, Value = d.Id }).ToList();
                ModelState.AddModelError("message", viewModel.MediaID + "，此微信公众号已存在！");
                return View(viewModel);
            }
            Media entity = new Media();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedById = CurrentManager.Id;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedDate = DateTime.Now;

            entity.MediaName = viewModel.MediaName;
            entity.MediaID = viewModel.MediaID;
            entity.MediaLink = viewModel.MediaLink;
            entity.MediaLogo = viewModel.MediaLogo;
            entity.MediaQR = viewModel.MediaQR;

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
            //entity.AuthenticateType = viewModel.AuthenticateType;
            //entity.TransmitNum = viewModel.TransmitNum;
            //entity.CommentNum = viewModel.CommentNum;
            //entity.LikesNum = viewModel.LikesNum;
            entity.Content = viewModel.Content;
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
            foreach (var viewModelMediaTagId in viewModel.MediaTagIds)
            {
                var tag = _mediaTagRepository.LoadEntities(d => d.Id == viewModelMediaTagId).FirstOrDefault();
                entity.MediaTags.Add(tag);
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
            entity.MediaName = item.MediaName;
            entity.IsAuthenticate = item.IsAuthenticate;
            entity.IsOriginal = item.IsOriginal;
            entity.IsComment = item.IsComment;
            entity.LastReadNum = item.LastReadNum;
            entity.AvgReadNum = item.AvgReadNum;
            entity.PublishFrequency = item.PublishFrequency;
            entity.Area = item.Area;
            entity.LastPushDate = item.LastPushDate;
            entity.AuthenticateType = item.AuthenticateType;
            entity.TransmitNum = item.TransmitNum;
            entity.CommentNum = item.CommentNum;
            entity.LikesNum = item.LikesNum;
            entity.Content = item.Content;
            entity.Status = item.Status;
            entity.MediaLink = item.MediaLink;
            entity.MediaLogo = item.MediaLogo;
            entity.MediaQR = item.MediaQR;
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
            //var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            //entity.ModifiedById = CurrentManager.Id;
            //entity.ModifiedBy = CurrentManager.UserName;
            //entity.ModifiedDate = DateTime.Now;
            //entity.TypeName = viewModel.TypeName;
            //entity.CallIndex = viewModel.CallIndex;
            //_mediaService.Update(entity, viewModel.AdPositions);
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
    }
}