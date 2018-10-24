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
using WorkFlow.Models;
using WorkFlow.Template;

namespace WorkFlow.Controllers
{
    /// <summary>
    /// 审批
    /// </summary>
    public class ApproveController : BaseController
    {
        private readonly IWorkFlowService _service;
        private readonly IWorkFlowProvider _workFlowProvider;

        public ApproveController(IWorkFlowService service, IWorkFlowProvider workFlowProvider)
        {
            _service = service;
            _workFlowProvider = workFlowProvider;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Examination(string id)
        {
            return View();
        }
        /// <summary>
        /// 审批
        /// </summary>
        /// <returns></returns>
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Examination(WorkFlowRecordDetailView view)
        {
            var detail = _service.GetDetailById(view.Id);
            detail.ProcessResult = view.ProcessResult;
            detail.Status = (short) WorkFlowEnum.Processed;
            detail.ProcessDate=DateTime.Now;
            detail.ProcessComment = view.ProcessComment;
            //初始化下一个步骤
            WorkFlowRecordDetail nextDetail=new WorkFlowRecordDetail();
            nextDetail.Id = IdBuilder.CreateIdNum();
            nextDetail.IsEnd = false;
            nextDetail.IsStart = false;
            nextDetail.ProcessById = view.FlowTo;
            nextDetail.AddedDate=DateTime.Now;
            nextDetail.Status= (short)WorkFlowEnum.UnProecess;
            detail.WorkFlowRecord.WorkFlowRecordDetails.Add(nextDetail);
            //让书签继续往下执行。
            var isOk = view.ProcessResult=="通过" ? 1 : 0;
            _workFlowProvider.ResumeBookMark(
                detail.WorkFlowRecord.WorkFlowDefinition.ActityType,
                Guid.Parse(detail.WorkFlowRecord.WfInstanceId), 
                detail.Name,
                isOk);
            _service.UpdateDetail(detail);
            TempData["Msg"] = "操作成功";
            return RedirectToAction("Index");
        }
    }
}