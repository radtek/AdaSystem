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
using Ada.Web.Models;

namespace Ada.Web.Controllers
{
    public class MediaController : Controller
    {
        private readonly IRepository<Media> _repository;
        public MediaController(IRepository<Media> repository)
        {
            _repository = repository;
        }
        public ActionResult WeiXin()
        {
            var model = new MediaView();
            GetData(model);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WeiXin(MediaView viewModel)
        {
            //var routeValues = ControllerContext.RouteData.Values;
            //if (viewModel!=null)
            //{
            //    routeValues["MediaName"] = viewModel.MediaName;
            //}
            GetData(viewModel);
            return View(viewModel);
        }

        private void GetData(MediaView viewModel)
        {
            viewModel.offset = viewModel.offset ?? 1;
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
                int min = Convert.ToInt32(temp[0].Trim()) * 10000;
                viewModel.FansNumStart = Convert.ToInt32(temp[0].Trim());
                int max = Convert.ToInt32(temp[1].Trim()) * 10000;
                viewModel.FansNumEnd = Convert.ToInt32(temp[1].Trim());
                medias = medias.Where(d => d.FansNum >= min && d.FansNum <= max);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.PriceRange))
            {
                var temp = viewModel.PriceRange.Split('-');
                decimal min = Convert.ToDecimal(temp[0].Trim());
                decimal max = Convert.ToDecimal(temp[1].Trim());
                viewModel.PriceStart = min;
                viewModel.PriceEnd = max;
                medias = !string.IsNullOrWhiteSpace(viewModel.AdPositionName) ?
                    medias.Include(d => d.MediaPrices).Where(d => d.MediaPrices.Any(p => p.AdPositionName == viewModel.AdPositionName && p.PurchasePrice >= min && p.PurchasePrice <= max)) :
                    medias.Include(d => d.MediaPrices).Where(d => d.MediaPrices.Any(p => p.PurchasePrice >= min && p.PurchasePrice <= max));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AvgReadNumRange))
            {
                var temp = viewModel.AvgReadNumRange.Split('-');
                decimal min = Convert.ToDecimal(temp[0].Trim());
                decimal max = Convert.ToDecimal(temp[1].Trim());
                viewModel.AvgReadNumStart = (int?) min;
                viewModel.AvgReadNumEnd = (int?) max;
                medias = medias.Where(d => d.AvgReadNum >= min && d.AvgReadNum <= max);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaNames))
            {
                viewModel.MediaNames = viewModel.MediaNames.Trim().Replace("\r\n", ",").Replace("，", ",").Replace(" ", ",");
                var mediaNames = viewModel.MediaNames.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
                medias = medias.Where(d => mediaNames.Contains(d.MediaName)||mediaNames.Contains(d.MediaID));
            }
            viewModel.total = medias.Count();
            medias = medias.OrderByDescending(d => d.IsTop).ThenByDescending(d => d.IsHot).ThenByDescending(d => d.IsRecommend).ThenBy(d => d.Id).Skip(viewModel.limit.Value * (viewModel.offset.Value - 1))
                 .Take(viewModel.limit.Value);
            viewModel.Medias = medias.AsNoTracking().ToList();
        }
    }
}