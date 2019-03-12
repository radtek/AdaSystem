using System;
using System.Linq;
using Ada.Core;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Infrastructure;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Services.QuartzTask;
using Newtonsoft.Json;
using Quartz;

namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class WeixinArticleMonitorJob : IJob
    {
        private readonly IJobService _service = EngineContext.Current.Resolve<IJobService>();
        public void Execute(IJobExecutionContext context)
        {
            var name = context.JobDetail.Key.Name;
            var group = context.JobDetail.Key.Group;
            var job = _service.GetByJobKey(name,group);
            byte[] b = System.Text.Encoding.Default.GetBytes(job.Params);
            //转成 Base64 
            var url = job.ApiUrl + Convert.ToBase64String(b);
            var times = job.Repetitions ?? 3;
            var detail = new JobDetail {Id = IdBuilder.CreateIdNum(), RequestDate = DateTime.Now,IsSuccess = false};
            for (var i = 0; i < times; i++)
            {
                try
                {
                    var rhtml = Ada.Core.Tools.HttpUtility.Get(url);
                    detail.ReponseDate = DateTime.Now;
                    var result = JsonConvert.DeserializeObject<WeiXinPro2JSON>(rhtml);
                    detail.ReponseContent = result.message;
                    detail.Retcode = result.CodeValue.ToString();
                    if (result.retcode == ReturnCode.请求成功)
                    {
                        if (result.data.Any())
                        {
                            var data = result.data.FirstOrDefault();
                            if (data != null)
                            {
                                detail.Num1 = data.viewCount ?? 0;
                                detail.Num2 = data.likeCount ?? 0;
                                detail.IsSuccess = true;
                            }
                        }
                        break;
                    }

                    if (result.retcode == ReturnCode.目标参数搜索没结果)
                    {
                        detail.Retmsg = result.message;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    detail.Retmsg = ex.Message;
                    detail.Retcode = "502";
                }
            }
            detail.AddedDate=DateTime.Now;
            job.JobDetails.Add(detail);
            if (context.NextFireTimeUtc != null)
            {
                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
                job.Remark = "微信文章监测：" + DateTime.Now;
                if (job.NextTime>=job.EndTime)
                {
                    job.TriggerState = TriggerState.Complete.ToString();
                    job.Remark = "微信文章监测完成" ;
                    job.NextTime = null;
                    context.Scheduler.DeleteJob(context.JobDetail.Key);
                }
            }

            
            _service.Update(job);
        }
    }
}