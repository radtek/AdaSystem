using System;
using System.Collections.Generic;
using System.Linq;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.Domain.Resource;
using Ada.Data;
using log4net;
using Quartz;

namespace QuartzTask.Jobs
{
    [DisallowConcurrentExecution]
    public class UpdateMediaSellPriceJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (var db = new AdaEFDbcontext())
                {
                    var name = context.JobDetail.Key.Name;
                    var group = context.JobDetail.Key.Group;
                    var job = db.Set<Job>().FirstOrDefault(d => d.GroupName == group && d.JobName == name);
                    var media = db.Set<MediaPrice>().FirstOrDefault(d =>
                          d.IsDelete == false && d.Media.IsDelete == false && d.SellPrice == null);
                    var priceRange = db.Set<Field>().Where(d => d.FieldType.CallIndex == "SellPriceRange" && d.IsDelete == false)
                        .OrderBy(d => d.Taxis).ToList();
                    if (media != null)
                    {
                        media.SellPrice = SetSalePrice(Convert.ToDecimal(media.PurchasePrice), priceRange);
                        //改变工作计划时间
                        if (context.NextFireTimeUtc != null)
                        {
                            if (job != null)
                            {
                                job.NextTime = context.NextFireTimeUtc.Value.ToLocalTime().DateTime;
                                job.Remark = "销售价格更新任务正在运行中，本次成功更新：" + media.Media.MediaType.TypeName + "-" + media.Media.MediaName;
                            }
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        if (job != null) job.Remark = "销售价格更新任务暂无可更新的资源数据！更新时间：" + DateTime.Now;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("销售价格更新任务异常", ex);
            }
        }

        private decimal SetSalePrice(decimal price, IEnumerable<Field> priceRanges)
        {
            if (price <= 0) return 0;
            foreach (var range in priceRanges)
            {
                var qj = range.Text.Split('-');
                if (price >= decimal.Parse(qj[0]) && price <= decimal.Parse(qj[1]))
                {
                    var value = decimal.Parse(range.Value);
                    return value <= 5 ? PriceZero(value * price) : PriceZero(value + price);
                }
            }
            return 0;
        }
        private decimal PriceZero(decimal a)
        {
            if (a >= 100000)
            {
                return (int)a / 1000 * 1000;
            }
            if (a >= 10000)
            {
                return (int)a / 1000 * 1000;
            }
            if (a >= 1000)
            {
                return (int)a / 100 * 100;
            }
            if (a >= 100)
            {
                return (int)a / 100 * 100;
            }
            return a;
        }
    }
}