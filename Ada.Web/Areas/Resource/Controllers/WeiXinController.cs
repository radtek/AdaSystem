using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
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
        private readonly IRepository<MediaType> _mediaTypeRepository;
        public WeiXinController(IMediaService mediaService,
            IRepository<Media> repository,
            IRepository<MediaType> mediaTypeRepository
        )
        {
            _mediaService = mediaService;
            _repository = repository;
            _mediaTypeRepository = mediaTypeRepository;
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
                    IsAuthenticate=d.IsAuthenticate,
                    IsOriginal=d.IsOriginal,
                    IsComment=d.IsComment,
                    FansNum=d.FansNum,
                    LastReadNum=d.LastReadNum,
                    AvgReadNum=d.AvgReadNum,
                    PublishFrequency=d.PublishFrequency,
                    Area=d.Area,
                    LastPushDate=d.LastPushDate,
                    AuthenticateType=d.AuthenticateType,
                    TransmitNum=d.TransmitNum,
                    CommentNum=d.CommentNum,
                    LikesNum=d.LikesNum,
                    Content=d.Content,
                    Status=d.Status,
                    ApiUpDate=d.ApiUpDate,
                    MediaLink = d.MediaLink,
                    MediaLogo = d.MediaLogo,
                    MediaQR = d.MediaQR,
                    LinkManId = d.LinkManId,
                    LinkManName = d.LinkMan.Name,
                    MediaTags = string.Join(",",d.MediaTags.Select(t=>t.TagName)),
                    MediaPrices = d.MediaPrices.Select(p=>new WeiXinPrice(){AdPosition = p.AdPositionName,PriceDate = p.PriceDate,InvalidDate = p.InvalidDate,PurchasePrice = p.PurchasePrice}).ToList()
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(MediaView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View();
            }
            Media entity = new Media();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedById = CurrentManager.Id;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedDate = DateTime.Now;
            entity.MediaName = viewModel.MediaName;
            entity.IsAuthenticate = viewModel.IsAuthenticate;
            entity.IsOriginal = viewModel.IsOriginal;
            entity.IsComment = viewModel.IsComment;
            entity.LastReadNum = viewModel.LastReadNum;
            entity.AvgReadNum = viewModel.AvgReadNum;
            entity.PublishFrequency = viewModel.PublishFrequency;
            entity.Area = viewModel.Area;
            entity.LastPushDate = viewModel.LastPushDate;
            entity.AuthenticateType = viewModel.AuthenticateType;
            entity.TransmitNum = viewModel.TransmitNum;
            entity.CommentNum = viewModel.CommentNum;
            entity.LikesNum = viewModel.LikesNum;
            entity.Content = viewModel.Content;
            entity.Status = viewModel.Status;
            entity.MediaLink = viewModel.MediaLink;
            entity.MediaLogo = viewModel.MediaLogo;
            entity.MediaQR = viewModel.MediaQR;
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