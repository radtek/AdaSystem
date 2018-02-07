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
using Quartz;
using QuartzTask.Models;

namespace QuartzTask.Controllers
{
    public class JobController : BaseController
    {
        private readonly IJobService _jobService;
        private readonly IQuartzService _quartzService;
        private readonly IRepository<Job> _repository;
        public JobController(IJobService jobService, IRepository<Job> repository, IQuartzService quartzService)
        {
            _jobService = jobService;
            _repository = repository;
            _quartzService = quartzService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(JobView viewModel)
        {
            var result = _jobService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new JobView
                {
                    Id = d.Id,
                    JobName = d.JobName,
                    GroupName = d.GroupName,
                    JobType = d.JobType,
                    TriggerName = d.TriggerName,
                    Cron = d.Cron,
                    TriggerState = d.TriggerState,
                    StartTime = d.StartTime,
                    EndTime = d.EndTime,
                    NextTime = d.NextTime,
                    PreTime = d.PreTime,
                    Remark = d.Remark
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            entity.GroupName = viewModel.GroupName;
            entity.JobType = viewModel.JobType;
            entity.TriggerName = viewModel.TriggerName;
            entity.TriggerState = Quartz.TriggerState.None.ToString();
            entity.Cron = viewModel.Cron;
            entity.StartTime = viewModel.StartTime;
            entity.EndTime = viewModel.EndTime;
            entity.Remark = viewModel.Remark;
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
            entity.GroupName = item.GroupName;
            entity.JobType = item.JobType;
            entity.TriggerName = item.TriggerName;
            entity.Cron = item.Cron;
            //entity.TriggerState = item.TriggerState;
            entity.StartTime = item.StartTime;
            entity.EndTime = item.EndTime;
            entity.Remark = item.Remark;
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            entity.GroupName = viewModel.GroupName;
            entity.JobType = viewModel.JobType;
            entity.TriggerName = viewModel.TriggerName;
            entity.Cron = viewModel.Cron;
            //entity.TriggerState = viewModel.TriggerState;
            entity.StartTime = viewModel.StartTime;
            entity.EndTime = viewModel.EndTime;
            entity.Remark = viewModel.Remark;
            _jobService.Update(entity);
            TempData["Msg"] = "操作成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.TriggerState!="None")
            {
                return Json(new { State = 0, Msg = "此任务未处于关闭状态，请先关闭任务，再进行删除！" });
            }
            _jobService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
        /// <summary>
        /// 开启
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Start(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.TriggerState = Quartz.TriggerState.Normal.ToString();
            //if (entity.StartTime == null)
            //{
            //    entity.StartTime = DateTime.Now;
            //}
            //if (entity.EndTime == null)
            //{
            //    entity.EndTime = DateTime.MaxValue.AddDays(-1);
            //}
            _jobService.Update(entity);
            _quartzService.Start(entity);
            TempData["Msg"] = "任务启动成功";
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Pause(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.TriggerState = Quartz.TriggerState.Paused.ToString();
            _jobService.Update(entity);
            _quartzService.Pause(entity);
            TempData["Msg"] = "任务暂停运行";
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 继续
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Resume(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.TriggerState = Quartz.TriggerState.Normal.ToString();
            _jobService.Update(entity);
            _quartzService.Resume(entity);
            TempData["Msg"] = "任务重新开始运行";
            return RedirectToAction("Index");
        }
        public ActionResult Stop(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.TriggerState = Quartz.TriggerState.None.ToString();
            _jobService.Update(entity);
            _quartzService.Stop(entity);
            TempData["Msg"] = "任务停止";
            return RedirectToAction("Index");
        }
    }
}