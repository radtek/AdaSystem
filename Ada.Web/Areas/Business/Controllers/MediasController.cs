using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Ada.Services.Resource;
using Ada.Services.Setting;
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
        private readonly IFieldService _fieldService;
        private readonly ISettingService _settingService;
        public MediasController(IMediaService mediaService, ISettingService settingService, IFieldService fieldService)
        {
            _mediaService = mediaService;
            _settingService = settingService;
            _fieldService = fieldService;
        }
        public ActionResult Index()
        {
            MediaView media = new MediaView();
            return View(media);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MediaView viewModel)
        {
            var export = Request.Form["Submit.Export"];
            if (string.IsNullOrWhiteSpace(viewModel.MediaTypeId))
            {
                ModelState.AddModelError("message", "请先选择媒体类型！");
                return View(viewModel);
            }
            var setting = _settingService.GetSetting<WeiGuang>();
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            viewModel.Status = Consts.StateNormal;
            viewModel.limit = setting.BusinessSeachRows;
            var result = _mediaService.LoadEntitiesFilter(viewModel).ToList();
            viewModel.Medias = result;
            watcher.Stop();
            List<string> noDatas = new List<string>();
            //找到没有的
            if (!string.IsNullOrWhiteSpace(viewModel.MediaNames))
            {
                var names = viewModel.MediaNames.Split(',').ToList();
                int i = 0;
                foreach (var name in names)
                {
                    var temp = result.FirstOrDefault(d =>
                        d.MediaName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                    if (temp == null)
                    {
                        result.Add(new Media
                        {
                            MediaName = name,
                            Taxis = i
                        });
                        noDatas.Add(name);
                    }
                    else
                    {
                        temp.Taxis = i;
                    }

                    i++;
                }
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaIDs))
            {
                var ids = viewModel.MediaIDs.Split(',').ToList();
                int i = 0;
                foreach (var id in ids)
                {
                    var temp = result.FirstOrDefault(d =>
                        d.MediaID.Equals(id, StringComparison.CurrentCultureIgnoreCase));
                    if (temp == null)
                    {
                        result.Add(new Media
                        {
                            MediaID = id,
                            Taxis = i
                        });
                        noDatas.Add(id);
                    }
                    else
                    {
                        temp.Taxis = i;
                    }
                    i++;
                }
            }

            if (export == "export")
            {
                viewModel.limit = setting.BusinessExportRows;

                JArray jObjects = new JArray();
                var priceRange = _fieldService.GetFieldsByKey("ExportPrice").ToList();
                var priceType = viewModel.PriceType;
                foreach (var media in result.OrderBy(d => d.Taxis))
                {
                    var jo = new JObject();
                    jo.Add("主键", string.IsNullOrWhiteSpace(media.Id) ? "不存在的资源" : media.Id);
                    jo.Add("媒体类型", media.MediaType?.TypeName);
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
                    if (media.MediaTags.Count > 0)
                    {
                        jo.Add("媒体分类", string.Join(",", media.MediaTags.Select(d => d.TagName)));
                    }
                    if (!string.IsNullOrWhiteSpace(media.MediaLink))
                    {
                        jo.Add("媒体链接", media.MediaLink);
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
                    if (media.MonthPostNum != null)
                    {
                        jo.Add("月发文篇数", media.MonthPostNum);
                    }
                    if (media.LastPushDate!=null)
                    {
                        jo.Add("最近微信发文日期", media.LastPushDate);
                    }
                    if (media.PostNum != null)
                    {
                        jo.Add("微博数", media.PostNum);
                    }
                    if (media.FriendNum != null)
                    {
                        jo.Add("转发数", media.FriendNum);
                    }
                    if (media.IsAuthenticate != null)
                    {
                        jo.Add("是否认证", media.IsAuthenticate.Value ? "是" : "否");
                    }
                    if (!string.IsNullOrWhiteSpace(media.AuthenticateType))
                    {
                        jo.Add("微博认证", media.AuthenticateType);
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
                        var price = mediaMediaPrice.PurchasePrice ?? 0;
                        if (priceType == "1")
                        {
                            price = SetSalePrice(price, priceRange);
                        }
                        jo.Add(mediaMediaPrice.AdPositionName, price);
                    }
                    jo.Add("价格日期", media.MediaPrices.FirstOrDefault()?.PriceDate);
                    if (!string.IsNullOrWhiteSpace(media.Abstract))
                    {
                        jo.Add("媒体摘要", media.Abstract);
                    }

                    jo.Add("媒体说明", media.Content);
                    jo.Add("备注说明", media.Remark);
                    jo.Add("经办媒介", media.Transactor);
                    jObjects.Add(jo);
                }
                return File(ExportData(jObjects.ToString()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
            }
            else
            {

                if (!result.Any())
                {
                    ModelState.AddModelError("message", "没有查询到相关媒体信息！");
                    return View(viewModel);
                }

                var noDataStr = string.Empty;
                if (noDatas.Count > 0)
                {
                    noDataStr = "其中以下资源不存在系统中：" + string.Join(",", noDatas);
                }
                ModelState.AddModelError("message", "本次查询查询耗时：" + watcher.ElapsedMilliseconds + "毫秒，共查询结果为" + viewModel.total + "条。注：查询结果最多显示" + setting.BusinessSeachRows + "条。" + noDataStr);
                return View(viewModel);
            }
        }
        private decimal SetSalePrice(decimal price, IEnumerable<Field> priceRanges)
        {
            if (price <= 0) return 0;
            foreach (var range in priceRanges)
            {
                var qj = range.Text.Split('-');
                if (price >= decimal.Parse(qj[0]) && price <= decimal.Parse(qj[1]))
                {
                    return PriceZero(decimal.Parse(range.Value) + price);
                }
            }
            return 0;
        }
        private decimal PriceZero(decimal a)
        {
            if (a >= 100000)
            {
                return (int)a / 1000 * 1000;
            }
            if (a >= 10000)
            {
                return (int)a / 1000 * 1000;
            }
            if (a >= 1000)
            {
                return (int)a / 100 * 100;
            }
            if (a >= 100)
            {
                return (int)a / 100 * 100;
            }
            return a;
        }
    }


}
