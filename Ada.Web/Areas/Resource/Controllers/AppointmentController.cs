using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Filter;
using Ada.Framework.Messaging;
using Ada.Services.Admin;
using Ada.Services.Resource;
using Ada.Services.Setting;

namespace Resource.Controllers
{
    public class AppointmentController : BaseController
    {
        private readonly IRepository<Media> _repository;
        private readonly IMediaService _service;
        private readonly IMediaAppointmentService _mediaAppointmentService;
        private readonly IRepository<MediaAppointment> _mediaAppointmentRepository;
        private readonly IMessageService _messageService;
        private readonly IManagerService _managerService;
        private readonly ISettingService _settingService;
        private static readonly object Locker = new object();
        public AppointmentController(IRepository<Media> repository,
            IMediaService service,
            IMediaAppointmentService mediaAppointmentService,
            IRepository<MediaAppointment> mediaAppointmentRepository,
            IMessageService messageService,
            IManagerService managerService,
            ISettingService settingService)
        {
            _repository = repository;
            _service = service;
            _mediaAppointmentService = mediaAppointmentService;
            _mediaAppointmentRepository = mediaAppointmentRepository;
            _messageService = messageService;
            _managerService = managerService;
            _settingService = settingService;
        }
        public ActionResult Index(string id)
        {
            var medias = _repository.LoadEntities(d =>
                d.IsDelete == false && d.MediaType.CallIndex == "weixin" && d.Cooperation == Consts.StateNormal).OrderByDescending(d => d.MediaPrices.FirstOrDefault(p => p.AdPositionName == "头条（原创）").SellPrice).ThenBy(d => d.MediaName).ToList();
            ViewBag.Medias = medias;
            var media = string.IsNullOrWhiteSpace(id) ? medias.FirstOrDefault() : medias.FirstOrDefault(d => d.Id == id);
            return View(media);
        }
        [HttpPost, AdaValidateAntiForgeryToken]
        public ActionResult Make(MediaAppointmentView view)
        {
            lock (Locker)
            {
                var media = _repository.LoadEntities(d => d.Id == view.MediaId).FirstOrDefault();
                //判断是否被人预约
                if (media.MediaAppointments.Any(d => d.AppointmentDate.Value.Date == view.AppointmentDate.Value.Date && d.Taxis == view.Taxis))
                {
                    return Json(new { State = 0, Msg = "抱歉！" + media.MediaName + "[" + view.AppointmentDate.Value.Date.ToString("MM-dd") + "] 已被预约，请择日再预约。" });
                }
                media.MediaAppointments.Add(new MediaAppointment()
                {
                    Transactor = CurrentManager.UserName,
                    TransactorId = CurrentManager.Id,
                    AppointmentDate = view.AppointmentDate.Value.Date,
                    State = view.State,
                    AddedDate = DateTime.Now,
                    Id = IdBuilder.CreateIdNum(),
                    Taxis = view.Taxis
                });
                _service.Update(media);
                //推送通知给编辑部和业务部
                var config = _settingService.GetSetting<WeiGuang>();
                if (config.AppointmentPush)
                {

                    var openids = _managerService.GetByOrganizationName("编辑部")
                        .Where(d => !string.IsNullOrWhiteSpace(d.OpenId)).Select(d => d.OpenId).ToList();
                    //var openids = _managerService.GetByOrganizationName("技术部")
                    //    .Where(d => !string.IsNullOrWhiteSpace(d.OpenId)).Select(d => d.OpenId).ToList();
                    //openids.AddRange(jsbOpenIds);
                    if (openids.Any())
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("预约日期：" + view.AppointmentDate.Value.Date.ToString("MM-dd"));
                        sb.AppendLine("预约人员：" + CurrentManager.UserName);
                        var dic = new Dictionary<string, object>
                        {
                            {"Title", "自运营号有新的预约申请，请及时关注。\r\n"},
                            {"AppId", "wxcd1a304c25e0ea53"},
                            {"TemplateId", "dMmnd8bv_GcFIH5SnMByO80r-bcJQRPMWduHpYpd1nU"},
                            {"TemplateName", "预约成功通知"},
                            {"OpenIds", string.Join(",",openids)},
                            {"KeyWord1", media.MediaName},
                            {"KeyWord2", view.Taxis==1?"头条":"次条"},
                            {"Remark", sb.ToString()},
                            {"Url",""}

                        };
                        _messageService.Send("Push", dic);
                    }
                }
                return Json(new { State = 1, Msg = "预约成功" });
            }

        }
        [HttpPost, AdaValidateAntiForgeryToken]
        public ActionResult Cancle(string id)
        {
            var make = _mediaAppointmentRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            //判断是否是自己的
            if (make == null)
            {
                return Json(new { State = 0, Msg = "不存在的预约" });
            }
            var premission = PremissionData();
            if (premission.Any())
            {
                if (make.TransactorId != CurrentManager.Id)
                {
                    return Json(new { State = 0, Msg = "无法取消他人的预约，请联系预约人进行取消" });
                }
            }
            _mediaAppointmentService.Delete(make);
            //推送通知给编辑部和业务部
            try
            {
                var config = _settingService.GetSetting<WeiGuang>();
                if (config.CancleAppointmentPush)
                {
                    var openids = _managerService.GetByOrganizationName("业务部")
                        .Where(d => !string.IsNullOrWhiteSpace(d.OpenId)).Select(d => d.OpenId).ToList();
                    var bjbOpenIds = _managerService.GetByOrganizationName("编辑部")
                        .Where(d => !string.IsNullOrWhiteSpace(d.OpenId)).Select(d => d.OpenId).ToList();
                    var jsbOpenIds = _managerService.GetByOrganizationName("技术部")
                        .Where(d => !string.IsNullOrWhiteSpace(d.OpenId)).Select(d => d.OpenId).ToList();
                    openids.AddRange(bjbOpenIds);
                    openids.AddRange(jsbOpenIds);
                    if (openids.Any())
                    {
                        var dic = new Dictionary<string, object>
                        {
                            {"Title", make.Transactor+" 的预约已取消。\r\n"},
                            {"Remark", ""},
                            {"AppId", "wxcd1a304c25e0ea53"},
                            {"TemplateId", "r_JKy6y3X8CtisTk2HT_NinKw2r0IE3KmNmFPIYpows"},
                            {"TemplateName", "预约取消通知"},
                            {"OpenIds", string.Join(",",openids)},
                            {"KeyWord1", make.AppointmentDate.Value.ToString("yyyy-MM-dd")},
                            {"KeyWord2", make.Media.MediaName+"-"+(make.Taxis==1?"头条":"次条")},
                            {"KeyWord3", "自行取消"}

                        };
                        _messageService.Send("Push", dic);
                    }
                }
               
            }
            catch 
            {

                //throw;
            }
            return Json(new { State = 1, Msg = "取消成功" });
        }

        public ActionResult List()
        {
            return View();
        }
        public ActionResult GetList(MediaAppointmentView viewModel)
        {
            var result = _mediaAppointmentService.LoadEntitiesFilter(viewModel).AsNoTracking().ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new MediaAppointmentView
                {
                    Id = d.Id,
                    Transactor = d.Transactor,
                    AppointmentDate = d.AppointmentDate,
                    Taxis = d.Taxis,
                    MediaName = d.Media.MediaName,
                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}