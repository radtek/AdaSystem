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
    /// <summary>
    /// 媒体组
    /// </summary>
    public class MediaGroupController : BaseController
    {
        private readonly IMediaGroupService _mediaGroupService;
        private readonly IRepository<MediaGroup> _repository;
        public MediaGroupController(IMediaGroupService mediaGroupService,
            IRepository<MediaGroup> repository
        )
        {
            _mediaGroupService = mediaGroupService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(MediaGroupView viewModel)
        {
            var result = _mediaGroupService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new MediaGroupView
                {
                    Id = d.Id,
                    GroupName = d.GroupName
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
        [AdaValidateAntiForgeryToken]
        public ActionResult Add(MediaGroupView viewModel)
        {
            if (ModelState.IsValid)
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
                return Json(new { State = 0, Msg = viewModel.GroupName + "，已存在！" });
            }
            MediaGroup entity = new MediaGroup();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedById = CurrentManager.Id;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedDate = DateTime.Now;
            entity.GroupName = viewModel.GroupName;
            _mediaGroupService.Add(entity);
            TempData["Msg"] = "添加成功";
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