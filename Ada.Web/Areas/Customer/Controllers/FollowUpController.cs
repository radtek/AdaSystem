using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;
using Ada.Framework.Filter;
using Ada.Services.Customer;

namespace Customer.Controllers
{
    public class FollowUpController : BaseController
    {
        private readonly IFollowUpService _followUpService;
        private readonly IRepository<FollowUp> _repository;
        public FollowUpController(IRepository<FollowUp> repository, IFollowUpService followUpService)
        {
            _followUpService = followUpService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            ViewBag.IsAdmin = !PremissionData().Any();
            return View();
        }
        public ActionResult GetList(FollowUpView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _followUpService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new FollowUpView
                {
                    Id = d.Id,
                    Content = d.Content,
                    FollowUpWay = d.FollowUpWay,
                    LinkManName = d.LinkMan.Name,
                    NextTime = d.NextTime,
                    Transactor = d.LinkMan.LoginName,
                    CompanyName = d.LinkMan.Commpany.Name,
                    IpAddress = d.IpAddress
                })
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _followUpService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }

    }
}