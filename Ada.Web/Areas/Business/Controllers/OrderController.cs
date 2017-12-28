using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Finance;
using Ada.Core.Domain.Purchase;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Business;
using Ada.Services.Purchase;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace Business.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IBusinessOrderService _businessOrderService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IRepository<BusinessOrder> _repository;
        private readonly IRepository<BusinessOrderDetail> _businessOrderDetailRepository;
        private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseOrderDetailRepository;
        private readonly IRepository<MediaType> _mediaTypeRepository;
        private readonly IRepository<MediaPrice> _mediaPriceRepository;
        public OrderController(IBusinessOrderService businessOrderService,
            IRepository<BusinessOrder> repository,
            IRepository<MediaType> mediaTypeRepository,
            IRepository<BusinessOrderDetail> businessOrderDetailRepository,
            IPurchaseOrderService purchaseOrderService,
            IRepository<PurchaseOrder> purchaseOrderRepository,
            IRepository<Receivables> receivablesRepository,
            IBusinessPayeeService businessPayeeService,
            IRepository<PurchaseOrderDetail> purchaseOrderDetailRepository,
            IRepository<MediaPrice> mediaPriceRepository
        )
        {
            _businessOrderService = businessOrderService;
            _repository = repository;
            _mediaTypeRepository = mediaTypeRepository;
            _businessOrderDetailRepository = businessOrderDetailRepository;
            _purchaseOrderService = purchaseOrderService;
            _purchaseOrderRepository = purchaseOrderRepository;
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
            _mediaPriceRepository = mediaPriceRepository;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(BusinessOrderView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _businessOrderService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessOrderView
                {
                    Id = d.Id,
                    OrderNum = d.OrderNum,
                    LinkManName = d.LinkManName,
                    TotalMoney = d.TotalMoney,
                    Transactor = d.Transactor,
                    Status = d.Status ?? Consts.StateLock,
                    OrderDate = d.OrderDate,
                    TotalSellMoney = d.TotalSellMoney,
                    TotalTaxMoney = d.TotalTaxMoney,
                    DiscountMoney = d.TotalDiscountMoney,
                    AdderBy = d.AddedBy,
                    PurchaseSchedule = GetPurchaseSchedule(d),
                    OrderDetailCount = d.BusinessOrderDetails.Count,
                    OrderSchedule = d.BusinessOrderDetails.Count(o => o.Status == Consts.StateNormal) + "/" + d.BusinessOrderDetails.Count,
                    Remark = d.Remark
                })
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 采购进度
        /// </summary>
        /// <param name="businessOrder"></param>
        /// <returns></returns>
        private string GetPurchaseSchedule(BusinessOrder businessOrder)
        {
            var count = businessOrder.BusinessOrderDetails.Count;
            //获取采购订单明细完成情况
            var purchaseOrder = _purchaseOrderRepository.LoadEntities(d => d.BusinessOrderId == businessOrder.Id)
                .FirstOrDefault();
            int finish = 0;
            if (purchaseOrder != null)
            {
                finish = purchaseOrder.PurchaseOrderDetails.Count(d => d.IsDelete == false && d.Status == Consts.PurchaseStatusSuccess);
            }
            return finish + "/" + count;
        }
        public ActionResult Details(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            var details = item.BusinessOrderDetails.Select(d => new BusinessOrderDetailView()
            {
                Money = d.Money,
                PrePublishDate = d.PrePublishDate,
                MediaTitle = d.MediaTitle,
                MediaTypeName = d.MediaTypeName,
                MediaName = d.MediaName,
                MediaByPurchase = d.MediaByPurchase,
                AdPositionName = d.AdPositionName,
                PublishLink = GetPurchaseOrderDetail(d.Id)?.PublishLink,
                PublishDate = GetPurchaseOrderDetail(d.Id)?.PublishDate,
                PurchaseStatus = GetPurchaseOrderDetail(d.Id)?.Status,
            });
            return PartialView("OrderDetails", details);
        }

        private PurchaseOrderDetail GetPurchaseOrderDetail(string id)
        {
            return _purchaseOrderDetailRepository.LoadEntities(d => d.BusinessOrderDetailId == id).FirstOrDefault();
        }
        public ActionResult Add()
        {
            BusinessOrderView viewModel = new BusinessOrderView();
            viewModel.DiscountRate = 100;
            viewModel.Tax = 0;
            viewModel.DiscountMoney = 0;
            viewModel.OrderDate = DateTime.Now;
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(BusinessOrderView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var orderDetails = JsonConvert.DeserializeObject<List<BusinessOrderDetail>>(viewModel.OrderDetails);
            if (orderDetails.Count <= 0)
            {
                ModelState.AddModelError("message", "请录入订单明细！");
                return View(viewModel);
            }
            BusinessOrder entity = new BusinessOrder();
            entity.Id = IdBuilder.CreateIdNum();
            entity.OrderNum = IdBuilder.CreateOrderNum("XD");
            entity.Status = Consts.StateLock;//待处理
            entity.AuditStatus = Consts.StateLock;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedById = CurrentManager.Id;
            entity.AddedDate = DateTime.Now;
            //entity.BusinessType = viewModel.BusinessType;
            entity.Remark = viewModel.Remark;
            entity.LinkManId = viewModel.LinkManId;
            entity.LinkManName = viewModel.LinkManName;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            entity.Tax = viewModel.Tax;
            //entity.DiscountRate = viewModel.DiscountRate;
            //entity.SettlementType = viewModel.SettlementType;
            entity.OrderDate = viewModel.OrderDate;
            //entity.TotalDiscountMoney = viewModel.DiscountMoney;

            foreach (var businessOrderDetail in orderDetails)
            {
                businessOrderDetail.Id = IdBuilder.CreateIdNum();
                businessOrderDetail.Status = Consts.StateLock;//待转采购
                businessOrderDetail.VerificationStatus = Consts.StateLock;
                businessOrderDetail.VerificationMoney = businessOrderDetail.Money;
                businessOrderDetail.ConfirmVerificationMoney = 0;
                entity.BusinessOrderDetails.Add(businessOrderDetail);
            }
            entity.TotalTaxMoney = orderDetails.Sum(d => d.TaxMoney);
            entity.TotalMoney = orderDetails.Sum(d => d.Money);
            entity.TotalSellMoney = orderDetails.Sum(d => d.SellMoney);
            //entity.VerificationMoney = entity.TotalMoney;
            //entity.ConfirmVerificationMoney = 0;
            //entity.VerificationStatus = Consts.StateLock;
            _businessOrderService.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Update", new { id = entity.Id });
        }

        public ActionResult SelectMedia()
        {
            //媒体类型
            var mediaTypes = _mediaTypeRepository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis).ToList();
            return PartialView("SelectMedia", mediaTypes);
        }
        public ActionResult Update(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            BusinessOrderView entity = new BusinessOrderView();
            entity.Id = item.Id;
            entity.OrderNum = item.OrderNum;
            //entity.BusinessType = item.BusinessType;
            entity.LinkManId = item.LinkManId;
            entity.LinkManName = item.LinkManName;
            entity.Transactor = item.Transactor;
            entity.TransactorId = item.TransactorId;
            entity.Tax = item.Tax;
            //entity.DiscountRate = item.DiscountRate;
            //entity.SettlementType = item.SettlementType;
            //entity.DiscountMoney = item.TotalDiscountMoney;
            entity.OrderDate = item.OrderDate;
            entity.Remark = item.Remark;
            entity.Status = item.Status;
            var orderDetails = item.BusinessOrderDetails.Where(d => d.IsDelete == false).Select(d => new
            {
                d.Id,
                d.MediaTypeName,
                d.MediaName,
                d.AdPositionName,
                d.MediaTitle,
                PrePublishDate = d.PrePublishDate.IfNotNull(t => t.Value.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd")),
                d.SellMoney,
                d.Money,
                d.Tax,
                d.TaxMoney,
                d.MediaPriceId,
                d.Remark,
                d.CostMoney,
                d.MediaByPurchase,
                Status = d.Status ?? Consts.StateLock
            });
            entity.OrderDetails = JsonConvert.SerializeObject(orderDetails);

            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(BusinessOrderView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }

            var orderDetails = JsonConvert.DeserializeObject<List<BusinessOrderDetail>>(viewModel.OrderDetails);
            if (orderDetails.Count <= 0)
            {
                ModelState.AddModelError("message", "请录入订单明细！");
                return View(viewModel);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            if (entity.VerificationStatus == Consts.StateNormal)
            {
                ModelState.AddModelError("message", "该订单已核销，无效修改！");
                return View(viewModel);
            }
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            //entity.BusinessType = viewModel.BusinessType;
            entity.LinkManId = viewModel.LinkManId;
            entity.LinkManName = viewModel.LinkManName;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            entity.Tax = viewModel.Tax;
            //entity.DiscountRate = viewModel.DiscountRate;
            //entity.SettlementType = viewModel.SettlementType;
            entity.OrderDate = viewModel.OrderDate;
            //entity.TotalDiscountMoney = viewModel.DiscountMoney;
            entity.Remark = viewModel.Remark;
            //删除已剔除的
            var ids = orderDetails.Select(d => d.Id).ToList();
            foreach (var entityBusinessOrderDetail in entity.BusinessOrderDetails)
            {
                if (!ids.Contains(entityBusinessOrderDetail.Id))
                {
                    entityBusinessOrderDetail.IsDelete = true;
                }
            }
            //更新，增加
            foreach (var businessOrderDetail in orderDetails)
            {
                if (string.IsNullOrWhiteSpace(businessOrderDetail.Id))
                {
                    businessOrderDetail.Id = IdBuilder.CreateIdNum();
                    businessOrderDetail.Status = Consts.StateLock;
                    businessOrderDetail.VerificationStatus = Consts.StateLock;
                    businessOrderDetail.VerificationMoney = businessOrderDetail.Money;
                    businessOrderDetail.ConfirmVerificationMoney = 0;
                    entity.BusinessOrderDetails.Add(businessOrderDetail);
                }
                else
                {
                    //未核销的更新
                    var detail = _businessOrderDetailRepository.LoadEntities(d => d.Id == businessOrderDetail.Id)
                        .FirstOrDefault();
                    if (detail.VerificationStatus!=Consts.StateNormal)
                    {
                        detail.MediaPriceId = businessOrderDetail.MediaPriceId;
                        detail.MediaTitle = businessOrderDetail.MediaTitle;
                        detail.PrePublishDate = businessOrderDetail.PrePublishDate;
                        detail.Money = businessOrderDetail.Money;
                        detail.SellMoney = businessOrderDetail.SellMoney;
                        detail.Tax = businessOrderDetail.Tax;
                        detail.TaxMoney = businessOrderDetail.TaxMoney;
                        detail.AdPositionName = businessOrderDetail.AdPositionName;
                        detail.MediaName = businessOrderDetail.MediaName;
                        detail.MediaTypeName = businessOrderDetail.MediaTypeName;
                        detail.Remark = businessOrderDetail.Remark;
                        detail.CostMoney = businessOrderDetail.CostMoney;
                        detail.MediaByPurchase = businessOrderDetail.MediaByPurchase;
                        detail.VerificationMoney = businessOrderDetail.Money;
                        detail.ConfirmVerificationMoney = 0;
                    }
                    

                }
            }
            entity.TotalTaxMoney = orderDetails.Sum(d => d.TaxMoney);
            entity.TotalMoney = orderDetails.Sum(d => d.Money);
            entity.TotalSellMoney = orderDetails.Sum(d => d.SellMoney);
            //entity.VerificationMoney = entity.TotalMoney;
            //entity.ConfirmVerificationMoney = 0;
            _businessOrderService.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Update", new { id = entity.Id });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.Status == Consts.StateLock)
            {
                entity.DeletedBy = CurrentManager.UserName;
                entity.DeletedById = CurrentManager.Id;
                entity.DeletedDate = DateTime.Now;
                _businessOrderService.Delete(entity);
                return Json(new { State = 1, Msg = "删除成功" });
            }
            return Json(new { State = 0, Msg = "此订单状态无法删除" });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult TransformPurchaseOrder(string id)
        {
            var details = Request["rows"];
            if (string.IsNullOrWhiteSpace(details))
            {
                return Json(new { State = 0, Msg = "请先选择要转换的订单." });
            }
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.VerificationStatus == Consts.StateNormal)
            {
                return Json(new { State = 0, Msg = "该项目订单已核销，无法转换" });
            }
            entity.Status = Consts.StateNormal;//已下单
            var purchaseOrder = _purchaseOrderRepository.LoadEntities(d => d.BusinessOrderId == id && d.IsDelete == false).FirstOrDefault();
            bool isAdd = false;
            if (purchaseOrder == null)
            {
                purchaseOrder = new PurchaseOrder();
                purchaseOrder.Id = IdBuilder.CreateIdNum();
                purchaseOrder.BusinessOrderId = entity.Id;
                purchaseOrder.BusinessBy = entity.Transactor;
                purchaseOrder.BusinessById = entity.TransactorId;
                purchaseOrder.OrderDate = DateTime.Now;
                purchaseOrder.TotalMoney = 0;
                purchaseOrder.OrderNum = IdBuilder.CreateOrderNum("CD");
                purchaseOrder.Status = Consts.StateLock;
                isAdd = true;
            }
            var orderDetails = JsonConvert.DeserializeObject<List<BusinessOrderDetail>>(details);
            foreach (var order in orderDetails)
            {
                if (string.IsNullOrWhiteSpace(order.Id))
                {
                    order.Id = IdBuilder.CreateIdNum();
                    order.Status = Consts.StateNormal;
                    order.VerificationStatus = Consts.StateLock;
                    order.VerificationMoney = order.Money;
                    order.ConfirmVerificationMoney = 0;
                    entity.BusinessOrderDetails.Add(order);
                    entity.TotalTaxMoney = entity.TotalTaxMoney + order.TaxMoney;
                    entity.TotalMoney = entity.TotalMoney + order.Money;
                    entity.TotalSellMoney = entity.TotalSellMoney + order.SellMoney;
                    //entity.VerificationMoney = entity.TotalMoney;
                }
                else
                {
                    var temp = _businessOrderDetailRepository.LoadEntities(d => d.Id == order.Id).FirstOrDefault();
                    if (temp.Status == Consts.StateNormal)
                    {
                        continue;
                    }
                    temp.Status = Consts.StateNormal;//已转采购
                }
                PurchaseOrderDetail purchaseOrderDetail = new PurchaseOrderDetail();
                purchaseOrderDetail.Id = IdBuilder.CreateIdNum();
                purchaseOrderDetail.BusinessOrderDetailId = order.Id;
                purchaseOrderDetail.CostMoney = order.CostMoney;
                purchaseOrderDetail.VerificationStatus = Consts.StateLock;
                purchaseOrderDetail.AdPositionName = order.AdPositionName;
                purchaseOrderDetail.MediaTitle = order.MediaTitle;
                purchaseOrderDetail.MediaName = order.MediaName;
                purchaseOrderDetail.MediaTypeName = order.MediaTypeName;
                purchaseOrderDetail.MediaPriceId = order.MediaPriceId;
                purchaseOrderDetail.AuditStatus = Consts.StateLock;
                purchaseOrderDetail.Status = Consts.PurchaseStatusWait;
                var price = _mediaPriceRepository.LoadEntities(d => d.Id == order.MediaPriceId).FirstOrDefault();
                purchaseOrderDetail.Transactor = price.Media.Transactor;
                purchaseOrderDetail.TransactorId = price.Media.TransactorId;
                purchaseOrderDetail.LinkMan = price.Media.LinkMan;
                purchaseOrderDetail.LinkManName = price.Media.LinkMan.Name;
                purchaseOrderDetail.Tax = 0;
                purchaseOrderDetail.TaxMoney = 0;
                purchaseOrderDetail.BargainMoney = 0;
                purchaseOrderDetail.Money = 0;
                purchaseOrderDetail.ConfirmVerificationMoney = 0;
                purchaseOrderDetail.VerificationMoney = 0;
                purchaseOrderDetail.DiscountMoney = 0;
                purchaseOrderDetail.PurchaseMoney = 0;
                purchaseOrderDetail.DiscountRate = 100;
                purchaseOrder.PurchaseOrderDetails.Add(purchaseOrderDetail);
            }

            if (isAdd)
            {
                _purchaseOrderService.Add(purchaseOrder);
            }
            else
            {
                _purchaseOrderService.Update(purchaseOrder);
            }

            return Json(new { State = 1, Msg = "转换成功" });
        }

    }
}