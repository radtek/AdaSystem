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
    /// 媒介开发
    /// </summary>
    public class MediaDevelopProgressController : BaseController
    {
        private readonly IMediaDevelopService _service;
        private readonly IRepository<MediaDevelop> _repository;
        public MediaDevelopProgressController(IMediaDevelopService service,
            IRepository<Media> mediaRepository,
            IRepository<MediaDevelop> repository)
        {
            _service = service;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(MediaDevelopView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _service.LoadEntitiesFilter(viewModel,true).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new MediaDevelopView
                {
                    Id = d.Id,
                    MediaName = d.MediaName,
                    MediaID = d.MediaID,
                    MediaTypeName = d.MediaType.TypeName,
                    Platform = d.Platform,
                    Transactor = d.Transactor,
                    Content = d.Content,
                    Status = d.Status,
                    SubBy = d.SubBy,
                    SubDate = d.SubDate,
                    GetDate = d.GetDate
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddProgress(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("AddMediaDevelopProgress", entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProgress(MediaDevelopProgressView viewModel)
        {
            var entity = _repository.LoadEntities(d => d.Id == viewModel.MediaDevelopId).FirstOrDefault();
            //进度记录
            MediaDevelopProgress progress = new MediaDevelopProgress();
            progress.Id = IdBuilder.CreateIdNum();
            progress.ProgressContent = "资源开发中";
            progress.Remark = viewModel.ProgressContent;
            progress.ProgressDate = DateTime.Now;
            entity.MediaDevelopProgresses.Add(progress);
            _service.Update(entity);
            TempData["Msg"] = "操作成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Finish(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            //进度记录
            MediaDevelopProgress progress = new MediaDevelopProgress();
            progress.Id = IdBuilder.CreateIdNum();
            progress.ProgressContent = "开发完成";
            progress.Remark = "资源已成功开发完成，并已入媒体库。";
            progress.ProgressDate = DateTime.Now;
            entity.AuditDate=DateTime.Now;
            entity.Status = Consts.StateOK;
            entity.MediaDevelopProgresses.Add(progress);
            _service.Update(entity);
            return Json(new { State = 1, Msg = "操作成功" });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Cancle(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.Status==Consts.StateOK)
            {
                return Json(new { State = 0, Msg = "此资源已经开发完成，无法撤销" });
            }
            //进度记录
            MediaDevelopProgress progress = new MediaDevelopProgress();
            progress.Id = IdBuilder.CreateIdNum();
            progress.ProgressContent = "撤销资源认领";
            progress.Remark = "资源已被媒介人员：【" + entity.Transactor + "】撤销。";
            progress.ProgressDate = DateTime.Now;
            entity.Status = Consts.StateLock;
            entity.Transactor = null;
            entity.TransactorId = null;
            entity.GetDate = null;
            entity.MediaDevelopProgresses.Add(progress);
            _service.Update(entity);
            return Json(new { State = 1, Msg = "撤销成功" });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _service.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}