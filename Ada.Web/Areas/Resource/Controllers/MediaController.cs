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
        private readonly IMediaService _mediaService;
        public MediaController(IMediaPriceService mediaPriceService, IMediaService mediaService)
        {
            _mediaPriceService = mediaPriceService;
            _mediaService = mediaService;
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
                    d.Media.MediaType.TypeName,
                    d.Media.MediaType.CallIndex,
                    d.Media.MediaID,
                    d.AdPositionName,
                    d.Media.Platform,
                    d.Media.Client,
                    d.Media.Channel,
                    d.PurchasePrice,
                    d.Media.Transactor,
                    MediaTagStr = string.Join(",", d.Media.MediaTags.Select(t => t.TagName)),
                    
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMedias(MediaView viewModel)
        {
            var result = _mediaService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new
                {
                    d.Id,
                    d.MediaName,
                    d.MediaType.TypeName,
                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}