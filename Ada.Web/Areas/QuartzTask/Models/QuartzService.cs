using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Ada.Core.Domain.QuartzTask;
using Quartz;
using Quartz.Impl;

namespace QuartzTask.Models
{
    public class QuartzService : IQuartzService
    {
        private readonly IScheduler _scheduler;
        public QuartzService()
        {
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.Start();
        }
        public void Pause(Job entity)
        {
            var jobKey = new JobKey(entity.JobName, entity.GroupName);
            _scheduler.PauseJob(jobKey);
        }

        public void Resume(Job entity)
        {
            var jobKey = new JobKey(entity.JobName, entity.GroupName);
            _scheduler.ResumeJob(jobKey);
        }

        public void Start(Job entity)
        {
            var jobKey=new JobKey(entity.JobName,entity.GroupName);
            if (_scheduler.CheckExists(jobKey))
            {
                return;
            }
            IJobDetail jobDetail = JobBuilder.Create(JobHelper.GetJobType(entity.JobType)).WithIdentity(jobKey).Build();
            JobDataMap jobDataMap = jobDetail.JobDataMap;
            jobDataMap.Put("任务描述", entity.Remark);
            var triggerKey=new TriggerKey(entity.TriggerName,entity.GroupName);
            DateTimeOffset startRunTime = DateBuilder.NextGivenMinuteDate(entity.StartTime, 1);
            DateTimeOffset endRunTime = DateBuilder.NextGivenSecondDate(entity.EndTime, 1);
            ITrigger trigger = TriggerBuilder.Create().StartAt(startRunTime).EndAt(endRunTime).WithIdentity(triggerKey).WithCronSchedule(entity.Cron).Build();
            _scheduler.ScheduleJob(jobDetail, trigger);
            
        }

        public void Stop(Job entity)
        {
            var jobKey = new JobKey(entity.JobName, entity.GroupName);
            _scheduler.DeleteJob(jobKey);
        }
    }
}