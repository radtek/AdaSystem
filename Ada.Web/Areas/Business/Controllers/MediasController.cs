using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DataTable = System.Data.DataTable;

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
            //var search = Request.Form["Submit.Search"];
            var export = Request.Form["Submit.Export"];
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

            if (export == "export")
            {
                JArray jObjects = new JArray();
                foreach (var media in result)
                {
                    var jo = new JObject();
                    jo.Add("媒体类型", media.MediaType.TypeName);
                    if (!string.IsNullOrWhiteSpace(media.Platform))
                    {
                        jo.Add("平台", media.Platform);
                    }
                    jo.Add("媒体名称", media.MediaName);
                    if (!string.IsNullOrWhiteSpace(media.MediaID))
                    {
                        jo.Add("媒体ID", media.MediaID);
                    }
                    jo.Add("粉丝数", media.FansNum ?? 0);
                    foreach (var mediaMediaPrice in media.MediaPrices)
                    {
                        jo.Add(mediaMediaPrice.AdPositionName, mediaMediaPrice.PurchasePrice);
                        //jo.Add(mediaMediaPrice.AdPositionName + "更新日期", mediaMediaPrice.PriceDate);
                        //jo.Add(mediaMediaPrice.AdPositionName + "失效日期", mediaMediaPrice.InvalidDate);
                    }
                    jObjects.Add(jo);
                }

                var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());
                byte[] bytes;
                using (var workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(dt, "江西微广");
                    using (var ms = new MemoryStream())
                    {
                        workbook.SaveAs(ms);
                        bytes = ms.ToArray();
                    }
                }
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表.xlsx");
            }
            ModelState.AddModelError("message", "本次查询查询耗时：" + watcher.ElapsedMilliseconds + "毫秒，共查询结果为" + result.Count + "条。注：查询结果最多显示50条");
            return View(result);


        }

    }


}
