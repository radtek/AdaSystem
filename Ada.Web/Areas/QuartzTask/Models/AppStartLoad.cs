using Ada.Core;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Infrastructure;

namespace QuartzTask.Models
{
    /// <summary>
    /// 防止网站进入休眠状态或者网站程序异常停止出现任务计划停止，这个类将恢复计划任务的作用
    /// 全局启动检测是否有在运行的计划任务
    /// </summary>
    public class AppStartLoad: IAppStart
    {
        private readonly IRepository<Job> _repository;
        private readonly IQuartzService _quartzService;
        public AppStartLoad()
        {
            _quartzService = EngineContext.Current.Resolve<IQuartzService>();
            _repository = EngineContext.Current.Resolve<IRepository<Job>>();
        }
        public void Register()
        {
            var jobsNormal = _repository.LoadEntities(d => d.TriggerState == Quartz.TriggerState.Normal.ToString());
            foreach (var job in jobsNormal)
            {
                _quartzService.Start(job);
            }
        }
    }
}