using System;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.Linq;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.API;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Domain.Resource;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Data;
using log4net;
using Newtonsoft.Json;
using Quartz;
using QuartzTask.Models;
using QuartzTask.Services;


namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class RedBookInfoJob : IJob
    {
        private readonly IMediaJobService _service = EngineContext.Current.Resolve<IMediaJobService>();
        public void Execute(IJobExecutionContext context)
        {
            _service.RedBookInfo(context);
        }
        
    }
}