using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
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
        private readonly IRepository<Manager> _managerRepository;
        public ApproveController(IWorkFlowService service, 
            IWorkFlowProvider workFlowProvider,
            IRepository<Manager> managerRepository)
        {
            _service = service;
            _workFlowProvider = workFlowProvider;
            _managerRepository = managerRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Examination(string id)
        {
            var record = _service.GetRecordById(id);
            ViewBag.FlowTos = _managerRepository.LoadEntities(d => d.IsDelete == false && d.Status == Consts.StateNormal && d.Roles.Any(r => r.DataRange > 0)).ToList()
                .Select(d => new SelectListItem() { Text = string.Join(",", d.Organizations.Select(o => o.OrganizationName)) + " — " + d.UserName, Value = d.Id }).OrderByDescending(d => d.Text).ToList();
            return View(record);
        }
        public ActionResult GetList(WorkFlowRecordView viewModel)
        {
            viewModel.FlowTo = CurrentManager.Id;
            viewModel.MyApprove = true;
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
                    Level = d.Level,
                    Status = d.Status,
                    AddedBy = d.AddedBy
                    
                })
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 审批
        /// </summary>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Examination(WorkFlowRecordDetailView view)
        {
            var detail = _service.GetDetailById(view.Id);
            detail.ProcessResult = view.ProcessResult;
            detail.Status = (short)WorkFlowEnum.Processed;
            detail.ProcessDate = DateTime.Now;
            detail.ProcessComment = view.ProcessComment;
            //初始化下一个步骤
            
            WorkFlowRecordDetail nextDetail = new WorkFlowRecordDetail();
            nextDetail.Id = IdBuilder.CreateIdNum();
            nextDetail.IsEnd = false;
            nextDetail.IsStart = false;
            if (!string.IsNullOrWhiteSpace(view.FlowTo))
            {
                var nextProcess = _managerRepository.LoadEntities(d => d.Id == view.FlowTo).FirstOrDefault();
                nextDetail.ProcessById = view.FlowTo;
                nextDetail.ProcessBy = nextProcess.UserName;
            }
            nextDetail.AddedDate = DateTime.Now;
            nextDetail.Status = (short)WorkFlowEnum.UnProecess;
            detail.WorkFlowRecord.WorkFlowRecordDetails.Add(nextDetail);
            _service.UpdateDetail(detail);
            //让书签继续往下执行。
            var isOk = view.ProcessResult == "通过" ? 1 : 0;
            _workFlowProvider.ResumeBookMark(
                detail.WorkFlowRecord.WorkFlowDefinition.ActityType,
                Guid.Parse(detail.WorkFlowRecord.WfInstanceId),
                detail.Name,
                isOk);
            TempData["Msg"] = "操作成功";
            return RedirectToAction("Index");
        }
    }
}