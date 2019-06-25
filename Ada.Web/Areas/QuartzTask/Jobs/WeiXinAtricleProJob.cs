using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Domain.Resource;
using Ada.Core.Infrastructure;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Services.Cache;
using Ada.Services.QuartzTask;
using Newtonsoft.Json;
using Quartz;
using QuartzTask.Services;

namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class WeiXinAtricleProJob1 : IJob
    {
        private readonly IMediaJobService _service = EngineContext.Current.Resolve<IMediaJobService>();
        public void Execute(IJobExecutionContext context)
        {
            _service.WeixinArticleBySouhu(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinAtricleProJob2 : IJob
    {
        private readonly IMediaJobService _service = EngineContext.Current.Resolve<IMediaJobService>();
        public void Execute(IJobExecutionContext context)
        {
            _service.WeixinArticleBySouhu(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinAtricleProJob3 : IJob
    {
        private readonly IMediaJobService _service = EngineContext.Current.Resolve<IMediaJobService>();
        public void Execute(IJobExecutionContext context)
        {
            _service.WeixinArticleBySouhu(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinAtricleProJob4 : IJob
    {
        private readonly IMediaJobService _service = EngineContext.Current.Resolve<IMediaJobService>();
        public void Execute(IJobExecutionContext context)
        {
            _service.WeixinArticleBySouhu(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinAtricleProJob5 : IJob
    {
        private readonly IMediaJobService _service = EngineContext.Current.Resolve<IMediaJobService>();
        public void Execute(IJobExecutionContext context)
        {
            _service.WeixinArticleBySouhu(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinAtricleProJob6 : IJob
    {
        private readonly IMediaJobService _service = EngineContext.Current.Resolve<IMediaJobService>();
        public void Execute(IJobExecutionContext context)
        {
            _service.WeixinArticleBySouhu(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinAtricleProJob7 : IJob
    {
        private readonly IMediaJobService _service = EngineContext.Current.Resolve<IMediaJobService>();
        public void Execute(IJobExecutionContext context)
        {
            _service.WeixinArticleBySouhu(context);
        }
    }
    [DisallowConcurrentExecution]
    public class WeiXinAtricleProJob8 : IJob
    {
        private readonly IMediaJobService _service = EngineContext.Current.Resolve<IMediaJobService>();
        public void Execute(IJobExecutionContext context)
        {
            _service.WeixinArticleBySouhu(context);
        }
    }
}