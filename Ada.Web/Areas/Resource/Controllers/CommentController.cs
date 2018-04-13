using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Resource;

namespace Resource.Controllers
{
    public class CommentController : BaseController
    {
        private readonly IMediaCommentService _service;
        public CommentController(IMediaCommentService service)
        {
            _service = service;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(MediaCommentView viewModel)
        {
            var result = _service.LoadEntitiesFilter(viewModel).AsNoTracking().ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new
                {
                    d.Id,
                    d.Transactor,
                    d.CommentDate,
                    d.Content,
                    d.Score,
                    d.Media.MediaName,
                    d.Media.MediaID,
                    d.Media.MediaType.TypeName,
                    d.MediaId
                })
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
            {
                return Json(new { State = 0, Msg = "请选择要删除的数据" });
            }
            var arr = ids.Split(',');
            _service.Delete(arr);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}