using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.WorkFlow;
using Ada.Core.Tools;
using Ada.Core.ViewModel.WorkFlow;
using Ada.Framework.Filter;
using Ada.Services.WorkFlow;
using Newtonsoft.Json.Linq;

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
                    WorkFlowDefinitionType = d.WorkFlowDefinition.WFType,
                    Remark = d.Remark

                })
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        
        public ActionResult Export(WorkFlowRecordView viewModel)
        {
            viewModel.limit = 3000;
            var result = _service.LoadRecordsFilter(viewModel).ToList();
            JArray jObjects = new JArray();
            foreach (var workFlowRecord in result)
            {
                var jo = new JObject();
                jo.Add("流程类型", workFlowRecord.WorkFlowDefinition.Name);
                jo.Add("申请人员", workFlowRecord.AddedBy);
                jo.Add("申请日期", workFlowRecord.AddedDate.Value.ToString("yyyy-MM-dd"));
                jo.Add("申请主题", workFlowRecord.Title);
                string jg;
                if (workFlowRecord.Status == 1)
                {
                    jg = workFlowRecord.WorkFlowRecordDetails.All(r => r.ProcessResult != "驳回") ? "申请通过" : "申请驳回";
                }
                else
                {
                    jg = "审批中";
                }
                jo.Add("流程状态", jg);
                jo.Add("申请内容", Utils.DropHtml(workFlowRecord.Content).Replace(" ","").Replace("请假条尊敬的领导：我因（请假原因：简单陈述即可）",""));
                jo.Add("备注", workFlowRecord.Remark);
                jObjects.Add(jo);
            }
            return Json(new {State = 1, Msg = ExportFile(jObjects.ToString(),"申请记录")});
        }
        /// <summary>
        /// 审批详情
        /// </summary>
        /// <returns></returns>
        public ActionResult Detail(string id)
        {
            var record = _service.GetRecordById(id);
            return View(record);
        }
        /// <summary>
        /// 删除审批记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string id)
        {
            _service.DeleteRecord(id);
            return Json(new { State = 1, Msg = "删除成功" });
        }
        /// <summary>
        /// 撤销审批记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Cancle(string id)
        {
            var record = _service.GetRecordById(id);
            if (record.Status == Consts.StateNormal)
            {
                return Json(new { State = 0, Msg = "流程已审批，无法撤销!" });
            }
            _service.DeleteRecord(id);
            return Json(new { State = 1, Msg = "撤销成功" });
        }
        /// <summary>
        /// 备注记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Remark(string id,string content)
        {
            var record = _service.GetRecordById(id);
            record.Remark = string.IsNullOrWhiteSpace(content) ? null : content;
            _service.UpdateRecord(record);
            return Json(new { State = 1, Msg = "保存成功" });
        }
    }
}