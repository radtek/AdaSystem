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
    /// 媒体开发申请页
    /// </summary>
    public class MediaDevelopController : BaseController
    {
        private readonly IMediaDevelopService _service;
        private readonly IRepository<MediaDevelop> _repository;
        public MediaDevelopController(IMediaDevelopService service,
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
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new MediaDevelopView
                {
                    Id = d.Id,
                    MediaName = d.MediaName,
                    MediaID = d.MediaID,
                    MediaTypeName = d.MediaType.TypeName,
                    Transactor = d.Transactor,
                    Platform = d.Platform,
                    Content = d.Content,
                    Status = d.Status,
                    SubBy = d.SubBy,
                    SubDate = d.SubDate
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        
        public ActionResult Add(MediaDevelopView viewModel)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }

            var names = viewModel.MediaName.Replace("\r\n", ",").Trim(',').Split(',');
            List<MediaDevelop> list = new List<MediaDevelop>();
            foreach (var name in names)
            {
                MediaDevelop entity = new MediaDevelop();
                entity.Id = IdBuilder.CreateIdNum();
                entity.AddedById = CurrentManager.Id;
                entity.AddedBy = CurrentManager.UserName;
                entity.AddedDate = DateTime.Now;
                entity.SubBy = CurrentManager.UserName;
                entity.SubById = CurrentManager.Id;
                entity.MediaName = name;
                entity.Content = viewModel.Content;
                entity.Status = Consts.StateLock;//待开发
                entity.MediaTypeId = viewModel.MediaTypeId;
                entity.SubDate=DateTime.Now;
                //进度记录
                MediaDevelopProgress progress=new MediaDevelopProgress();
                progress.Id = IdBuilder.CreateIdNum();
                progress.ProgressContent = "已提交申请";
                progress.Remark = "等待媒介认领资源。";
                progress.ProgressDate=DateTime.Now;
                entity.MediaDevelopProgresses.Add(progress);
                list.Add(entity);
            }
            _service.AddRange(list);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Add");
        }
        public ActionResult Progress(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("MediaDevelopProgress",entity);
        }
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.Status!=Consts.StateLock)
            {
                return Json(new { State = 0, Msg = "此媒体正在开发中，无法删除。如需删除请联系媒介撤销开发后，再进行删除！" });
            }
            _service.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}