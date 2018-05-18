using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Resource;
using Ada.Framework.Filter;

namespace Resource.Controllers
{
    public class AppointmentController : BaseController
    {
        private readonly IRepository<Media> _repository;

        public AppointmentController(IRepository<Media> repository)
        {
            _repository = repository;
        }
        public ActionResult Index()
        {
            var medias = _repository.LoadEntities(d =>
                    d.IsDelete == false && d.MediaType.CallIndex == "weixin" && d.Cooperation == Consts.StateNormal)
                .ToList();
            return View(medias);
        }
    }
}