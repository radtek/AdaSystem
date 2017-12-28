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
using Ada.Services.Resource;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DataTable = System.Data.DataTable;

namespace Business.Controllers
{
    public class MediasController : BaseController
    {
        private readonly IMediaService _mediaService;
        public MediasController(IMediaService mediaService)
        {
            _mediaService = mediaService;
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
            var export = Request.Form["Submit.Export"];
            ViewBag.ViewModel = viewModel;
            if (string.IsNullOrWhiteSpace(viewModel.MediaTypeId))
            {
                ModelState.AddModelError("message", "请先选择媒体类型！");
                return View();
            }
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            viewModel.Status = Consts.StateNormal;
            viewModel.limit = 100;
            var result = _mediaService.LoadEntitiesFilter(viewModel).ToList();
            watcher.Stop();
            if (!result.Any())
            {
                ModelState.AddModelError("message", "没有查询到相关媒体信息！");
                return View();
            }

            if (export == "export")
            {
                //找到没有的
                if (!string.IsNullOrWhiteSpace(viewModel.MediaNames))
                {
                    var names = viewModel.MediaNames.Split(',').ToList();
                    foreach (var name in names)
                    {
                        if (result.All(d => d.MediaName != name))
                        {
                            result.Add(new Media
                            {
                                MediaName = name
                            });
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(viewModel.MediaIDs))
                {
                    var ids = viewModel.MediaIDs.Split(',').ToList();
                    foreach (var id in ids)
                    {
                        if (result.All(d => d.MediaID != id))
                        {
                            result.Add(new Media
                            {
                                MediaID = id
                            });
                        }
                    }
                }
                JArray jObjects = new JArray();
                foreach (var media in result)
                {
                    var jo = new JObject();
                    jo.Add("媒体类型", media.MediaType == null ? "不存在的资源" : media.MediaType.TypeName);
                    if (!string.IsNullOrWhiteSpace(media.Platform))
                    {
                        jo.Add("平台", media.Platform);
                    }
                    jo.Add("媒体名称", media.MediaName);
                    if (!string.IsNullOrWhiteSpace(media.Client))
                    {
                        jo.Add("客户端", media.Client);
                    }
                    if (!string.IsNullOrWhiteSpace(media.Channel))
                    {
                        jo.Add("媒体频道", media.Channel);
                    }
                    if (!string.IsNullOrWhiteSpace(media.MediaID))
                    {
                        jo.Add("媒体ID", media.MediaID);
                    }
                    if (!string.IsNullOrWhiteSpace(media.Sex))
                    {
                        jo.Add("性别", media.Sex);
                    }
                    jo.Add("粉丝数", media.FansNum ?? 0);
                    if (!string.IsNullOrWhiteSpace(media.Area))
                    {
                        jo.Add("地区", media.Area);
                    }
                    if (!string.IsNullOrWhiteSpace(media.ResourceType))
                    {
                        jo.Add("资源类型", media.ResourceType);
                    }
                    if (!string.IsNullOrWhiteSpace(media.Efficiency))
                    {
                        jo.Add("出稿速度", media.Efficiency);
                    }
                    if (!string.IsNullOrWhiteSpace(media.SEO))
                    {
                        jo.Add("收录效果", media.SEO);
                    }
                
                    foreach (var mediaMediaPrice in media.MediaPrices)
                    {
                        jo.Add(mediaMediaPrice.AdPositionName, mediaMediaPrice.PurchasePrice);
                        //jo.Add(mediaMediaPrice.AdPositionName + "更新日期", mediaMediaPrice.PriceDate);
                        //jo.Add(mediaMediaPrice.AdPositionName + "失效日期", mediaMediaPrice.InvalidDate);
                    }
                    if (!string.IsNullOrWhiteSpace(media.Remark))
                    {
                        jo.Add("备注说明", media.Remark);
                    }
                    jObjects.Add(jo);
                }
                return File(ExportData(jObjects.ToString()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
            }
            ModelState.AddModelError("message", "本次查询查询耗时：" + watcher.ElapsedMilliseconds + "毫秒，共查询结果为" + result.Count + "条。注：查询结果最多显示100条");
            return View(result);


        }

    }


}
