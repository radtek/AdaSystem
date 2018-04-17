using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Business;

namespace Business.Controllers
{
    public class CommentController : BaseController
    {
        private readonly IOrderDetailCommentService _service;
        public CommentController(IOrderDetailCommentService service)
        {
            _service = service;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(MediaCommentView viewModel)
        {
            var result = _service.LoadComments(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result
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
            _service.Remove(arr);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}