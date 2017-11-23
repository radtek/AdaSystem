using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Purchase;
using Ada.Framework.Filter;
using Ada.Services.Purchase;

namespace Purchase.Controllers
{
    public class OrderDetailController : BaseController
    {
        private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;
        private readonly IRepository<BusinessOrderDetail> _businessOrderDetailRepository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseOrderDetailRepository;
        public OrderDetailController(IPurchaseOrderDetailService purchaseOrderDetailService,
            IRepository<BusinessOrderDetail> businessOrderDetailRepository,
            IRepository<PurchaseOrderDetail> purchaseOrderDetailRepository)
        {
            _purchaseOrderDetailService = purchaseOrderDetailService;
            _businessOrderDetailRepository = businessOrderDetailRepository;
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(PurchaseOrderDetailView viewModel)
        {
            var result = _purchaseOrderDetailService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new PurchaseOrderDetailView
                {
                    Id = d.Id,
                    Status = d.Status,
                    LinkManName = d.LinkManName,
                    PurchaseOrderNum = d.PurchaseOrder.OrderNum,
                    BusinessBy = d.PurchaseOrder.BusinessBy,
                    MediaTypeName = d.MediaTypeName,
                    MediaName = d.MediaName,
                    AdPositionName = d.AdPositionName,
                    CostMoney = d.CostMoney,
                    PurchaseMoney = d.PurchaseMoney,
                    Tax = d.Tax,
                    TaxMoney = d.TaxMoney,
                    DiscountMoney = d.DiscountMoney,
                    Money = d.Money,
                    MediaTitle = GetBusinessOrderDetail(d.BusinessOrderDetailId).MediaTitle,
                    PrePublishDate = GetBusinessOrderDetail(d.BusinessOrderDetailId).PrePublishDate,
                    PublishDate = d.PublishDate,
                    PublishLink = d.PublishLink,
                    Transactor = d.Transactor,
                    OrderDate = d.PurchaseOrder.OrderDate


                })
            }, JsonRequestBehavior.AllowGet);
        }

        private BusinessOrderDetail GetBusinessOrderDetail(string businessId)
        {
            var business = _businessOrderDetailRepository.LoadEntities(d => d.Id == businessId).FirstOrDefault();
            return business;
        }

        public ActionResult Update(string id)
        {
            var entity = _purchaseOrderDetailRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            PurchaseOrderDetailView item=new PurchaseOrderDetailView();
            item.Id = entity.Id;
            item.LinkManId = entity.LinkManId;
            item.LinkManName = entity.LinkManName;
            item.AdPositionName = entity.AdPositionName;
            item.MediaName = entity.MediaName;
            item.MediaTypeName = entity.MediaTypeName;
            item.CostMoney = entity.CostMoney;
            item.PublishDate = entity.PublishDate;
            item.PublishLink = entity.PublishLink;
            item.Tax = entity.Tax;
            item.TaxMoney = entity.TaxMoney;
            item.PurchaseMoney = entity.PurchaseMoney;
            item.Money = entity.Money;
            item.DiscountMoney = entity.DiscountMoney;
            item.Transactor = entity.Transactor;
            item.TransactorId = entity.TransactorId;
            item.Status = entity.Status;
            item.BargainMoney = entity.BargainMoney;
            //item.OrderDate = entity.PurchaseOrder.OrderDate;
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(PurchaseOrderDetailView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var entity = _purchaseOrderDetailRepository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.PurchaseType = viewModel.PurchaseType;
            entity.SettlementType = viewModel.SettlementType;
            entity.LinkManId = viewModel.LinkManId;
            entity.LinkManName = viewModel.LinkManName;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            entity.PublishDate = viewModel.PublishDate;
            entity.PublishLink = viewModel.PublishLink;

            entity.Tax = viewModel.Tax;
            entity.TaxMoney = viewModel.TaxMoney;
            entity.PurchaseMoney = viewModel.PurchaseMoney;
            entity.Money = viewModel.Money;
            entity.DiscountMoney = viewModel.DiscountMoney;
            entity.BargainMoney = viewModel.BargainMoney;

           
            entity.Status = viewModel.Status;
            entity.Remark = viewModel.Remark;
            
            _purchaseOrderDetailService.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _purchaseOrderDetailRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _purchaseOrderDetailService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}