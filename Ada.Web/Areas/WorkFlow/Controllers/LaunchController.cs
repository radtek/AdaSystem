using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.WorkFlow;
using Ada.Core.Tools;
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
    /// 发起流程
    /// </summary>
    public class LaunchController : BaseController
    {
        private readonly IRepository<WorkFlowDefinition> _repository;
        private readonly IWorkFlowDefinitionService _service;
        private readonly IMessageService _messageService;
        private readonly IWorkFlowProvider _workFlowProvider;
        private readonly IRepository<Manager> _managerRepository;
        private readonly ISettingService _settingService;
        public LaunchController(IRepository<WorkFlowDefinition> repository,
            IRepository<Manager> managerRepository,
            IWorkFlowDefinitionService service,
            IWorkFlowProvider workFlowProvider,
            IMessageService messageService,
            ISettingService settingService)
        {
            _repository = repository;
            _managerRepository = managerRepository;
            _service = service;
            _workFlowProvider = workFlowProvider;
            _messageService = messageService;
            _settingService = settingService;
        }
        /// <summary>
        /// 选择工作流
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var wfs = _repository.LoadEntities(d => d.IsDelete == false).ToList();
            return View(wfs);
        }
        /// <summary>
        /// 申请表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Add(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            WorkFlowRecordView viewModel = new WorkFlowRecordView();
            viewModel.WorkFlowDefinitionId = id;
            viewModel.WorkFlowDefinitionName = entity.Name;
            viewModel.WorkFlowDefinitionDescription = entity.Description;
            viewModel.Content = entity.TempForm;
            ViewBag.FlowTos = _managerRepository.LoadEntities(d => d.IsDelete == false && d.Status == Consts.StateNormal && d.Roles.Any(r => r.DataRange > 0)).ToList()
                .Select(d => new SelectListItem() { Text = string.Join(",", d.Organizations.Where(g => g.ParentId != null).Select(o => o.OrganizationName)) + " — " + d.UserName, Value = d.Id }).OrderByDescending(d => d.Text).ToList();
            return View(viewModel);
        }
        /// <summary>
        /// 发起申请，启动工作流
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(WorkFlowRecordView viewModel)
        {

            var workFlow = _repository.LoadEntities(d => d.Enabled && d.Id == viewModel.WorkFlowDefinitionId).FirstOrDefault();
            if (workFlow == null)
            {
                TempData["Warning"] = "工作流程未开启或者未定义";
                return View(viewModel);
            }
            //启动工作流
            var wfApp = _workFlowProvider.CreateWorkflowApp(workFlow.ActityType, null);
            //在工作流记录表添加一条数据：
            WorkFlowRecord workFlowRecord = new WorkFlowRecord();
            workFlowRecord.Id = IdBuilder.CreateIdNum();
            workFlowRecord.Title = viewModel.Title;
            workFlowRecord.Content = viewModel.Content;
            workFlowRecord.Level = viewModel.Level;
            workFlowRecord.WfInstanceId = wfApp.Id.ToString();
            workFlowRecord.Status = (short)WorkFlowEnum.UnProecess;
            workFlowRecord.AddedDate = DateTime.Now;
            workFlowRecord.AddedBy = CurrentManager.UserName;
            workFlowRecord.AddedById = CurrentManager.Id;
            //在工作流记录明细表里面添加两条步骤。一个当前已经处理的完成步骤。
            WorkFlowRecordDetail initDetail = new WorkFlowRecordDetail();
            initDetail.Id = IdBuilder.CreateIdNum();
            initDetail.Name = "提交申请信息";
            initDetail.IsEnd = false;
            initDetail.IsStart = true;
            initDetail.ProcessBy = CurrentManager.UserName;
            initDetail.ProcessById = CurrentManager.Id;
            initDetail.ProcessComment = "提交申请";
            initDetail.ProcessResult = "通过";
            initDetail.ProcessDate = DateTime.Now;
            initDetail.Status = (short)WorkFlowEnum.Processed;
            workFlowRecord.WorkFlowRecordDetails.Add(initDetail);
            //二个步骤：下一步谁审批的步骤
            var nextProcess = _managerRepository.LoadEntities(d => d.Id == viewModel.FlowTo).FirstOrDefault();
            WorkFlowRecordDetail nextDetail = new WorkFlowRecordDetail();
            nextDetail.Id = IdBuilder.CreateIdNum();
            nextDetail.AddedDate = DateTime.Now;
            nextDetail.IsEnd = false;
            nextDetail.IsStart = false;
            nextDetail.Status = (short)WorkFlowEnum.UnProecess;
            nextDetail.ProcessBy = nextProcess.UserName;
            nextDetail.ProcessById = nextProcess.Id;
            workFlowRecord.WorkFlowRecordDetails.Add(nextDetail);
            workFlow.WorkFlowRecords.Add(workFlowRecord);
            _service.Update(workFlow);
            wfApp.Run();
            //微信提醒
            var setting = _settingService.GetSetting<WeiGuang>();
            if (setting.WorkFlowPush)
            {
                var retrunUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                                "/WorkFlow/Approve/Examination/" + workFlowRecord.Id;
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
                    {"KeyWord1", viewModel.Title},
                    {"KeyWord2", CurrentManager.UserName},
                    {"KeyWord3", workFlow.Name},
                    {"KeyWord4", DateTime.Now.ToString("yyyy-MM-dd HH:mm")}

                };
                _messageService.Send("Push", dic);
            }

            //返回我的工作记录表
            return RedirectToAction("Index", "Record");
        }
    }
}