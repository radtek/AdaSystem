using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Ada.Data;
using log4net;
using Quartz;

namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class WeiXinJobByHXHG : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            string[] team = { "X1712181402100028", "X1712181349560012", "X1712181221160001", "X1712181337340002" };
            WeiXinCollection.GetInfoAndArticle(context, team);
        }
    }
}