using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Ada.Core;
using Ada.Core.Domain.Resource;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Services.API;
using log4net;
using Quartz;

namespace QuartzTask.Jobs
{
    public class WeiXinInfoJob : IJob
    {
        private readonly IiDataAPIService _iDataAPIService;
        private readonly IRepository<Media> _repository;
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public WeiXinInfoJob()
        {
            _iDataAPIService = EngineContext.Current.Resolve<IiDataAPIService>();
            _repository = EngineContext.Current.Resolve<IRepository<Media>>();
        }
        public void Execute(IJobExecutionContext context)
        {
            Task.Factory.StartNew(() =>
            {
                _logger.Info("微信信息自动任务开始：" + DateTime.Now);
                try
                {
                    var medias = _repository.LoadEntities(d => d.IsDelete == false && d.MediaType.CallIndex == "weixin" && d.IsSlide == true).OrderBy(d => d.Id).ToList();
                    foreach (var media in medias)
                    {
                        if (string.IsNullOrWhiteSpace(media.MediaID)) continue;
                        BaseParams wxparams = new BaseParams
                        {
                            CallIndex = "weixin",
                            IsLog = false,
                            UID = media.MediaID.Trim(),
                            Transactor = "系统自动",
                            TransactorId = "系统自动"
                        };
                        try
                        {
                            _iDataAPIService.GetWeinXinInfo(wxparams);
                        }
                        catch (Exception e)
                        {
                            _logger.Error("微信信息" + media.MediaID + "，自动任务失败：" + DateTime.Now, e);
                        }

                    }
                    _logger.Info("微信信息自动任务结束：" + DateTime.Now );
                }
                catch (Exception ex)
                {
                    _logger.Info("微信信息自动任务异常结束！", ex);
                }


            });
        }
    }
}