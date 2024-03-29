﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Quartz;

namespace QuartzTask.Services
{
   public interface IMediaJobService: IDependency
   {
       void WeixinInfo(IJobExecutionContext context);
       void WeixinArticle(IJobExecutionContext context);
       void WeixinArticleBySouhu(IJobExecutionContext context);
       void WeixinArticlePro4(IJobExecutionContext context);
       void WeixinArticleData(IJobExecutionContext context);

       void WeiboArticle(IJobExecutionContext context);
       void RedBookInfo(IJobExecutionContext context);
       void RedBookArticle(IJobExecutionContext context);
   }
}
