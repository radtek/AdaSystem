using Ada.Core.Infrastructure;
using Quartz;
using QuartzTask.Services;


namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class RedBookArticlesJob : IJob
    {
        private readonly IMediaJobService _service = EngineContext.Current.Resolve<IMediaJobService>();
        public void Execute(IJobExecutionContext context)
        {
            _service.RedBookArticle(context);
        }
    }
}