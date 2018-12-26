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
    public class MediaTagController : BaseController
    {
        private readonly IMediaTagService _mediaTagService;
        private readonly IRepository<MediaTag> _repository;
        public MediaTagController(IMediaTagService mediaTagService,
            IRepository<MediaTag> repository
        )
        {
            _mediaTagService = mediaTagService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(MediaTagView viewModel)
        {
            var result = _mediaTagService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new MediaTagView
                {
                    Id = d.Id,
                    TagName = d.TagName,
                    Taxis = d.Taxis
                })
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        
        public ActionResult AddOrUpdate(MediaTagView viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { State = 0, Msg = "数据校验失败，请核对输入的信息是否准确" });
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Id))
            {
                //校验唯一性
                var temp = _repository
                    .LoadEntities(d => d.TagName.Equals(viewModel.TagName, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false && d.Id != viewModel.Id)
                    .FirstOrDefault();
                if (temp != null)
                {
                    return Json(new { State = 0, Msg =viewModel.TagName + "，已存在！" });
                }
                var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
                entity.ModifiedById = CurrentManager.Id;
                entity.ModifiedBy = CurrentManager.UserName;
                entity.ModifiedDate = DateTime.Now;
                entity.TagName = viewModel.TagName;
                entity.Taxis = viewModel.Taxis;
                _mediaTagService.Update(entity);
                return Json(new { State = 1, Msg = "更新成功" });
            }
            else
            {
                //校验唯一性
                var temp = _repository
                    .LoadEntities(d => d.TagName.Equals(viewModel.TagName, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false)
                    .FirstOrDefault();
                if (temp != null)
                {
                    return Json(new { State = 0, Msg = viewModel.TagName + "，已存在！" });
                }
                MediaTag entity = new MediaTag();
                entity.Id = IdBuilder.CreateIdNum();
                entity.AddedById = CurrentManager.Id;
                entity.AddedBy = CurrentManager.UserName;
                entity.AddedDate = DateTime.Now;
                entity.TagName = viewModel.TagName;
                entity.Taxis = viewModel.Taxis;
                _mediaTagService.Add(entity);
                return Json(new { State = 1, Msg = "添加成功" });
            }
        }
       
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _mediaTagService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}