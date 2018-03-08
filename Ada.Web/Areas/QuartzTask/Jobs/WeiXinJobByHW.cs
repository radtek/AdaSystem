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
    public class WeiXinJobByHW : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            string[] team = { "X1712181337040001", "X1712181349070011" };
            WeiXinCollection.GetInfoAndArticle(context, team);
        }
    }
}