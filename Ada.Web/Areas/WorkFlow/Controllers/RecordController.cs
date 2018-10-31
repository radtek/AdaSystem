using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.WorkFlow;
using Ada.Core.ViewModel.WorkFlow;
using Ada.Framework.Filter;
using Ada.Services.WorkFlow;

namespace WorkFlow.Controllers
{
    /// <summary>
    /// 流程记录
    /// </summary>
    public class RecordController : BaseController
    {
        private readonly IWorkFlowService _service;
        private readonly IRepository<WorkFlowDefinition> _repository;
        public RecordController(IWorkFlowService service, IRepository<WorkFlowDefinition> repository)
        {
            _service = service;
            _repository = repository;
        }
        public ActionResult Index()
        {
            ViewBag.WFS = _repository.LoadEntities(d => d.Enabled).ToList();
            return View();
        }
        public ActionResult GetList(WorkFlowRecordView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _service.LoadRecordsFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new WorkFlowRecordView
                {
                    Id = d.Id,
                    Title = d.Title,
                    WorkFlowDefinitionName = d.WorkFlowDefinition.Name,
                    AddedDate = d.AddedDate,
                    Status = d.Status,
                    AddedBy = d.AddedBy,
                    Result = d.WorkFlowRecordDetails.All(r => r.ProcessResult != "驳回"),
                    WorkFlowDefinitionType = d.WorkFlowDefinition.WFType

                })
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 审批详情
        /// </summary>
        /// <returns></returns>
        public ActionResult Detail(string id)
        {
           var record= _service.GetRecordById(id);
            return View(record);
        }
        /// <summary>
        /// 删除审批记录
        /// </summary>
        /// <returns></returns>
        [HttpPost,AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            _service.DeleteRecord(id);
            return Json(new{State=1,Msg="删除成功"});
        }
    }
}