using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Common;
using Ada.Core.Domain.Customer;
using Ada.Core.Domain.Purchase;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.API.iDataAPI;
using Ada.Core.ViewModel.Business;
using Ada.Services.Business;
using Ada.Services.Cache;
using Ada.Services.Common;
using ClosedXML.Excel;
using MvcThrottle;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Action = System.Action;

namespace Ada.Web.Controllers
{
    [EnableThrottling]
    public class TestController : Controller
    {
        private readonly IRepository<BusinessOrderDetail> _temp;
        private readonly IRepository<Media> _media;
        private readonly IRepository<Field> _fieldRepository;
        private readonly IRepository<MediaArticle> _mediaArticleRepository;
        private readonly IRepository<MediaPrice> _mediaPrice;
        private readonly IRepository<MediaType> _mediaType;
        private readonly IRepository<Manager> _manager;
        private readonly IRepository<PurchaseOrderDetail> _ptemp;
        private readonly IRepository<LinkMan> _linkman;
        private readonly IRepository<Commpany> _commpanyRepository;
        private readonly IDbContext _dbContext;
        private readonly ICacheService _cacheService;//IBusinessOrderDetailService
        private readonly IRepository<BusinessInvoice> _invoice;
        private readonly IBusinessWriteOffService _businessWriteOffService;
        private readonly IRepository<BusinessWriteOffDetail> _businessWriteOffDetail;
        private readonly IRepository<Fans> _fans;
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
            IRepository<MediaArticle> mediaArticleRepository,
            IBusinessOrderDetailService businessOrderDetailService,
            IRepository<Manager> manager,
            IRepository<LinkMan> linkman,
            IRepository<Commpany> commpanyRepository,
            IRepository<BusinessInvoice> invoice,
            IBusinessWriteOffService businessWriteOffService,
            IRepository<BusinessWriteOffDetail> businessWriteOffDetail,
            IRepository<Fans> fans)
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
            _invoice = invoice;
            _manager = manager;
            _linkman = linkman;
            _commpanyRepository = commpanyRepository;
            _businessWriteOffService = businessWriteOffService;
            _businessWriteOffDetail = businessWriteOffDetail;
            _fans = fans;
        }

        public ActionResult Error()
        {
            throw new ApplicationException("测试异常");
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
        public ActionResult UpdateInvoice()
        {

            var lists = _invoice.LoadEntities(d => d.IsDelete == false);
            foreach (var item in lists)
            {
                item.Company = item.Company.Trim().Replace(" ", "").Replace("　", "");
            }
            _dbContext.SaveChanges();
            return Content("发票公司名称更新完成");


        }
        public ActionResult UpdateMedia()
        {


            var count = _mediaArticleRepository.Update(d => d.Media.MediaType.CallIndex == "weixin",
                 a => new MediaArticle() { Content = null });
            _dbContext.SaveChanges();
            return Content("成功更新" + count + "个");
            //return Content("原有：" + start + ",去重后：" + end);


        }

        public ActionResult CheckMedia(string n)
        {

            var m = _media.LoadEntities(d => d.IsDelete == false && d.MediaType.CallIndex == n).ToList();
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
        public ActionResult Fans()
        {
            string path = Server.MapPath("~/upload/fans.xlsx");
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
                    var id = row.GetCell(0)?.ToString();
                    var range1 = row.GetCell(1)?.ToString();
                    var range2 = row.GetCell(2)?.ToString();
                    var ids = row.GetCell(3)?.ToString();
                    var parent = _fans.LoadEntities(d => d.Id == id).FirstOrDefault();
                    parent.AvatarRange = range1 + "-" + range2;
                    var arry = ids.Split(',').ToList();
                    _fans.Update(d => arry.Contains(d.Id), f => new Fans() { ParentId = id });
                    count++;
                }
                _dbContext.SaveChanges();
            }
            return Content("导入更新" + count + "条FANS资源");
        }
        public ActionResult Cache(string id)
        {
            var cache = _cacheService.GetObject<string>(id);
            return cache == null ? Content("无此KEY的缓存值") : Content("缓存值：" + cache);
        }

        public ActionResult ChangeMedia()
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
                    //media.Status = 0;
                    //media.LinkManId = "X1712181731290052";
                    count++;
                }

                _dbContext.SaveChanges();
            }
            return Content("成功更改" + count + "条资源");
        }
        public ActionResult ChangeMediaXls(string name, string id)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(id))
            {
                return Content("参数错误");
            }
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

                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    var mediaId = row.GetCell(0)?.ToString();
                    var media = _media.LoadEntities(d => d.Id == mediaId.Trim()).FirstOrDefault();
                    if (media == null) continue;
                    media.Transactor = name;
                    media.TransactorId = id;
                    media.LinkMan.Transactor = name;
                    media.LinkMan.TransactorId = id;

                    count++;
                }

                _dbContext.SaveChanges();
            }
            return Content("成功更换" + count + "条资源");
        }
        public ActionResult ChangeMediaForLinkmanXls(string name, string id)
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

                var transactor = name;
                var transactorId = id;
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    var mediaId = row.GetCell(0)?.ToString();
                    var linkId = row.GetCell(1)?.ToString();
                    var media = _media.LoadEntities(d => d.Id == mediaId.Trim()).FirstOrDefault();
                    if (media == null) continue;
                    if (!string.IsNullOrWhiteSpace(linkId))
                    {
                        var lid = linkId;
                        var link = _linkman.LoadEntities(d => d.Id == lid.Trim()).FirstOrDefault();
                        if (link == null)
                        {
                            continue;
                        }
                    }
                    media.Transactor = transactor;
                    media.TransactorId = transactorId;
                    if (!string.IsNullOrWhiteSpace(linkId))
                    {
                        media.LinkManId = linkId;
                    }
                    else
                    {
                        media.LinkMan.Transactor = transactor;
                        media.LinkMan.TransactorId = transactorId;
                    }

                    media.IsDelete = false;
                    count++;
                }

                _dbContext.SaveChanges();
            }
            return Content("成功更换" + count + "条资源");
        }
        public ActionResult ChangeOrder()
        {
            var result = _ptemp.Update(d => d.IsDelete == false && !d.PurchasePaymentOrderDetails.Any() && d.TransactorId == "X1807121117097012",
                o => new PurchaseOrderDetail() { TransactorId = "X1809031204013244", Transactor = "刘娟" });
            return Content("成功更换" + result + "条订单");
        }
        public ActionResult ChangeOrderXls()
        {
            string path = Server.MapPath("~/upload/order.xlsx");
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
                    var orderId = row.GetCell(0)?.ToString();
                    var order = _ptemp.LoadEntities(d => d.Id == orderId.Trim()).FirstOrDefault();
                    if (order == null) continue;
                    order.Transactor= row.GetCell(1)?.ToString();
                    order.TransactorId = row.GetCell(2)?.ToString();
                    count++;
                }
                _dbContext.SaveChanges();
            }
            return Content("成功转移" + count + "条订单");
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
                    //media.Status = 0;
                    media.IsDelete = true;
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

        public ActionResult DeletePrice()
        {
            var count = _mediaPrice.Update(d => d.AdPositionName == "长笔记", u => new MediaPrice() { IsDelete = true });
            _dbContext.SaveChanges();
            return Content("删除" + count + "条价格");
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
        public ActionResult OrderChangeLinkman(string l, string c)
        {
            int count =
            _ptemp.Update(d => !d.PurchasePaymentOrderDetails.Any() && d.LinkManId == l,
                p => new PurchaseOrderDetail() { LinkManId = c });
            _dbContext.SaveChanges();
            return Content("成功更换" + count + "条资源");
        }
        public ActionResult MediaUpdate()
        {
            //int count =
            //    _media.Update(d => d.LinkManId == "X1801151511230002",
            //        p => new Media() { LinkManId = "X1803131311440593" });
            var medias = _media.LoadEntities(d => d.IsDelete == false && d.MediaName.Contains(" ")).ToList();
            foreach (var media in medias)
            {
                media.MediaName = media.MediaName.Replace(" ", "");
            }
            _dbContext.SaveChanges();
            return Content("成功更换" + medias.Count + "条资源");
        }
        //public ActionResult WriteOff()
        //{
        //    var result = _businessWriteOffService.LoadEntitiesFilter(new BusinessWriteOffDetailView());
        //    IList<BusinessWriteOffDetail> list = new List<BusinessWriteOffDetail>();
        //    foreach (var businessWriteOffDetailView in result)
        //    {
        //        BusinessWriteOffDetail item = new BusinessWriteOffDetail();
        //        item.Id = IdBuilder.CreateIdNum();
        //        item.BusinessWriteOffId = businessWriteOffDetailView.Id;
        //        item.BusinessOrderId = businessWriteOffDetailView.OrderId;
        //        item.BusinessOrderDetailId = businessWriteOffDetailView.OrderDetailId;
        //        item.CostMoney = businessWriteOffDetailView.PurchaseMoney ?? 0;
        //        item.MoneyBackDay = businessWriteOffDetailView.ReturnDays ?? 0;
        //        item.Percentage = businessWriteOffDetailView.Percentage ?? 0;
        //        item.SellMoney = businessWriteOffDetailView.BusinessMoney ?? 0;
        //        item.Commission = businessWriteOffDetailView.Commission ?? 0;
        //        item.MediaTypeId = businessWriteOffDetailView.MediaTypeId;
        //        item.Profit = businessWriteOffDetailView.Profit ?? 0;
        //        item.PublishDate = businessWriteOffDetailView.PublishDate;
        //        list.Add(item);
        //    }
        //    _businessWriteOffDetail.Add(list);
        //    _dbContext.SaveChanges();
        //    return Content("提成明细添加成功：共" + list.Count + "条");
        //}
        public ActionResult Tool()
        {
            //var result=  _businessOrderDetailService.BusinessPerformanceGroupByMediaType(new BusinessOrderDetailView()
            //  {
            //      PublishDateStart=new DateTime(2018,5,1),
            //      PublishDateEnd = new DateTime(2018,5,31)
            //  });
            //  return Json(result.ToList(),JsonRequestBehavior.AllowGet);
            //var medias = _media.Update(d => d.IsDelete == false && d.LinkMan.IsDelete,d=>new Media(){IsDelete = true});
            //_dbContext.SaveChanges();
            var medias = _media.LoadEntities(d => d.IsDelete == false & d.TransactorId != d.LinkMan.TransactorId).ToList();

            //var medias = _media.Update(d => d.IsDelete == false && d.TransactorId != d.LinkMan.TransactorId, d => new Media() { IsDelete = true });
            //_dbContext.SaveChanges();
            return Json(medias.Select(d => new { d.Id, d.MediaType.TypeName, d.MediaName, d.MediaID }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateManager()
        {
            string path = Server.MapPath("~/upload/manager.xlsx");
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
                    var managerId = row.GetCell(0)?.ToString();
                    var manager = _manager.LoadEntities(d => d.Id == managerId).FirstOrDefault();
                    if (manager == null) continue;
                    manager.QuartersId = row.GetCell(1)?.ToString();
                    manager.BankName = row.GetCell(3)?.ToString();
                    manager.BankAccount = row.GetCell(4)?.ToString();
                    manager.BankNum = row.GetCell(2)?.ToString();
                    count++;
                }

                _dbContext.SaveChanges();
            }
            return Content("成功更新" + count + "条资源");
        }
        public ActionResult UpdateCommpany()
        {
            string path = Server.MapPath("~/upload/commpany.xlsx");
            int count = 0;
            List<string> noList = new List<string>();
            List<string> noSelf = new List<string>();
            using (FileStream ms = new FileStream(path, FileMode.Open))
            {
                //创建工作薄
                IWorkbook wk = new XSSFWorkbook(ms);
                //1.获取第一个工作表
                ISheet sheet = wk.GetSheetAt(0);
                if (sheet.LastRowNum < 1)
                {
                    return Content("此文件没有导入数据，请填充数据再进行导入");
                }

                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    var name = row.GetCell(0)?.ToString();
                    var manager = _commpanyRepository.LoadEntities(d => d.Name == name).FirstOrDefault();
                    if (manager == null)
                    {
                        noList.Add(name);
                        continue;
                    }
                    //验证是否包含自运营
                    var isSelf = manager.LinkMans.Any(d => d.BusinessOrders.Any(b =>
                        b.BusinessOrderDetails.Any(o =>
                            o.MediaPrice.Media.Cooperation == 1 &&
                            o.MediaPrice.Media.MediaType.CallIndex == "weixin")));
                    if (isSelf)
                    {
                        manager.IsCooperation = true;
                    }
                    else
                    {
                        noSelf.Add(name);
                    }


                    count++;
                }

                _dbContext.SaveChanges();
            }
            return Content("成功更新" + count + "条公司，" + "其中" + string.Join(",", noList) + "不存在,其中" + string.Join(",", noSelf) + "没有自运营订单");
        }
        public ActionResult Export()
        {
            var medias = _media.LoadEntities(d => d.IsDelete == false & d.TransactorId != d.LinkMan.TransactorId).ToList();
            JArray jObjects = new JArray();
            foreach (var media in medias)
            {
                var jo = new JObject();
                jo.Add("Id", media.Id);
                jo.Add("媒体类型", media.MediaType.TypeName);
                jo.Add("媒体名称", media.MediaName);
                jo.Add("媒体ID", media.MediaID);
                jo.Add("归属媒介", media.Transactor);
                jo.Add("结算人", media.LinkMan.Name);
                jo.Add("结算人归属", media.LinkMan.Transactor);
                jObjects.Add(jo);
            }
            var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());
            byte[] bytes;
            using (var workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dt, "媒体异常数据");
                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    bytes = ms.ToArray();
                }
            }
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "媒体异常数据.xlsx");

        }
        public ActionResult ExportBusiness()
        {
            DateTime start = new DateTime(2018, 1, 1);
            DateTime end = new DateTime(2019, 1, 1);
            var orders = _temp
                .LoadEntities(d =>
                    d.BusinessOrder.IsDelete == false &&
                    (d.Status == Consts.StateOK || d.Status == Consts.StateNormal) &&
                    d.VerificationStatus == Consts.StateLock);
            var pOrders = _ptemp.LoadEntities(d => d.IsDelete == false);
            orders= from b in orders
                    from p in pOrders
                    where b.Id == p.BusinessOrderDetailId && p.PublishDate >= start&&p.PublishDate<end
                select b;
            var group = orders.GroupBy(d => d.BusinessOrder.LinkMan).Select(d => new
            {
                d.Key,
                Total = d.Sum(o => o.VerificationMoney)
            }).OrderBy(d => d.Total).ToList();
            JArray jObjects = new JArray();
            foreach (var item in group)
            {
                var jo = new JObject();
                jo.Add("公司名称", item.Key.Commpany.Name);
                jo.Add("联系人", item.Key.Name);
                jo.Add("未核销总额", item.Total);
                jo.Add("经办人员", item.Key.Transactor);
                jObjects.Add(jo);
            }
            var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());
            byte[] bytes;
            using (var workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dt, "2018年未核销金额汇总");
                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    bytes = ms.ToArray();
                }
            }
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "2018年未核销金额汇总.xlsx");

        }
        public ActionResult MediaUpdateByDouYinPrice(string n, string c)
        {
            int count =
                _mediaPrice.Update(d => d.Media.MediaType.Id == "X1804031723358488" && d.AdPositionName == n,
                    p => new MediaPrice() { AdPositionName = c });
            _dbContext.SaveChanges();
            return Content("成功更换" + count + "条资源");
        }
        public ActionResult DeleteMedia()
        {
            var media = _media.Update(
                d => d.MediaTypeId == "X1904190915292236" && d.TransactorId == "X1801180851430024",
                d => new Media() {IsDelete = true});
            _dbContext.SaveChanges();
            return Content("成功删除" + media + "条资源");
        }
        public async Task<ActionResult> Http(string p)
        {
            CookieContainer cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri("http://56062118-yuyue.m.weimob.com"), new Cookie("express.session", "s%3A7etJNperJt_uLjupxp5LEbZRGukKh5id.wcrED5PA%2BRd4Ply59ERMDBPqdMJ7RoZUGWBbFfQTYb8"));
            //cookieContainer.Add(new Cookie("express.session", "s%3A7etJNperJt_uLjupxp5LEbZRGukKh5id.wcrED5PA%2BRd4Ply59ERMDBPqdMJ7RoZUGWBbFfQTYb8"));
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookieContainer;
            //handler.UseCookies = true;
            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri("http://56062118-yuyue.m.weimob.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsJsonAsync(
                    "api/micro-book-consumer/mobileController/mobileCalendar", new
                    {
                        aid = "56062118",
                        calendarDate = p,
                        sno = 19463
                    });
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<result>(result);
                return Content("请求结果：" + obj.code.errmsg + "，是否有预约：" + (obj.data.items.Any() ? "是" : "否"));
            }


            //var content = await HttpUtility.PostAsync("http://56062118-yuyue.m.weimob.com/api/micro-book-consumer/mobileController/mobileCalendar", postdata,cookieContainer);
            //return Content("");
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

    class result
    {
        public result()
        {
            data = new data();
        }
        public data data { get; set; }
        public code code { get; set; }
    }

    class code
    {
        public string errmsg { get; set; }
    }
    class data
    {
        public data()
        {
            items = new List<items>();
        }
        public List<items> items { get; set; }
    }

    class items
    {
        public string bookDivideTimes { get; set; }
    }

}