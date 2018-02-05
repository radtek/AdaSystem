using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.ViewModel.API;
using Ada.Framework.Filter;
using Ada.Services.API;

namespace APIStore.Controllers
{
    public class RequestRecordController : BaseController
    {
        private readonly IAPIRequestRecordService _service;
        public RequestRecordController(IAPIRequestRecordService service)
        {
            _service = service;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(APIRequestRecordView viewModel)
        {
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new APIRequestRecordView
                {
                    Id = d.Id,
                    RequestParameters = d.RequestParameters,
                    Retcode = d.Retcode,
                    Retmsg = d.Retmsg,
                    IsSuccess = d.IsSuccess,
                    ReponseContent=d.ReponseContent,
                    ReponseDate = d.ReponseDate,
                    APIName = d.APIInterfaces.APIName
                })
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete()
        {
            var ids = Request["Ids"].Split(',');
            _service.Delete(ids);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}