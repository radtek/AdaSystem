using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API;
using Ada.Framework.Filter;
using Ada.Services.API;
using Newtonsoft.Json.Linq;

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
        [ValidateAntiForgeryToken]
        public ActionResult Export(APIRequestRecordView viewModel)
        {
            viewModel.limit = 10000;
            return File(ExportData(ExportExcel(viewModel)), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
        }
        private string ExportExcel(APIRequestRecordView viewModel)
        {
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            JArray jObjects = new JArray();
            foreach (var item in result)
            {
                var jo = new JObject();
                jo.Add("API名称", item.APIInterfaces.APIName);
                jo.Add("请求参数", item.RequestParameters);
                jo.Add("返回码", item.Retcode);
                jo.Add("错误信息", item.Retmsg);
                jo.Add("响应内容", item.ReponseContent);
                jo.Add("响应时间",item.ReponseDate);
                jObjects.Add(jo);
            }

            return jObjects.ToString();
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