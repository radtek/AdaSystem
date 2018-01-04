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
        private readonly IBusinessOrderDetailService _businessOrderDetailService;
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
            IRepository<MediaPrice> mediaPriceRepository,
            IBusinessOrderDetailService businessOrderDetailService
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
            _businessOrderDetailService = businessOrderDetailService;
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
                    TotalMoney = d.BusinessOrderDetails.Sum(o=>o.Money),
                    Transactor = d.Transactor,
                    Status = d.Status ?? Consts.StateLock,
                    OrderDate = d.OrderDate,
                    TotalSellMoney = d.BusinessOrderDetails.Sum(o => o.SellMoney),
                    TotalTaxMoney = d.BusinessOrderDetails.Sum(o => o.TaxMoney),
                    //DiscountMoney = d.TotalDiscountMoney,
                    //AdderBy = d.AddedBy,
                    PurchaseSchedule = GetPurchaseSchedule(d),
                    OrderDetailCount = d.BusinessOrderDetails.Count,
                    OrderSchedule = d.BusinessOrderDetails.Count(o => o.Status == Consts.StateNormal) + "/" + d.BusinessOrderDetails.Count,
                    Remark = d.Remark
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDetails(BusinessOrderDetailView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _businessOrderDetailService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessOrderDetailView
                {
                    Id = d.Id,
                    VerificationMoney = d.VerificationMoney,
                    MediaName = d.MediaName,
                    MediaTypeName = d.MediaTypeName,
                    AdPositionName = d.AdPositionName,
                    Money = d.Money

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
                CostMoney = GetPurchaseOrderDetail(d.Id).PurchaseMoney
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
            viewModel.AuditStatus = Consts.StateLock;
            viewModel.Status = Consts.StateLock;
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
                businessOrderDetail.AuditStatus = Consts.StateLock;
                entity.BusinessOrderDetails.Add(businessOrderDetail);
            }
            //entity.TotalTaxMoney = orderDetails.Sum(d => d.TaxMoney);
            //entity.TotalMoney = orderDetails.Sum(d => d.Money);
            //entity.TotalSellMoney = orderDetails.Sum(d => d.SellMoney);
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
            entity.AuditStatus = item.AuditStatus;
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
            //if (orderDetails.Count <= 0)
            //{
            //    ModelState.AddModelError("message", "请录入订单明细！");
            //    return View(viewModel);
            //}
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
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
            //找到已删除的
            var ids = orderDetails.Select(d => d.Id).ToList();
            List<string> deleteIds = new List<string>();
            foreach (var entityBusinessOrderDetail in entity.BusinessOrderDetails)
            {
                if (!ids.Contains(entityBusinessOrderDetail.Id))
                {
                    deleteIds.Add(entityBusinessOrderDetail.Id);
                }
            }
            //更新，增加
            decimal? costMoney = 0;
            decimal? sellMoney = 0;
            foreach (var businessOrderDetail in orderDetails)
            {
                //新增
                if (string.IsNullOrWhiteSpace(businessOrderDetail.Id))
                {
                    businessOrderDetail.Id = IdBuilder.CreateIdNum();
                    businessOrderDetail.Status = Consts.StateLock;
                    businessOrderDetail.AuditStatus = Consts.StateLock;
                    businessOrderDetail.VerificationStatus = Consts.StateLock;
                    businessOrderDetail.VerificationMoney = businessOrderDetail.Money;
                    businessOrderDetail.ConfirmVerificationMoney = 0;
                    entity.BusinessOrderDetails.Add(businessOrderDetail);
                }
                else
                {
                    //未核销的，未审核的更新
                    var detail = _businessOrderDetailRepository.LoadEntities(d => d.Id == businessOrderDetail.Id)
                        .FirstOrDefault();
                    if (detail.VerificationStatus != Consts.StateNormal && detail.AuditStatus == Consts.StateLock)
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
                    //可以修改稿件标题
                    detail.MediaTitle = businessOrderDetail.MediaTitle;
                    //已下单的金额校验
                    if (detail.Status == Consts.StateNormal)
                    {
                        costMoney += detail.CostMoney;
                        sellMoney += detail.SellMoney;
                    }
                }
            }

            if (sellMoney <= costMoney && sellMoney > 0 && costMoney > 0)
            {
                ModelState.AddModelError("message", "不能低于成本金额！");
                return View(viewModel);
            }
            //删除
            foreach (var deleteId in deleteIds)
            {
                var temp = _businessOrderDetailRepository.LoadEntities(d => d.Id == deleteId)
                    .FirstOrDefault();
                _businessOrderDetailRepository.Remove(temp);
            }
            //entity.TotalTaxMoney = orderDetails.Sum(d => d.TaxMoney);
            //entity.TotalMoney = orderDetails.Sum(d => d.Money);
            //entity.TotalSellMoney = orderDetails.Sum(d => d.SellMoney);
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
            if (entity.BusinessOrderDetails.Count==0)
            {
                entity.DeletedBy = CurrentManager.UserName;
                entity.DeletedById = CurrentManager.Id;
                entity.DeletedDate = DateTime.Now;
                _businessOrderService.Delete(entity);
                return Json(new { State = 1, Msg = "删除成功" });
            }
            return Json(new { State = 0, Msg = "此订单状态无法删除" });
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Audit(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.AuditStatus == null || entity.AuditStatus == Consts.StateLock)
            {
                entity.AuditStatus = Consts.StateNormal;

            }
            else
            {
                entity.AuditStatus = Consts.StateLock;
            }
            //已下单的都审核
            foreach (var entityBusinessOrderDetail in entity.BusinessOrderDetails.Where(d => d.Status == Consts.StateNormal))
            {
                entityBusinessOrderDetail.AuditStatus = entity.AuditStatus;
            }
            _businessOrderService.Update(entity);
            TempData["Msg"] = "操作成功";
            return RedirectToAction("Update", new { id });
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
            entity.Status = Consts.StateNormal;//已下单
            entity.AuditStatus = Consts.StateNormal;//审核通过
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
            var detailsTemp = entity.BusinessOrderDetails.Where(d => d.Status == Consts.StateNormal).ToList();
            var costMoney = detailsTemp.Sum(d => d.CostMoney);
            var sellMoney = detailsTemp.Sum(d => d.SellMoney);
            foreach (var order in orderDetails)
            {
                if (string.IsNullOrWhiteSpace(order.Id))
                {
                    order.Id = IdBuilder.CreateIdNum();
                    order.Status = Consts.StateNormal;
                    order.VerificationStatus = Consts.StateLock;
                    order.AuditStatus = Consts.StateNormal;
                    order.VerificationMoney = order.Money;
                    order.ConfirmVerificationMoney = 0;
                    entity.BusinessOrderDetails.Add(order);
                    //entity.TotalTaxMoney = entity.TotalTaxMoney + order.TaxMoney;
                    //entity.TotalMoney = entity.TotalMoney + order.Money;
                    //entity.TotalSellMoney = entity.TotalSellMoney + order.SellMoney;
                    //entity.VerificationMoney = entity.TotalMoney;
                    costMoney += order.CostMoney;
                    sellMoney += order.SellMoney;
                }
                else
                {
                    var temp = _businessOrderDetailRepository.LoadEntities(d => d.Id == order.Id).FirstOrDefault();
                    if (temp.Status == Consts.StateNormal)
                    {
                        continue;
                    }
                    temp.MediaTitle = order.MediaTitle;
                    temp.PrePublishDate = order.PrePublishDate;
                    temp.Money = order.Money;
                    temp.SellMoney = order.SellMoney;
                    temp.Tax = order.Tax;
                    temp.TaxMoney = order.TaxMoney;
                    temp.Remark = order.Remark;
                    temp.VerificationMoney = order.Money;
                    temp.ConfirmVerificationMoney = 0;
                    temp.Status = Consts.StateNormal;//已转采购
                    temp.AuditStatus = Consts.StateNormal;//审核通过
                    costMoney += temp.CostMoney;
                    sellMoney += temp.SellMoney;
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
                purchaseOrderDetail.AuditStatus = Consts.StateNormal;
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
            //校验金额
            if (sellMoney <= costMoney)
            {
                return Json(new { State = 0, Msg = "不能低于成本金额" });
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