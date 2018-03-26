using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Log;
using Ada.Core.Domain.Purchase;
using Ada.Core.Domain.Resource;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Admin;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Caching;
using log4net;

namespace Ada.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly IRepository<BusinessOrderDetail> _temp;
        private readonly IRepository<Media> _media;
        private readonly IRepository<PurchaseOrderDetail> _ptemp;
        private readonly IDbContext _dbContext;

        public TestController(
            IDbContext dbContext,
            IRepository<BusinessOrderDetail> temp,
            IRepository<PurchaseOrderDetail> ptemp,
            IRepository<Media> media,
            IRepository<MediaArticle> mediaArticle,
            IRepository<MediaPrice> mediaPrice,
            IRepository<BusinessOrder> businessOrder)
        {
            _dbContext = dbContext;
            _ptemp = ptemp;
            _temp = temp;
            _media = media;
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
            List<string> temp=new List<string>();
            foreach (var businessOrderDetail in business)
            {
                var sell = businessOrderDetail.SellMoney;
                var verificationMoney = businessOrderDetail.VerificationMoney;
                var confirmMoney = businessOrderDetail.ConfirmVerificationMoney;
                var total = verificationMoney + confirmMoney;
                if (sell!= total)
                {
                    temp.Add("订单ID："+businessOrderDetail.Id+"，媒体名称："+businessOrderDetail.MediaName);
                }
            }

            return Content("金额不一致的订单：" + string.Join("&",temp) );


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

            var medias = _media.LoadEntities(d => d.IsDelete == false && d.MediaType.CallIndex == "weixin").ToList();
            //int start = medias.Count;
            //var last = medias.Distinct(new FastPropertyComparer<Media>("MediaID"));
            //var end = last.Count();
            int i = 0;
            foreach (var media in medias)
            {
                if (string.IsNullOrWhiteSpace(media.MediaID))
                {
                    media.IsDelete = true;
                    i++;
                }
            }
            _dbContext.SaveChanges();
            return Content("成功更新" + i + "个");
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

    }
}