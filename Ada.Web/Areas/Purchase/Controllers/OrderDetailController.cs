using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
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
            viewModel.Managers = PremissionData();
            var result = _purchaseOrderDetailService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new PurchaseOrderDetailView
                {
                    Id = d.Id,
                    Status = d.Status,
                    LinkManName = d.LinkManName,
                    LinkManId = d.LinkManId,
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
                    OrderDate = d.PurchaseOrder.OrderDate,
                    
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
            PurchaseOrderDetailView item = new PurchaseOrderDetailView();
            item.Id = entity.Id;
            item.LinkManId = entity.LinkManId;
            item.LinkManName = entity.LinkManName;
            item.AdPositionName = entity.AdPositionName;
            item.MediaName = entity.MediaName;
            item.MediaTypeName = entity.MediaTypeName;
            item.CostMoney = entity.CostMoney;
            item.PublishDate = entity.PublishDate;
            item.PublishLink = entity.PublishLink;
            item.Remark = entity.Remark;
            //item.Tax = entity.Tax;
            //item.TaxMoney = entity.TaxMoney;
            item.PurchaseMoney = entity.PurchaseMoney;
            //item.Money = entity.Money;
            //item.DiscountRate = entity.DiscountRate;
            //item.DiscountMoney = entity.DiscountMoney;
            item.Transactor = entity.Transactor;
            item.TransactorId = entity.TransactorId;
            item.BusinessBy = entity.PurchaseOrder.BusinessBy;
            var businessOrder = GetBusinessOrderDetail(entity.BusinessOrderDetailId);
            item.PrePublishDate = businessOrder.PrePublishDate;
            item.MediaTitle = businessOrder.MediaTitle;
            item.BusinessRemark = businessOrder.Remark;
            item.AuditStatus = entity.AuditStatus;
            item.Status = entity.Status;
            item.IsPayment = entity.PurchasePaymentOrderDetails.Count > 0;
            //item.BargainMoney = entity.BargainMoney;
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
            //if (viewModel.PurchaseMoney > entity.CostMoney)
            //{
            //    ModelState.AddModelError("message", "不能低于成本金额处理");
            //    return View(viewModel);
            //}
            ////已审核 已完成 无税金额不能低于上次填写的金额
            //if (viewModel.PurchaseMoney >= entity.PurchaseMoney && entity.Status == Consts.PurchaseStatusSuccess)
            //{
            //    ModelState.AddModelError("message", "不能低于上次的无税金额：" + entity.PurchaseMoney);
            //    return View(viewModel);
            //}
            //var businessOrderDetail = GetBusinessOrderDetail(entity.BusinessOrderDetailId);
            //if (businessOrderDetail.BusinessOrder.VerificationStatus==Consts.StateNormal)
            //{
            //    ModelState.AddModelError("message", "此销售订单已核销，无需再进行处理");
            //    return View(viewModel);
            //}
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
            //entity.DiscountRate = viewModel.DiscountRate;
            //entity.Tax = viewModel.Tax;
            //entity.TaxMoney = viewModel.TaxMoney;
            entity.PurchaseMoney = viewModel.PurchaseMoney;
            var tax = entity.Tax ?? 0;
            entity.Money = entity.PurchaseMoney * (1 + tax / 100);
            entity.Status = viewModel.Status;
            if (entity.Status==Consts.PurchaseStatusSuccess)
            {
                if (entity.PublishDate==null)
                {
                    ModelState.AddModelError("message", "出刊日期不能为空");
                    return View(viewModel);
                }

                if (entity.PurchaseMoney<=0||entity.PurchaseMoney==null)
                {
                    ModelState.AddModelError("message", "无税金额不能为0元");
                    return View(viewModel);
                }
                entity.AuditStatus = Consts.StateNormal;
            }
            //entity.Money = viewModel.Money;
            //entity.DiscountMoney = viewModel.DiscountMoney;
            //entity.BargainMoney = viewModel.BargainMoney;
            //if (entity.CostMoney != viewModel.CostMoney)
            //{
            //    //更新成本金额
            //    businessOrderDetail.CostMoney = viewModel.CostMoney;
            //}
            //entity.CostMoney = viewModel.CostMoney;

            
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
            var businessOrder = GetBusinessOrderDetail(entity.BusinessOrderDetailId);
            //是否已经核销
            if (businessOrder.VerificationStatus == Consts.StateNormal)
            {
                return Json(new { State = 0, Msg = "此销售订单已核销，无法删除" });
            }
            //是否已经付款
            if (entity.PurchasePaymentOrderDetails.Count > 0)
            {
                return Json(new { State = 0, Msg = "此采购订单已请款，无法删除" });
            }
            //entity.DeletedBy = CurrentManager.UserName;
            //entity.DeletedById = CurrentManager.Id;
            //entity.DeletedDate = DateTime.Now;
            //businessOrder.BusinessOrder.TotalMoney = businessOrder.BusinessOrder.TotalMoney - businessOrder.Money;
            //businessOrder.BusinessOrder.TotalSellMoney =
            //    businessOrder.BusinessOrder.TotalSellMoney - businessOrder.SellMoney;
            //businessOrder.BusinessOrder.TotalTaxMoney =
            //    businessOrder.BusinessOrder.TotalTaxMoney - businessOrder.TaxMoney;
            //businessOrder.BusinessOrder.VerificationMoney = businessOrder.BusinessOrder.TotalMoney;
            _businessOrderDetailRepository.Remove(businessOrder);
            _purchaseOrderDetailService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Audit(string id)
        {
            var entity = _purchaseOrderDetailRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.AuditStatus == null || entity.AuditStatus == Consts.StateLock)
            {
                entity.AuditStatus = Consts.StateNormal;
            }
            else
            {
                entity.AuditStatus = Consts.StateLock;
            }
            _purchaseOrderDetailService.Update(entity);
            TempData["Msg"] = "操作成功";
            return RedirectToAction("Update", new { id });
        }
    }
}