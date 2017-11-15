using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Resource;

namespace Resource.Controllers
{
    public class MediaController : BaseController
    {
        private readonly IMediaPriceService _mediaPriceService;

        public MediaController(IMediaPriceService mediaPriceService)
        {
            _mediaPriceService = mediaPriceService;
        }
        public ActionResult GetMediaPrices(MediaView viewModel)
        {
            var result = _mediaPriceService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new
                {
                    d.Id,
                    d.Media.MediaName,
                    d.Media.MediaID,
                    d.AdPositionName,
                    d.PurchasePrice,
                    d.Media.Transactor,
                    MediaTagStr = string.Join(",", d.Media.MediaTags.Select(t => t.TagName)),
                    
                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}