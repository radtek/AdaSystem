using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Resource;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Ada.Services.Resource;
using Ada.Services.Setting;
using Newtonsoft.Json.Linq;

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
            var isExport = export == "export";
            viewModel.limit = isExport? setting.BusinessExportRows: setting.BusinessSeachRows;
            var results = _mediaService.LoadEntitiesFilter(viewModel).ToList();

            watcher.Stop();
            //List<string> noDatas = new List<string>();
            //找到没有的
            if (!string.IsNullOrWhiteSpace(viewModel.MediaNames))
            {
                var names = viewModel.MediaNames.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
                int i = 0;
                foreach (var name in names)
                {
                    var temp = results.FirstOrDefault(d =>
                        d.MediaName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                    if (temp == null)
                    {
                        results.Add(new Media
                        {
                            MediaName = name,
                            Taxis = i
                        });
                        //noDatas.Add(name);
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
                var ids = viewModel.MediaIDs.Split(',').Distinct().Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
                int i = 0;
                foreach (var id in ids)
                {
                    var temp = results.FirstOrDefault(d =>
                        d.MediaID.Equals(id, StringComparison.CurrentCultureIgnoreCase));
                    if (temp == null)
                    {
                        results.Add(new Media
                        {
                            MediaID = id,
                            Taxis = i
                        });
                        //noDatas.Add(id);
                    }
                    else
                    {
                        temp.Taxis = i;
                    }
                    i++;
                }
            }

            if (isExport)
            {
                JArray jObjects = new JArray();
                var priceRange = _fieldService.GetFieldsByKey("ExportPrice").ToList();
                var priceType = viewModel.PriceType;
                foreach (var media in results.OrderBy(d => d.Taxis))
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
                    jo.Add("粉丝数(万)", Utils.ShowFansNum(media.FansNum));
                    if (!string.IsNullOrWhiteSpace(media.Area))
                    {
                        jo.Add("地区", media.Area);
                    }

                    if (media.MediaType?.CallIndex == "weixin")
                    {
                        jo.Add("月发文篇数", media.MonthPostNum);
                    }
                    if (media.MediaType?.CallIndex == "weixin")
                    {
                        jo.Add("最近微信发文日期", media.LastPushDate?.ToString("yyyy-MM-dd"));
                    }
                    if (media.MediaType?.CallIndex == "sinablog")
                    {
                        jo.Add("微博数", media.PostNum);
                    }
                    if (media.MediaType?.CallIndex == "weixin")
                    {
                        jo.Add("最近头条阅读数", media.MediaArticles.Where(l => l.IsTop == true).OrderByDescending(a => a.PublishDate).FirstOrDefault()?.ViewCount);
                    }
                    if (media.MediaType?.CallIndex == "weixin")
                    {
                        jo.Add("十天平均阅读数", Convert.ToInt32(media.MediaArticles.Where(l => l.IsTop == true && l.PublishDate > DateTime.Now.Date.AddDays(-10)).Average(aaa => aaa.ViewCount)));
                    }
                    if (media.MediaType?.CallIndex == "sinablog")
                    {
                        jo.Add("转发数",Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate).Take(50).Average(aaa => aaa.ShareCount)) );
                    }
                    if (media.MediaType?.CallIndex == "sinablog")
                    {
                        jo.Add("评论数", Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate).Take(50).Average(aaa => aaa.CommentCount)));
                    }
                    if (media.MediaType?.CallIndex == "sinablog")
                    {
                        jo.Add("点赞数", Convert.ToInt32(media.MediaArticles.OrderByDescending(a => a.PublishDate).Take(50).Average(aaa => aaa.LikeCount)));
                    }
                    if (media.MediaType?.CallIndex == "sinablog")
                    {
                        jo.Add("最近博文日期", media.MediaArticles.OrderByDescending(a => a.PublishDate).FirstOrDefault()?.PublishDate.Value.ToString("yyyy-MM-dd"));
                    }
                    if (media.MediaType?.CallIndex == "sinablog")
                    {
                        jo.Add("一周博文数", media.MediaArticles.OrderByDescending(a => a.PublishDate).Count(l => l.PublishDate > DateTime.Now.Date.AddDays(-7)));
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
            viewModel.Medias = results.OrderBy(d => d.Taxis).ToList();
            if (!results.Any())
            {
                ModelState.AddModelError("message", "没有查询到相关媒体信息！");
                return View(viewModel);
            }
            //var noDataStr = string.Empty;
            //if (noDatas.Count > 0)
            //{
            //    noDataStr = "其中以下资源不存在系统中：" + string.Join(",", noDatas);
            //}
            ModelState.AddModelError("message", "本次查询查询耗时：" + watcher.ElapsedMilliseconds + "毫秒，共查询结果为" + viewModel.total + "条。注：查询结果最多显示" + setting.BusinessSeachRows + "条。");
            return View(viewModel);
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
