using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.WorkFlow;
using Ada.Core.ViewModel.Setting;
using Ada.Core.ViewModel.WorkFlow;
using Ada.Framework.Filter;
using Ada.Framework.Messaging;
using Ada.Services.Setting;
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
        private readonly IMessageService _messageService;
        private readonly ISettingService _settingService;
        public ApproveController(IWorkFlowService service, 
            IWorkFlowProvider workFlowProvider,
            IRepository<Manager> managerRepository,
            IMessageService messageService,
            ISettingService settingService)
        {
            _service = service;
            _workFlowProvider = workFlowProvider;
            _managerRepository = managerRepository;
            _messageService = messageService;
            _settingService = settingService;
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
            Manager nextProcess=null;
            if (!string.IsNullOrWhiteSpace(view.FlowTo)&&view.FlowTo!="1")
            {
                nextProcess = _managerRepository.LoadEntities(d => d.Id == view.FlowTo).FirstOrDefault();
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
            //微信提醒
            var setting = _settingService.GetSetting<WeiGuang>();
            if (setting.WorkFlowPush)
            {
                if (nextProcess != null)
                {
                    if (!string.IsNullOrWhiteSpace(nextProcess.OpenId))
                    {
                        var retrunUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                                        "/WorkFlow/Approve/Examination/" + detail.WorkFlowRecord.Id;
                        var url = Request.Url.Scheme + "://" + Request.Url.Authority +
                                  "/weixin/login/manager?returnUrl=" + Uri.EscapeDataString(retrunUrl);
                        var dic = new Dictionary<string, object>
                    {
                        {"Title", "您有新的申请需要审核！\r\n"},
                        {"Remark", "\r\n点击详情进行审批"},
                        {"Url",url},
                        {"AppId", "wxcd1a304c25e0ea53"},
                        {"TemplateId", "1iKylsb9ogt5eH9vUsIQOVCqvnsnYTIPWFbr-6ZY8mY"},
                        {"TemplateName", "申请审核通知"},
                        {"OpenIds", nextProcess.OpenId},
                        {"KeyWord1", detail.WorkFlowRecord.Title},
                        {"KeyWord2", detail.WorkFlowRecord.AddedBy},
                        {"KeyWord3", detail.WorkFlowRecord.WorkFlowDefinition.Name},
                        {"KeyWord4", detail.WorkFlowRecord.AddedDate}

                    };
                        _messageService.Send("Push", dic);
                    }

                }
                //通知申请人
                var recordBy = _managerRepository.LoadEntities(d => d.Id == detail.WorkFlowRecord.AddedById).FirstOrDefault();
                if (recordBy != null)
                {
                    if (!string.IsNullOrWhiteSpace(recordBy.OpenId))
                    {
                        var retrunUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                                         "/WorkFlow/Record/Detail/" + detail.WorkFlowRecord.Id;
                        var url1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                                   "/weixin/login/manager?returnUrl=" + Uri.EscapeDataString(retrunUrl1);

                        var dic1 = new Dictionary<string, object>
                    {
                        {"Title", "您的申请有新的进展！\r\n"},
                        {"Remark", "\r\n点击详情进行查看"},
                        {"Url",url1},
                        {"AppId", "wxcd1a304c25e0ea53"},
                        {"TemplateId", "zt0urD-W03g9q1_68_4H3pYHdEWVQD8w2XyLgVsxAxY"},
                        {"TemplateName", "申请结果通知"},
                        {"OpenIds", recordBy.OpenId},
                        {"KeyWord1", detail.WorkFlowRecord.AddedBy},
                        {"KeyWord2", detail.WorkFlowRecord.AddedDate},
                        {"KeyWord3", detail.WorkFlowRecord.Title},
                        {"KeyWord4", detail.ProcessResult}
                    };
                        _messageService.Send("Push", dic1);
                    }
                }
            }
            
            
            TempData["Msg"] = "操作成功";
            return RedirectToAction("Index");
        }
    }
}