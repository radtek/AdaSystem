using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading;
using System.Web;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Domain.Resource;
using Ada.Core.Infrastructure;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Core.ViewModel.Resource;
using Ada.Services.QuartzTask;
using Ada.Services.Resource;
using log4net;
using Newtonsoft.Json;
using Quartz;
using QuartzTask.Services;
using WebGrease.Css.Ast.Selectors;

namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class WeiXinInfoProJob : IJob
    {
        private readonly IMediaJobService _service = EngineContext.Current.Resolve<IMediaJobService>();
        public void Execute(IJobExecutionContext context)
        {
            _service.WeixinInfo(context);
        }
    }
}