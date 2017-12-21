using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;

namespace Business.Controllers
{
    public class MediasController : BaseController
    {
        private readonly IRepository<Media> _mediaRepository;
        public MediasController(IRepository<Media> mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }
        public ActionResult Index()
        {
            ViewBag.ViewModel = new MediaView();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MediaView viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (string.IsNullOrWhiteSpace(viewModel.MediaTypeId))
            {
                ModelState.AddModelError("message", "请先选择媒体类型！");
                return View();
            }
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            var allList = _mediaRepository.LoadEntities(d => d.IsDelete == false && d.MediaTypeId == viewModel.MediaTypeId && d.Status == Consts.StateNormal);
            if (!string.IsNullOrWhiteSpace(viewModel.MediaNames))
            {
                var mediaNames = viewModel.MediaNames.Replace("，", ",").Split(',').ToList();
                allList = allList.Where(d => mediaNames.Contains(d.MediaName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaIDs))
            {
                var mediaIDs = viewModel.MediaIDs.Replace("，", ",").Split(',').ToList();
                allList = allList.Where(d => mediaIDs.Contains(d.MediaID));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AdPositionName))
            {
                allList = from m in allList
                          from p in m.MediaPrices
                          where p.AdPositionName == viewModel.AdPositionName
                          select m;
                if (viewModel.PriceStart != null)
                {
                    allList = from m in allList
                              from p in m.MediaPrices
                              where p.PurchasePrice >= viewModel.PriceStart && p.AdPositionName == viewModel.AdPositionName
                              select m;
                }
                if (viewModel.PriceEnd != null)
                {
                    allList = from m in allList
                              from p in m.MediaPrices
                              where p.PurchasePrice <= viewModel.PriceEnd && p.AdPositionName == viewModel.AdPositionName
                              select m;
                }

            }
            else
            {
                if (viewModel.PriceStart != null)
                {
                    allList = from m in allList
                              from p in m.MediaPrices
                              where p.PurchasePrice >= viewModel.PriceStart
                              select m;
                }
                if (viewModel.PriceEnd != null)
                {
                    allList = from m in allList
                              from p in m.MediaPrices
                              where p.PurchasePrice <= viewModel.PriceEnd
                              select m;
                }
            }

            var result = allList.OrderByDescending(d => d.Id).Take(50).ToList();
            watcher.Stop();
            if (!allList.Any())
            {
                ModelState.AddModelError("message", "没有查询到相关媒体信息！");
                return View();
            }
            ModelState.AddModelError("message", "本次查询查询耗时：" + watcher.ElapsedMilliseconds + "毫秒，共查询结果为" + result.Count + "条。注：查询结果最多显示50条");
            return View(result);
        }
    }


}
