using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Services.Cache;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Ada.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly IRepository<BusinessOrderDetail> _temp;
        private readonly IRepository<Media> _media;
        private readonly IRepository<Field> _fieldRepository;
        private readonly IRepository<MediaArticle> _mediaArticleRepository;
        private readonly IRepository<MediaPrice> _mediaPrice;
        private readonly IRepository<MediaType> _mediaType;
        private readonly IRepository<PurchaseOrderDetail> _ptemp;
        private readonly IDbContext _dbContext;
        private readonly ICacheService _cacheService;

        public TestController(
            IDbContext dbContext,
            IRepository<BusinessOrderDetail> temp,
            IRepository<PurchaseOrderDetail> ptemp,
            IRepository<Media> media,
            IRepository<MediaArticle> mediaArticle,
            IRepository<MediaPrice> mediaPrice,
            IRepository<BusinessOrder> businessOrder,
            IRepository<MediaType> mediaType,
            IRepository<Field> fieldRepository,
            ICacheService cacheService,
            IRepository<MediaArticle> mediaArticleRepository)
        {
            _dbContext = dbContext;
            _ptemp = ptemp;
            _temp = temp;
            _media = media;
            _mediaType = mediaType;
            _mediaPrice = mediaPrice;
            _fieldRepository = fieldRepository;
            _cacheService = cacheService;
            _mediaArticleRepository = mediaArticleRepository;
        }

        public ActionResult CheckOrder()
        {

            var b = _temp.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false).ToList();
            List<string> ids = new List<string>();
            foreach (var item in b)
            {
                if (_ptemp.LoadEntities(d => d.BusinessOrderDetailId == item.Id).Count() > 1)
                {
                    ids.Add(item.Id);
                }
            }

            var business = _temp
                .LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false && d.Status != 0).Count();
            var purchase = _ptemp.LoadEntities(d => d.IsDelete == false).Count();

            if (ids.Count > 0)
            {
                return Content("存在重复订单：" + string.Join(",", ids));
            }

            return Content("未找到重复订单，销售订单数：" + business + "，采购订单数：" + purchase);


        }
        public ActionResult CheckOrderMoney()
        {

            var business = _temp
                .LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false && d.Status != 0).ToList();
            List<string> temp = new List<string>();
            foreach (var businessOrderDetail in business)
            {
                var sell = businessOrderDetail.SellMoney;
                var verificationMoney = businessOrderDetail.VerificationMoney;
                var confirmMoney = businessOrderDetail.ConfirmVerificationMoney;
                var total = verificationMoney + confirmMoney;
                if (sell != total)
                {
                    temp.Add("订单ID：" + businessOrderDetail.Id + "，媒体名称：" + businessOrderDetail.MediaName);
                }
            }

            return Content("金额不一致的订单：" + string.Join("&", temp));


        }
        public ActionResult Update()
        {

            //var b = _temp.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false && d.VerificationStatus == Consts.StateLock).ToList();
            //int i = 0;
            //foreach (var businessOrderDetail in b)
            //{
            //    if (businessOrderDetail.VerificationMoney != businessOrderDetail.SellMoney)
            //    {
            //        businessOrderDetail.VerificationMoney = businessOrderDetail.SellMoney;
            //        i++;
            //    }


            //}
            var p = _ptemp.LoadEntities(d => d.IsDelete == false);
            int i = 0;
            foreach (var purchaseOrderDetail in p)
            {
                var tax = purchaseOrderDetail.Tax ?? 0;
                purchaseOrderDetail.Money = purchaseOrderDetail.PurchaseMoney * (1 + tax / 100);
                i++;
            }
            _dbContext.SaveChanges();
            return Content(i.ToString());


        }
        public ActionResult UpdateMedia()
        {


            var count = _mediaArticleRepository.Update(d => d.Media.MediaType.CallIndex == "weixin",
                 a => new MediaArticle() { Content = null });
            _dbContext.SaveChanges();
            return Content("成功更新" + count + "个");
            //return Content("原有：" + start + ",去重后：" + end);


        }

        public ActionResult CheckMedia()
        {

            var m = _media.LoadEntities(d => d.IsDelete == false && d.MediaType.CallIndex == "sinablog").ToList();
            List<string> ids = new List<string>();
            List<string> isnulls = new List<string>();
            foreach (var item in m)
            {
                if (string.IsNullOrWhiteSpace(item.MediaID))
                {
                    isnulls.Add(item.MediaName);
                    continue;
                }
                var temp = _media.LoadEntities(d =>
                    d.MediaID.Equals(item.MediaID.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                    d.IsDelete == false &&
                    d.MediaTypeId == item.MediaTypeId && d.Id != item.Id).FirstOrDefault();
                if (temp != null)
                {
                    ids.Add(item.MediaID);
                }
            }

            return Content("存在重复资源：" + string.Join(",", ids) + "，媒体ID是空的：" + string.Join(",", isnulls));


        }

        public ActionResult Test()
        {
            string[] where = { "X1712181402100028", "X1712181349560012", "X1712181221160001", "X1712181337340002" };
            var meidas = _media.LoadEntities(d =>
                d.IsDelete == false &&
                where.Contains(d.TransactorId) &&
                d.MediaType.CallIndex == "weixin" &&
                d.IsSlide == true &&
                d.Status == Consts.StateNormal &&
                (d.CollectionDate == null || SqlFunctions.DateDiff("hour", d.CollectionDate, DateTime.Now) > 0)).Count();



            return Content(meidas.ToString());
            ////var atricles = _mediaArticle.LoadEntities(d => true).GroupBy(d => d.MediaId).Select(d => new MediaView
            ////{
            ////    MediaName = d.Key.MediaName,
            ////    Id=d.Key.Id,
            ////    LastReadNum = d.Where(l => l.IsTop == true && l.Media.MediaType.CallIndex == "weixin").OrderByDescending(a => a.PublishDate).FirstOrDefault().ViewCount,
            ////});
            ////var temp = meidas.Join(atricles, m => m.Id, a => a.Key, (m, d) => new MediaView()
            ////{
            ////    MediaName = m.MediaName,
            ////    LastReadNum= d.Where(l => l.IsTop == true && l.Media.MediaType.CallIndex == "weixin").OrderByDescending(a => a.PublishDate).FirstOrDefault().ViewCount
            ////});
            //var temp = meidas.Select(d => new MediaView
            //{
            //    MediaName = d.MediaName,
            //    LastReadNum= d.MediaArticles.Where(l => l.IsTop == true).OrderByDescending(a => a.PublishDate).FirstOrDefault().ViewCount,
            //    AvgReadNum= (int?) d.MediaArticles.Where(a => SqlFunctions.DateDiff("day", a.PublishDate, DateTime.Now) <= 10).Average(a => a.ViewCount),

            //});

            //return Json(temp,JsonRequestBehavior.AllowGet);
        }

        public ActionResult DouYinUpdate1()
        {
            var douyins = _mediaPrice.LoadEntities(d => d.Media.Platform == "抖音" && d.Media.IsDelete == false && d.AdPositionName == "转发价格");
            var count = douyins.Count();
            //删除转发价格
            _mediaPrice.Remove(douyins);
            _dbContext.SaveChanges();
            return Content("删除" + count + "条");
            //var type = _mediaType.LoadEntities(d => d.CallIndex == "douyin").FirstOrDefault();
            //if (type != null)
            //{
            //    foreach (var douyin in douyins)
            //    {

            //        douyin.Media.MediaTypeId = type.Id;
            //        if (douyin.AdPositionName=="线上直播")
            //        {
            //            douyin
            //        }
            //    }
            //}

        }
        public ActionResult DouYinUpdate2()
        {
            var douyins = _media.LoadEntities(d => d.Platform == "抖音" && d.IsDelete == false).ToList();
            var type = _mediaType.LoadEntities(d => d.CallIndex == "douyin").FirstOrDefault();
            var count = 0;
            if (type != null)
            {
                foreach (var douyin in douyins)
                {
                    douyin.MediaTypeId = type.Id;
                    var uid = GetDouYinId(douyin.MediaLink);
                    douyin.MediaID = string.IsNullOrWhiteSpace(uid) ? null : uid;
                    count++;
                }
            }
            _dbContext.SaveChanges();
            return Content("更新抖音类型" + count + "条");

        }
        public ActionResult DouYinUpdate3()
        {
            var douyins = _mediaPrice.LoadEntities(d => d.Media.MediaType.CallIndex == "douyin" && d.Media.IsDelete == false);
            var count = 0;
            foreach (var douyin in douyins)
            {

                if (douyin.AdPositionName == "线上直播")
                {
                    douyin.AdPositionName = "线上活动";
                    douyin.AdPositionId = "X1804031723448509";
                }
                if (douyin.AdPositionName == "线下直播")
                {
                    douyin.AdPositionName = "线下活动";
                    douyin.AdPositionId = "X1804031723448510";
                }
                if (douyin.AdPositionName == "直发价格")
                {
                    douyin.AdPositionName = "直发";
                    douyin.AdPositionId = "X1804031723448511";
                }
                if (douyin.AdPositionName == "原创价格")
                {
                    douyin.AdPositionName = "原创";
                    douyin.AdPositionId = "X1804031723448512";
                }

                count++;
            }
            _dbContext.SaveChanges();
            return Content("更新抖音价格" + count + "条");
        }
        public ActionResult RedBookId()
        {
            var douyins = _media.LoadEntities(d => d.MediaTypeId == "X1712181059130008" && d.IsDelete == false).ToList();
            var count = 0;
            foreach (var douyin in douyins)
            {

                var uid = GetRedBookId(douyin.MediaLink);
                douyin.MediaID = string.IsNullOrWhiteSpace(uid) ? null : uid;
                count++;
            }
            _dbContext.SaveChanges();
            return Content("更新小红书类型" + count + "条");

        }
        public ActionResult Temp()
        {
            string path = Server.MapPath("~/upload/sell.xlsx");
            int count = 0;
            using (FileStream ms = new FileStream(path, FileMode.Open))
            {
                //创建工作薄
                IWorkbook wk = new XSSFWorkbook(ms);
                //1.获取第一个工作表
                ISheet sheet = wk.GetSheetAt(0);
                if (sheet.LastRowNum <= 1)
                {
                    return Content("此文件没有导入数据，请填充数据再进行导入");
                }
                List<Field> list = new List<Field>();
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    var text = row.GetCell(0)?.ToString();
                    var value = row.GetCell(1)?.ToString();
                    var sort = row.GetCell(2)?.ToString();
                    Field field = new Field();
                    field.Id = IdBuilder.CreateIdNum();
                    field.FieldTypeId = "X1804261113343734";
                    field.Text = text;
                    field.Value = value;
                    field.Taxis = Convert.ToInt32(sort);
                    list.Add(field);
                    count++;
                }
                _fieldRepository.Add(list);
                //List<string> list = new List<string>();
                //for (int i = 1; i <= sheet.LastRowNum; i++)
                //{
                //    IRow row = sheet.GetRow(i);
                //    var id = row.GetCell(0)?.ToString();
                //    if (!string.IsNullOrWhiteSpace(id))
                //    {
                //        list.Add(id);
                //    }
                //}

                //count = _media.Update(d => list.Contains(d.Id), m => new Media() { IsHot = true });

                _dbContext.SaveChanges();
            }
            return Content("导入成功" + count + "条资源");
        }

        public ActionResult Cache(string id)
        {
            var cache = _cacheService.GetObject<string>(id);
            return cache == null ? Content("无此KEY的缓存值") : Content("缓存值：" + cache);
        }

        public ActionResult ChangeMedia()
        {
            var medias = _media.LoadEntities(d => d.TransactorId == "X1801091653080193").ToList();
            foreach (var media in medias)
            {
                media.Transactor = "戴秋红";
                media.TransactorId = "X1712181402380029";
                media.LinkMan.Transactor = "戴秋红";
                media.LinkMan.TransactorId = "X1712181402380029";
                media.LinkMan.Commpany.Transactor = "戴秋红";
                media.LinkMan.Commpany.TransactorId = "X1712181402380029";
            }

            _dbContext.SaveChanges();
            return Content("成功转移：" + medias.Count());
        }
        public ActionResult ChangeMediaXls()
        {
            string path = Server.MapPath("~/upload/change.xlsx");
            int count = 0;
            using (FileStream ms = new FileStream(path, FileMode.Open))
            {
                //创建工作薄
                IWorkbook wk = new XSSFWorkbook(ms);
                //1.获取第一个工作表
                ISheet sheet = wk.GetSheetAt(0);
                if (sheet.LastRowNum <= 1)
                {
                    return Content("此文件没有导入数据，请填充数据再进行导入");
                }

                var username = "吴璇";
                var userId = "X1801180851430024";
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    var mediaId = row.GetCell(0)?.ToString();
                    var media = _media.LoadEntities(d => d.Id == mediaId.Trim()).FirstOrDefault();
                    if (media == null) continue;
                    media.Transactor = username;
                    media.TransactorId = userId;
                    media.LinkMan.Transactor = username;
                    media.LinkMan.TransactorId = userId;
                    media.LinkMan.Commpany.Transactor = username;
                    media.LinkMan.Commpany.TransactorId = userId;
                    count++;
                }

                _dbContext.SaveChanges();
            }
            return Content("成功更换" + count + "条资源");
        }
        public ActionResult CloseMediaXls()
        {
            string path = Server.MapPath("~/upload/close.xlsx");
            int count = 0;
            using (FileStream ms = new FileStream(path, FileMode.Open))
            {
                //创建工作薄
                IWorkbook wk = new XSSFWorkbook(ms);
                //1.获取第一个工作表
                ISheet sheet = wk.GetSheetAt(0);
                if (sheet.LastRowNum <= 1)
                {
                    return Content("此文件没有导入数据，请填充数据再进行导入");
                }
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    var mediaId = row.GetCell(0)?.ToString();
                    var media = _media.LoadEntities(d => d.Id == mediaId.Trim()).FirstOrDefault();
                    if (media == null) continue;
                    media.Status = 0;
                    count++;
                }

                _dbContext.SaveChanges();
            }
            return Content("成功关闭" + count + "条资源");
        }
        public ActionResult ChangeLinkMan()
        {
            string path = Server.MapPath("~/upload/link.xlsx");
            int count = 0;
            using (FileStream ms = new FileStream(path, FileMode.Open))
            {
                //创建工作薄
                IWorkbook wk = new XSSFWorkbook(ms);
                //1.获取第一个工作表
                ISheet sheet = wk.GetSheetAt(0);
                if (sheet.LastRowNum <= 1)
                {
                    return Content("此文件没有导入数据，请填充数据再进行导入");
                }
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    var mediaId = row.GetCell(0)?.ToString();
                    //var linkname = row.GetCell(1)?.ToString();
                    var linkId = row.GetCell(2)?.ToString();
                    var media = _media.LoadEntities(d => d.Id == mediaId.Trim()).FirstOrDefault();
                    if (media == null) continue;
                    media.LinkManId = linkId;
                    count++;
                }

                _dbContext.SaveChanges();
            }
            return Content("成功更换" + count + "条资源");
        }

        public ActionResult Brand()
        {
            string path = Server.MapPath("~/upload/brand.xlsx");
            int count = 0;
            using (FileStream ms = new FileStream(path, FileMode.Open))
            {
                //创建工作薄
                IWorkbook wk = new XSSFWorkbook(ms);
                //1.获取第一个工作表
                ISheet sheet = wk.GetSheetAt(0);
                if (sheet.LastRowNum <= 1)
                {
                    return Content("此文件没有导入数据，请填充数据再进行导入");
                }
                IList<string> list = new List<string>();
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    var brand = row.GetCell(0)?.ToString();
                    if (!string.IsNullOrWhiteSpace(brand))
                    {
                        brand = brand.Trim();
                        list.Add(brand);
                    }
                }
                //去重
                var result = list.Distinct();
                List<Field> fields = new List<Field>();
                foreach (var item in result)
                {
                    Field field = new Field();
                    field.Id = IdBuilder.CreateIdNum();
                    field.FieldTypeId = "X1804131418551871";
                    field.Text = item;
                    field.Value = item;
                    field.Taxis = 0;
                    fields.Add(field);
                }

                count = fields.Count;
                if (fields.Any())
                {
                    _fieldRepository.Add(fields);
                    _dbContext.SaveChanges();
                }

            }
            return Content("成功导入" + count + "条品牌");
        }

        public ActionResult Bitspaceman()
        {
            var result = HttpUtility.Post(
                  "http://120.76.205.241:8000/nlp/segment/bitspaceman?apikey=aHkIQg6KZL5nKgqhcAbrT7AYq484DkAfmFzd8rBgYDrK6CItsvAAOWwz7BiFkoQx",
                  new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("text", "骆驼罩杯们姨妈别克隆重的来向你们宣布一个消息我们的微信小程序“张大奕说”在 1月22日晚20点 将首次上线 日后将与淘宝同步上新以后在微信里也可以放肆的买买买了而且拼团优惠券巨划算，优惠不重样你们想要的通通有有木有很激动~~~“张大奕说”是姨妈自己取的名字虽然被说像脱口秀的名字哈哈哈但是我觉得很好啊换种方式陪伴在你们身边听姨妈继续碎碎念继续给你们种下一片大草原小程序首次上新，可能会有一些不足就让我们一起成长吧~从无到有的过程真的很值得期待然后带大家正式认识一下我们的“张大奕说”") });
            if (!string.IsNullOrWhiteSpace(result))
            {
                var resultJson = JsonConvert.DeserializeObject<BitspacemanJSON>(result);
                var brands = _fieldRepository.LoadEntities(d => d.FieldType.CallIndex == "Brand").Select(d => d.Text).ToList();
                var words = resultJson.wordList.Where(d => d.length > 1 && brands.Contains(d.word)).Select(d => d.word);
                words = words.Distinct().ToList();
                return Content(string.Join(",", words));
            }

            return Content(result);
        }
        public ActionResult Order()
        {
            int count =
            _ptemp.Update(d => d.LinkManName == "新芳" && !d.PurchasePaymentOrderDetails.Any() && d.LinkManId == "X1801151342500244",
                p => new PurchaseOrderDetail() { LinkManId = "X1801081415310947" });
            _dbContext.SaveChanges();
            return Content("成功更换" + count + "条资源");
        }
        public ActionResult MediaUpdate()
        {
            int count =
                _media.Update(d => d.LinkManId == "X1801151511230002",
                    p => new Media() { LinkManId = "X1803131311440593" });
            _dbContext.SaveChanges();
            return Content("成功更换" + count + "条资源");
        }
        public ActionResult Tool(string id)
        {
            // RandomHelper random = new RandomHelper();
            var request = Request.Url.Scheme + "://" + Request.Url.Authority;
            var code = Utils.IsMobilePhone(id);
            return Content(code.ToString());
        }

        public async Task<ActionResult> Http()
        {
            var content = await HttpUtility.GetAsync("http://whois.pconline.com.cn/ip.jsp");
            return Content(content);
        }
        private string GetDouYinId(string url)
        {
            string result = string.Empty;
            if (string.IsNullOrWhiteSpace(url))
            {
                return result;
            }
            Match match = Regex.Match(url, @"/share/user/(\d+)", RegexOptions.ECMAScript);
            result = match.Groups[1].Value;

            return result;

        }
        private string GetRedBookId(string url)
        {
            string result = string.Empty;
            if (string.IsNullOrWhiteSpace(url))
            {
                return result;
            }
            Match match = Regex.Match(url, @"/user/profile/(\w+)", RegexOptions.ECMAScript);
            result = match.Groups[1].Value;
            if (string.IsNullOrWhiteSpace(result))
            {
                Match match2 = Regex.Match(url, @"oid=user\.(\w+)", RegexOptions.ECMAScript);
                result = match2.Groups[1].Value;
            }
            return result;

        }
    }
}