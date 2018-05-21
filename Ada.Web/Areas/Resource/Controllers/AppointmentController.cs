using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Resource;

namespace Resource.Controllers
{
    public class AppointmentController : BaseController
    {
        private readonly IRepository<Media> _repository;
        private readonly IMediaService _service;
        private readonly IMediaAppointmentService _mediaAppointmentService;
        private readonly IRepository<MediaAppointment> _mediaAppointmentRepository;
        private static readonly object Locker = new object();
        public AppointmentController(IRepository<Media> repository, IMediaService service, IMediaAppointmentService mediaAppointmentService, IRepository<MediaAppointment> mediaAppointmentRepository)
        {
            _repository = repository;
            _service = service;
            _mediaAppointmentService = mediaAppointmentService;
            _mediaAppointmentRepository = mediaAppointmentRepository;
        }
        public ActionResult Index(string id)
        {
            var medias = _repository.LoadEntities(d =>
                d.IsDelete == false && d.MediaType.CallIndex == "weixin" && d.Cooperation == Consts.StateNormal).OrderByDescending(d => d.MediaPrices.FirstOrDefault(p => p.AdPositionName == "头条（原创）").SellPrice).ThenBy(d=>d.MediaName).ToList();
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
                if (media.MediaAppointments.Any(d => d.AppointmentDate.Value.Date == view.AppointmentDate.Value.Date))
                {
                    return Json(new { State = 0, Msg = "抱歉！" + media.MediaName + "[" + view.AppointmentDate.Value.Date.ToString("MM-dd") + "] 已被预约，请择日再预约。" });
                }

                var id = IdBuilder.CreateIdNum();
                media.MediaAppointments.Add(new MediaAppointment()
                {
                    Transactor = CurrentManager.UserName,
                    TransactorId = CurrentManager.Id,
                    AppointmentDate = view.AppointmentDate.Value.Date,
                    State = view.State,
                    AddedDate = DateTime.Now,
                    Id = id
                });
                _service.Update(media);
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
            return Json(new { State = 1, Msg = "取消成功" });
        }
    }
}