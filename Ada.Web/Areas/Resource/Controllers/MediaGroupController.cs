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
using Newtonsoft.Json;

namespace Resource.Controllers
{
    /// <summary>
    /// 媒体组
    /// </summary>
    public class MediaGroupController : BaseController
    {
        private readonly IMediaGroupService _mediaGroupService;
        private readonly IRepository<MediaGroup> _repository;
        private readonly IRepository<Media> _mediaRepository;
        public MediaGroupController(IMediaGroupService mediaGroupService,
            IRepository<MediaGroup> repository, IRepository<Media> mediaRepository
        )
        {
            _mediaGroupService = mediaGroupService;
            _repository = repository;
            _mediaRepository = mediaRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(MediaGroupView viewModel)
        {
            viewModel.GroupType = Consts.StateNormal;
            var result = _mediaGroupService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new MediaGroupView
                {
                    Id = d.Id,
                    GroupName = d.GroupName,
                    MediaViews = d.Medias.Select(m => new MediaView { MediaTypeName = m.MediaType.TypeName, MediaName = m.MediaName }).ToList()
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            MediaGroupView viewModel = new MediaGroupView();
            viewModel.MediaData = "[]";
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(MediaGroupView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            //校验唯一性
            var temp = _repository
                .LoadEntities(d => d.GroupName.Equals(viewModel.GroupName, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false)
                .FirstOrDefault();
            if (temp != null)
            {
                return Json(new { State = 0, Msg = viewModel.GroupName + "，此分组已存在！" });
            }
            MediaGroup entity = new MediaGroup();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedById = CurrentManager.Id;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedDate = DateTime.Now;
            entity.GroupName = viewModel.GroupName;
            entity.GroupType = Consts.StateNormal;//系统内部分组
            foreach (var id in viewModel.Medias)
            {
                var media = _mediaRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
                entity.Medias.Add(media);
            }
            _mediaGroupService.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Detail(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("MediaGroupDetail",entity);
        }
        public ActionResult Update(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            MediaGroupView viewModel = new MediaGroupView();
            viewModel.GroupName = entity.GroupName;
            viewModel.Id = id;
            viewModel.MediaData = JsonConvert.SerializeObject(entity.Medias.Select(d => new { id = d.Id, text = d.MediaName }));
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(MediaGroupView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            //校验唯一性
            var temp = _repository
                .LoadEntities(d => d.GroupName.Equals(viewModel.GroupName, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false && d.Id != viewModel.Id)
                .FirstOrDefault();
            if (temp != null)
            {
                return Json(new { State = 0, Msg = viewModel.GroupName + "，此分组已存在！" });
            }

            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.GroupName = viewModel.GroupName;
            entity.Medias.Clear();
            foreach (var id in viewModel.Medias)
            {
                var media = _mediaRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
                entity.Medias.Add(media);
            }
            _mediaGroupService.Update(entity);
            TempData["Msg"] = "修改成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _mediaGroupService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}