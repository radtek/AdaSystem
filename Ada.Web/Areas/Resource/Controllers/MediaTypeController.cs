using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Customer;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Resource;

namespace Resource.Controllers
{
    public class MediaTypeController : BaseController
    {
        private readonly IMediaTypeService _mediaTypeService;
        private readonly IRepository<MediaType> _repository;
        public MediaTypeController(IMediaTypeService mediaTypeService,
            IRepository<MediaType> repository
        )
        {
            _mediaTypeService = mediaTypeService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(MediaTypeView viewModel)
        {
            var result = _mediaTypeService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new MediaTypeView
                {
                    Id = d.Id,
                    TypeName = d.TypeName,
                    CallIndex = d.CallIndex,
                    Taxis = d.Taxis
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(MediaTypeView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View();
            }
            MediaType entity = new MediaType();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedById = CurrentManager.Id;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedDate = DateTime.Now;
            entity.TypeName = viewModel.TypeName;
            entity.CallIndex = viewModel.CallIndex;
            entity.Taxis = viewModel.Taxis;
            _mediaTypeService.Add(entity,viewModel.AdPositions);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            MediaTypeView entity = new MediaTypeView();
            entity.Id = item.Id;
            entity.CallIndex = item.CallIndex;
            entity.TypeName = item.TypeName;
            entity.Taxis = item.Taxis;
            entity.AdPositions = item.AdPositions.Select(d => d.Name).ToList();
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(MediaTypeView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.TypeName = viewModel.TypeName;
            entity.CallIndex = viewModel.CallIndex;
            entity.Taxis = viewModel.Taxis;
            _mediaTypeService.Update(entity,viewModel.AdPositions);
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
            _mediaTypeService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}