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
    public class LinkManController : BaseController
    {
        private readonly ILinkManService _linkManService;
        private readonly IRepository<Commpany> _repository;
        private readonly IRepository<LinkMan> _linkManRepository;

        public LinkManController(ILinkManService linkManService, IRepository<Commpany> repository, IRepository<LinkMan> linkManRepository)
        {
            _linkManService = linkManService;
            _repository = repository;
            _linkManRepository = linkManRepository;
        }
        public ActionResult GetList(LinkManView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _linkManService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new LinkManView
                {
                    Id = d.Id,
                    Name = d.Name,
                    CommpanyName = d.Commpany.Name,
                    IsCooperation = d.BusinessOrders.Any(l=>l.BusinessOrderDetails.Any(m=>m.MediaPrice.Media.Cooperation==1&&m.MediaPrice.Media.MediaType.CallIndex=="weixin"))
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCompanys(CommpanyView viewModel)
        {
            var offset = viewModel.offset ?? 0;
            var rows = viewModel.limit ?? 10;
            var allList = _repository.LoadEntities(d => d.IsDelete == false && d.IsBusiness == viewModel.IsBusiness);
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Name.Contains(viewModel.search));
            }
            var result = allList.OrderBy(d => d.Id).Skip(offset).Take(rows);
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new CommpanyView
                {
                    Id = d.Id,
                    Name = d.Name,
                    IsCooperation = d.LinkMans.Any(o => o.BusinessOrders.Any(l => l.BusinessOrderDetails.Any(m => m.MediaPrice.Media.Cooperation == 1 && m.MediaPrice.Media.MediaType.CallIndex == "weixin")))
                })
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(string id)
        {
            var linkman = _linkManRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("LinkManDetail",linkman);
        }
    }
}