using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.ViewModel.QuartzTask;
using Ada.Framework.Filter;
using Ada.Services.QuartzTask;
using Newtonsoft.Json.Linq;
using QuartzTask.Models;

namespace QuartzTask.Controllers
{
    public class WeiXinController : BaseController
    {
        private readonly IJobService _jobService;
        private readonly IQuartzService _quartzService;
        private readonly IRepository<Job> _repository;
        public WeiXinController(IJobService jobService, IRepository<Job> repository, IQuartzService quartzService)
        {
            _jobService = jobService;
            _repository = repository;
            _quartzService = quartzService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Add()
        {
            JobView viewModel = new JobView();
            viewModel.Repetitions = 6;
            viewModel.ApiUrl = "http://api01.idataapi.cn:8000/post/weixinpro2?apikey=aHkIQg6KZL5nKgqhcAbrT7AYq484DkAfmFzd8rBgYDrK6CItsvAAOWwz7BiFkoQx&link=";
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Add(JobView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            Job entity = new Job();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedById = CurrentManager.Id;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedDate = DateTime.Now;
            entity.JobName = viewModel.JobName;
            entity.GroupName = "WGLH-WXJC";
            entity.JobType = "WeixinArticleMonitorJob";
            entity.TriggerName = "WXARTICLE-" + DateTime.Now.Ticks;
            entity.TriggerState = Quartz.TriggerState.None.ToString();
            entity.Cron = viewModel.Cron;
            var arr = viewModel.StartAndEnd.Split('至');
            entity.StartTime = DateTime.Parse(arr[0]);
            entity.EndTime = DateTime.Parse(arr[1]);
            entity.IsLog = true;
            entity.ApiUrl = viewModel.ApiUrl;
            entity.Params = viewModel.Params;
            entity.IsLog = viewModel.IsLog;
            entity.Repetitions = viewModel.Repetitions;
            entity.Type = 1;
            entity.TimeOut = viewModel.TimeOut;
            entity.Repetitions = viewModel.Repetitions;

            _jobService.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            JobView entity = new JobView();
            entity.Id = item.Id;
            entity.JobName = item.JobName;
            entity.Cron = item.Cron;
            entity.StartTime = item.StartTime;
            entity.EndTime = item.EndTime;
            entity.StartAndEnd = item.StartTime.Value.ToString("yyyy-MM-dd HH:mm") + "至" +
                                 item.EndTime.Value.ToString("yyyy-MM-dd HH:mm");
            entity.ApiUrl = item.ApiUrl;
            entity.Params = item.Params;
            entity.Repetitions = item.Repetitions;
            return View(entity);
        }
        [HttpPost]
        public ActionResult Update(JobView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.JobName = viewModel.JobName;
            entity.Cron = viewModel.Cron;
            var arr = viewModel.StartAndEnd.Split('至');
            entity.StartTime = DateTime.Parse(arr[0]);
            entity.EndTime = DateTime.Parse(arr[1]);
            entity.ApiUrl = viewModel.ApiUrl;
            entity.Params = viewModel.Params;
            entity.Repetitions = viewModel.Repetitions;
            _jobService.Update(entity);
            TempData["Msg"] = "操作成功";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Start(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.EndTime!=null)
            {
                if (DateTime.Now>=entity.EndTime)
                {
                    return Json(new { State = 0, Msg = "该任务结束时间已过，无法启动任务，请修改任务结束时间，再进行启动" });
                }
            }
            entity.TriggerState = Quartz.TriggerState.Normal.ToString();
            _jobService.Update(entity);
            _quartzService.Start(entity);
            return Json(new { State = 1, Msg = "任务开始成功" });
        }
        [HttpPost]
        public ActionResult Stop(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.TriggerState = Quartz.TriggerState.None.ToString();
            _jobService.Update(entity);
            _quartzService.Stop(entity);
            return Json(new { State = 1, Msg = "任务停止成功" });
        }
        public ActionResult Chart(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("Chart", entity);
        }
        public ActionResult Export(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            JArray jObjects = new JArray();
            var details = entity.JobDetails.OrderBy(d => d.AddedDate).ToList();
            for (int i = 0; i < details.Count; i++)
            {
                var jo = new JObject();
                var item = details[i];
                var last = i == 0 ? item : details[i - 1];
                jo.Add("监测时间", item.AddedDate.Value.ToString("yyyy-MM-dd HH:mm"));
                jo.Add("阅读数", item.Num1);
                var rAdd = item.Num1 - last.Num1;
                jo.Add("阅读增量", rAdd);
                jo.Add("点赞数", item.Num2);
                var lAdd = item.Num2 - last.Num2;
                jo.Add("点赞增量", lAdd);
                jo.Add("备注", item.IsSuccess==true?"":"数据监测失败");
                jObjects.Add(jo);
            }
            return Json(new { State = 1, Msg = ExportFile(jObjects.ToString()) },JsonRequestBehavior.AllowGet);
        }
    }
}