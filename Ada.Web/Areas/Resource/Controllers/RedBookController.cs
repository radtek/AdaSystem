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
    /// <summary>
    /// 小红书
    /// </summary>
    public class RedBookController : BaseController
    {
        private readonly IMediaService _mediaService;
        private readonly IRepository<Media> _repository;
        private readonly IRepository<MediaTag> _mediaTagRepository;
        private readonly IRepository<MediaPrice> _mediaPriceRepository;
        private readonly IMediaTypeService _mediaTypeService;
        public RedBookController(IMediaService mediaService,
            IRepository<Media> repository,
            IMediaTypeService mediaTypeService,
            IRepository<MediaTag> mediaTagRepository,
            IRepository<MediaPrice> mediaPriceRepository
        )
        {
            _mediaService = mediaService;
            _repository = repository;
            _mediaTypeService = mediaTypeService;
            _mediaTagRepository = mediaTagRepository;
            _mediaPriceRepository = mediaPriceRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(MediaView viewModel)
        {
            viewModel.MediaTypeIndex = "redbook";
            viewModel.Managers = PremissionData();
            var result = _mediaService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new MediaView
                {
                    Id = d.Id,
                    MediaName = d.MediaName,
                    MediaLink = d.MediaLink,
                    LikesNum = d.LikesNum,
                    FansNum = d.FansNum,
                    Areas = d.Area,
                    Content = d.Content,
                    Remark = d.Remark,
                    Status = d.Status,
                    LinkManId = d.LinkManId,
                    LinkManName = d.LinkMan.Name,
                    Transactor = d.Transactor,
                    MediaGroups = d.MediaGroups.Select(g => new MediaGroupView() { Id = g.Id, GroupName = g.GroupName }).ToList(),
                    MediaTagStr = string.Join(",", d.MediaTags.Select(t => t.TagName)),
                    MediaPrices = d.MediaPrices.Select(p => new MediaPriceView()
                    {
                        AdPositionName = p.AdPositionName,
                        PriceDate = p.PriceDate,
                        InvalidDate = p.InvalidDate,
                        PurchasePrice = p.PurchasePrice
                    }).ToList()
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            MediaView viewModel = new MediaView();
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
            var temp = _repository.LoadEntities(d =>
                d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                d.MediaTypeId == viewModel.MediaTypeId).FirstOrDefault();
            if (temp != null)
            {
                ModelState.AddModelError("message", viewModel.MediaName + "，此小红书已存在！");
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
            entity.MediaLink = viewModel.MediaLink;
            entity.LikesNum = viewModel.LikesNum;
            entity.FansNum = viewModel.FansNum;
            entity.Area = viewModel.Areas;
            entity.Content = viewModel.Content;
            entity.Remark = viewModel.Remark;
            entity.Status = viewModel.Status;

            entity.ClickNum = viewModel.ClickNum;
            entity.IsHot = viewModel.IsHot;
            entity.IsSlide = viewModel.IsSlide;
            entity.IsRecommend = viewModel.IsRecommend;

            var mediaType = _mediaTypeService.GetMediaTypeByCallIndex("redbook");
            entity.MediaTypeId = mediaType.Id;
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
                entity.MediaTags.Clear();
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
            entity.MediaName = item.MediaName;
            entity.MediaLink = item.MediaLink;
            entity.LikesNum = item.LikesNum;
            entity.FansNum = item.FansNum;
            entity.Areas = item.Area;
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
            var temp = _repository.LoadEntities(d =>
                d.MediaName.Equals(viewModel.MediaName.Trim(), StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false &&
                d.MediaTypeId == viewModel.MediaTypeId && d.Id != viewModel.Id).FirstOrDefault();
            if (temp != null)
            {
                ModelState.AddModelError("message", viewModel.MediaName + "，此小红书已存在！");
                return View(viewModel);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            entity.MediaName = viewModel.MediaName.Trim();
            entity.MediaLink = viewModel.MediaLink;
            entity.LikesNum = viewModel.LikesNum;
            entity.FansNum = viewModel.FansNum;
            entity.Area = viewModel.Areas;
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
    }
}