using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Purchase;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Statistics;
using Ada.Framework.Filter;
using Newtonsoft.Json;

namespace Dashboards.Controllers
{
    public class PurchaseController : BaseController
    {
        private readonly IRepository<PurchaseOrderDetail> _purchaseRepository;
        private readonly IRepository<PurchasePayment> _purchasePaymentRepository;
        private readonly IRepository<MediaType> _mediaTypeRepository;
        public PurchaseController(IRepository<PurchaseOrderDetail> purchaseRepository,
            IRepository<PurchasePayment> purchasePaymentRepository,
            IRepository<MediaType> mediaTypeRepository)
        {
            _purchaseRepository = purchaseRepository;
            _purchasePaymentRepository = purchasePaymentRepository;
            _mediaTypeRepository = mediaTypeRepository;
        }
        public ActionResult Index()
        {
            var premission = PremissionData();
            var userId = string.Empty;
            if (premission.Count > 0)
            {
                userId = CurrentManager.Id;
            }
            var purchase = _purchaseRepository.LoadEntities(d => d.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(userId))
            {
                purchase = purchase.Where(d => d.TransactorId == userId);
            }
            //订单数
            PurchaseTotal total = new PurchaseTotal();
            total.Waiting = purchase.Count(d => d.Status == Consts.PurchaseStatusWait);
            total.Confirm = purchase.Count(d => d.Status == Consts.PurchaseStatusConfirm);
            total.Doing = purchase.Count(d => d.Status == Consts.PurchaseStatusTodo);
            //var start = DateTime.Now.Date;
            //var end = start.AddDays(1);
            //total.Today = purchase.Count(d => d.PublishDate >= start && d.PublishDate < end);
            //var start1 = DateTime.Now.Date.AddDays(1);
            //var end1 = start1.AddDays(1);
            //total.Tomorrow = purchase.Count(d => d.PublishDate >= start1 && d.PublishDate < end1);
            //已付款
            total.TotalPayMoney = purchase.Where(d=>d.Status!=Consts.PurchaseStatusFail).Sum(d => d.PurchaseMoney);
            total.PayMoney = _purchasePaymentRepository.LoadEntities(d => d.IsDelete == false).Sum(d =>
                  d.PurchasePaymentDetails.Where(t => t.Status == Consts.StateNormal).Sum(p => p.PayMoney));
            //发票数
            var invoice = _purchasePaymentRepository.LoadEntities(d => d.IsInvoice == true && d.InvoiceStauts == false);
            if (!string.IsNullOrWhiteSpace(userId))
            {
                invoice = invoice.Where(d => d.TransactorId == userId);
            }
            total.InvoiceCount = invoice.Count();
            //资源数
            var types = _mediaTypeRepository.LoadEntities(d => d.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var counts = types.Select(d => new
                {
                    value = d.Medias.Count(m => m.IsDelete == false && m.TransactorId == userId),
                    name = d.TypeName
                });
                total.Medias = JsonConvert.SerializeObject(counts);
            }
            else
            {
                var counts = types.Select(d => new
                {
                    value = d.Medias.Count(m => m.IsDelete == false),
                    name = d.TypeName
                });
                total.Medias = JsonConvert.SerializeObject(counts);
            }

            return View(total);
        }
    }
}