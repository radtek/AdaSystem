using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.ViewModel.Customer;
using Ada.Framework.Filter;
using Ada.Services.Customer;

namespace Customer.Controllers
{
    public class LinkManController : BaseController
    {
        private readonly ILinkManService _linkManService;

        public LinkManController(ILinkManService linkManService)
        {
            _linkManService = linkManService;
        }
        public ActionResult GetList(LinkManView viewModel)
        {
            var result = _linkManService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new LinkManView
                {
                    Id = d.Id,
                    Name = d.Name,       
                    CommpanyName = d.Commpany.Name
                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}