using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;

namespace Ada.Web.Controllers
{
    public class MediaController : Controller
    {
        private readonly IRepository<Media> _repository;
        public MediaController(IRepository<Media> repository)
        {
            _repository = repository;
        }
        public ActionResult WeiXin(MediaView viewModel)
        {
            viewModel.limit = 10;
            var medias = _repository.LoadEntities(d =>
                d.MediaType.CallIndex == "weixin" && d.IsDelete == false && d.Status == Consts.StateNormal &&
                d.IsSlide == true);
            if (!string.IsNullOrWhiteSpace(viewModel.MediaName))
                medias = medias.Where(d =>
                    d.MediaName.Contains(viewModel.MediaName) || d.MediaID.Contains(viewModel.MediaName));
            if (viewModel.MediaTagIds != null)
                medias = medias.Include(d => d.MediaTags).Where(d => d.MediaTags.Any(t => viewModel.MediaTagIds.Contains(t.Id)));
            if (!string.IsNullOrWhiteSpace(viewModel.FansNumRange))
            {
                var temp = viewModel.FansNumRange.Split('-');
                decimal min = Convert.ToDecimal(temp[0].Trim());
                decimal max = Convert.ToDecimal(temp[1].Trim());
                medias = medias.Where(d => d.FansNum >= min && d.FansNum <= max);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.PriceRange))
            {
                var temp = viewModel.PriceRange.Split('-');
                decimal min = Convert.ToDecimal(temp[0].Trim());
                decimal max = Convert.ToDecimal(temp[1].Trim());
                medias = !string.IsNullOrWhiteSpace(viewModel.AdPositionName) ?
                    medias.Include(d => d.MediaPrices).Where(d => d.MediaPrices.Any(p => p.AdPositionName == viewModel.AdPositionName && p.PurchasePrice >= min && p.PurchasePrice <= max)) :
                    medias.Include(d => d.MediaPrices).Where(d => d.MediaPrices.Any(p => p.PurchasePrice >= min && p.PurchasePrice <= max));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AvgReadNumRange))
            {
                var temp = viewModel.AvgReadNumRange.Split('-');
                decimal min = Convert.ToDecimal(temp[0].Trim());
                decimal max = Convert.ToDecimal(temp[1].Trim());
                medias = medias.Where(d => d.AvgReadNum >= min && d.AvgReadNum <= max);
            }
            medias = medias.OrderByDescending(d => d.IsTop).ThenByDescending(d => d.IsHot).ThenByDescending(d => d.IsRecommend).ThenBy(d => d.Id)
                 .Take(10);
            return View(medias.ToList());
        }
    }
}