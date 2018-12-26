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
    /// 开发资源认领
    /// </summary>
    public class MediaDevelopAllotController : BaseController
    {
        private readonly IMediaDevelopService _service;
        private readonly IRepository<MediaDevelop> _repository;
        public MediaDevelopAllotController(IMediaDevelopService service,
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
            viewModel.Status = Consts.StateLock;
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
                    Platform = d.Platform,
                    Content = d.Content,
                    Status = d.Status,
                    SubBy = d.SubBy,
                    SubDate = d.SubDate
                })
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        
        public ActionResult Allot(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.Status = Consts.StateNormal;
            entity.Transactor = CurrentManager.UserName;
            entity.TransactorId = CurrentManager.Id;
            entity.GetDate = DateTime.Now;
            //进度记录
            MediaDevelopProgress progress = new MediaDevelopProgress();
            progress.Id = IdBuilder.CreateIdNum();
            progress.ProgressContent = "已被认领";
            progress.Remark = "资源已被媒介人员：【" + entity.Transactor + "】认领开发";
            progress.ProgressDate = DateTime.Now;
            entity.MediaDevelopProgresses.Add(progress);
            _service.Update(entity);
            return Json(new { State = 1, Msg = "认领成功" });
        }
    }
}