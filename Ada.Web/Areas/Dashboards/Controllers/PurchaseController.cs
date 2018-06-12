using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Customer;
using Ada.Core.Domain.Purchase;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Statistics;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Newtonsoft.Json;

namespace Dashboards.Controllers
{
    public class PurchaseController : BaseController
    {
        private readonly IRepository<PurchaseOrderDetail> _purchaseRepository;
        private readonly IRepository<PurchasePayment> _purchasePaymentRepository;
        private readonly IRepository<MediaType> _mediaTypeRepository;
        private readonly IRepository<Media> _mediaRepository;
        private readonly IRepository<Commpany> _commpanyRepository;

        public PurchaseController(IRepository<PurchaseOrderDetail> purchaseRepository,
            IRepository<PurchasePayment> purchasePaymentRepository,
            IRepository<MediaType> mediaTypeRepository,
            IRepository<Media> mediaRepository,
            IRepository<Commpany> commpanyRepository)
        {
            _purchaseRepository = purchaseRepository;
            _purchasePaymentRepository = purchasePaymentRepository;
            _mediaTypeRepository = mediaTypeRepository;
            _mediaRepository = mediaRepository;
            _commpanyRepository = commpanyRepository;
        }
        public ActionResult Index()
        {
            var premission = PremissionData();
            
            var purchase = _purchaseRepository.LoadEntities(d => d.IsDelete == false);
            
            if (premission != null && premission.Count > 0)
            {
                purchase = purchase.Where(d => premission.Contains(d.TransactorId));
            }
            //订单数
            PurchaseTotal total = new PurchaseTotal();
            total.OrderCount = purchase.Count();
            total.Waiting = purchase.Count(d => d.Status == Consts.PurchaseStatusWait);
            total.Confirm = purchase.Count(d => d.Status == Consts.PurchaseStatusConfirm);
            total.Doing = purchase.Count(d => d.Status == Consts.PurchaseStatusTodo);
            var start = DateTime.Now.Date;
            var end = start.AddDays(1);
            total.Today = purchase.Count(d => d.PublishDate >= start && d.PublishDate < end);
            var start1 = DateTime.Now.Date.AddDays(1);
            var end1 = start1.AddDays(1);
            total.Tomorrow = purchase.Count(d => d.PublishDate >= start1 && d.PublishDate < end1);
            //已付款
            total.TotalPayMoney = purchase.Where(d => d.Status != Consts.PurchaseStatusFail).Sum(d => d.PurchaseMoney);
            var payment = _purchasePaymentRepository.LoadEntities(d => d.IsDelete == false);
           
            if (premission != null && premission.Count > 0)
            {
                payment = payment.Where(d => premission.Contains(d.TransactorId));
            }
            total.PayMoney = payment.Sum(d =>
                  d.PurchasePaymentDetails.Where(t => t.Status == Consts.StateNormal).Sum(p => p.PayMoney));
            //发票数
            var invoice = _purchasePaymentRepository.LoadEntities(d => d.IsInvoice == true && d.InvoiceStauts == false);
            
            if (premission != null && premission.Count > 0)
            {
                invoice = invoice.Where(d => premission.Contains(d.TransactorId));
            }
            total.InvoiceCount = invoice.Count();
            //资源数
            var types = _mediaTypeRepository.LoadEntities(d => d.IsDelete == false);
            if (premission != null && premission.Count > 0)
            {
                var counts = types.Select(d => new
                {
                    value = d.Medias.Count(m => m.IsDelete == false && premission.Contains(m.TransactorId)),
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

            //资源开发数排行
            //本月
            //var startTop = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //var endTop = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).AddDays(1);
            //var medias = _mediaRepository.LoadEntities(d =>
            //     d.IsDelete == false && d.MediaType.CallIndex != "website" && d.MediaType.CallIndex != "brush" && d.AddedDate >= startTop && d.AddedDate < endTop);
            //var managers = _managerService.GetByOrganizationName("媒介部").ToList();
            //List<MediaAddTop> top=new List<MediaAddTop>();
            //foreach (var managerView in managers)
            //{
            //    MediaAddTop item=new MediaAddTop();
            //    item.Transactor = managerView.UserName;
            //    item.MediasCount = medias.Count(d => d.TransactorId == managerView.Id);
            //    top.Add(item);
            //}
            //供应商排名
            var userId = string.Empty;
            if (premission != null && premission.Count > 0)
            {
                userId = CurrentManager.Id;
            }
            var companys =
                _commpanyRepository.LoadEntities(d => d.IsDelete == false && d.IsBusiness);
            if (!string.IsNullOrWhiteSpace(userId))
            {
                companys = companys.Where(d => d.TransactorId == userId);
            }
            total.CompanyTops = companys.Select(d => new CompanyTop()
            {
                Name = d.Name,
                TotalMoney = d.LinkMans.Sum(l => l.PurchaseOrderDetails.Where(o=>o.Status!=Consts.PurchaseStatusFail).Sum(p => p.PurchaseMoney))
            }).OrderByDescending(d => d.TotalMoney).Take(20).ToList();
            total.Tops= AddTopData(DateTime.Now.Year,DateTime.Now.Month);
            return View(total);
        }

        public ActionResult GetAddTop(string date)
        {
            var dateTemp = date.Split('-');
            var year = Convert.ToInt32(dateTemp[0]);
            var month = Convert.ToInt32(dateTemp[1]);
            return Json(AddTopData(year,month), JsonRequestBehavior.AllowGet);
        }

        private List<MediaAddTop> AddTopData(int year,int month)
        {
            var startTop = new DateTime(year, month, 1);
            var endTop = new DateTime(year, month, DateTime.DaysInMonth(year, month)).AddDays(1);
            var medias = _mediaRepository.LoadEntities(d =>
                d.IsDelete == false && d.MediaType.CallIndex != "website" && d.MediaType.CallIndex != "brush" && d.AddedDate >= startTop && d.AddedDate < endTop);
            var result = medias.GroupBy(d => d.Transactor).Select(d => new MediaAddTop()
            {
                Transactor = d.Key,
                MediasCount = d.Count()
            }).OrderByDescending(d => d.MediasCount).ToList();
            return result;
        }
    }
}