using System;
using System.Collections.Generic;
using System.IO;
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
using Ada.Framework.UploadFile;
using Ada.Services.Business;
using Ada.Services.Purchase;
using DocumentFormat.OpenXml.Office.Word;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

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
        private readonly IOrderDetailCommentService _orderDetailCommentService;
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
            IBusinessOrderDetailService businessOrderDetailService,
            IOrderDetailCommentService orderDetailCommentService
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
            _orderDetailCommentService = orderDetailCommentService;
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
                    LinkManId = d.LinkManId,
                    TotalMoney = d.BusinessOrderDetails.Sum(o => o.Money),
                    Transactor = d.Transactor,
                    Tax = d.Tax,
                    //Status = d.Status ?? Consts.StateLock,
                    OrderDate = d.OrderDate,
                    TotalSellMoney = d.BusinessOrderDetails.Sum(o => o.SellMoney),
                    TotalTaxMoney = d.BusinessOrderDetails.Sum(o => o.TaxMoney),
                    //DiscountMoney = d.TotalDiscountMoney,
                    //AdderBy = d.AddedBy,
                    //PurchaseSchedule = GetPurchaseSchedule(d),
                    OrderDetailCount = d.BusinessOrderDetails.Count,
                    OrderSchedule = d.BusinessOrderDetails.Count(o => o.Status == Consts.StateOK) + "/" + d.BusinessOrderDetails.Count,
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
                    PublishDate = GetPurchaseOrderDetail(d.Id)?.PublishDate,
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
            int finish = 0;
            foreach (var item in businessOrder.BusinessOrderDetails)
            {
                var purchare = _purchaseOrderDetailRepository.LoadEntities(d => d.BusinessOrderDetailId == item.Id)
                    .FirstOrDefault();
                if (purchare == null) continue;
                if (purchare.Status==Consts.PurchaseStatusSuccess)
                {
                    finish++;
                }
            }
            //var purchaseOrder = _purchaseOrderRepository.LoadEntities(d => d.BusinessOrderId == businessOrder.Id)
            //    .FirstOrDefault();
            //int finish = 0;
            //if (purchaseOrder != null)
            //{
            //    finish = purchaseOrder.PurchaseOrderDetails.Count(d => d.IsDelete == false && d.Status == Consts.PurchaseStatusSuccess);
            //}
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
                MediaPriceId = d.MediaPrice.MediaId,
                VerificationStatus = d.VerificationStatus,
                PublishLink = GetPurchaseOrderDetail(d.Id)?.PublishLink,
                PublishDate = GetPurchaseOrderDetail(d.Id)?.PublishDate,
                PurchaseStatus = GetPurchaseOrderDetail(d.Id)?.Status,
                CostMoney = GetPurchaseOrderDetail(d.Id)?.PurchaseMoney - GetPurchaseOrderDetail(d.Id)?.PurchaseReturenOrderDetails.Sum(p => p.Money)
            });
            ViewBag.Invoices= item.BusinessInvoiceDetails.ToList();
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
            entity.Tax = viewModel.Tax ?? 0;
            //entity.DiscountRate = viewModel.DiscountRate;
            //entity.SettlementType = viewModel.SettlementType;
            entity.OrderDate = viewModel.OrderDate;
            //entity.TotalDiscountMoney = viewModel.DiscountMoney;
            var orderDetails = JsonConvert.DeserializeObject<List<BusinessOrderDetail>>(viewModel.OrderDetails);
            if (orderDetails.Count > 0)
            {
                foreach (var businessOrderDetail in orderDetails)
                {
                    businessOrderDetail.Id = IdBuilder.CreateIdNum();
                    businessOrderDetail.Status = Consts.StateLock;//待转采购
                    businessOrderDetail.VerificationStatus = Consts.StateLock;
                    businessOrderDetail.VerificationMoney = businessOrderDetail.SellMoney;
                    businessOrderDetail.ConfirmVerificationMoney = 0;
                    businessOrderDetail.AuditStatus = Consts.StateLock;
                    entity.BusinessOrderDetails.Add(businessOrderDetail);
                }
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
                Status = d.Status ?? Consts.StateLock,
                PurchaseStatus = GetPurchaseOrderDetail(d.Id)?.Status,
                PurchaseMoney = GetPurchaseOrderDetail(d.Id)?.PurchaseMoney - (GetPurchaseOrderDetail(d.Id)?.PurchaseReturenOrderDetails.Sum(p => p.Money)??0)
            }).OrderByDescending(d => d.Id);
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

            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.LinkManId = viewModel.LinkManId;
            entity.LinkManName = viewModel.LinkManName;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            entity.Tax = viewModel.Tax??0;
            entity.OrderDate = viewModel.OrderDate;
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
            foreach (var businessOrderDetail in orderDetails)
            {
                //新增
                var detail = _businessOrderDetailRepository.LoadEntities(d => d.Id == businessOrderDetail.Id)
                    .FirstOrDefault();
                if (detail == null)
                {
                    businessOrderDetail.Id = IdBuilder.CreateIdNum();
                    businessOrderDetail.Status = Consts.StateLock;
                    businessOrderDetail.AuditStatus = Consts.StateLock;
                    businessOrderDetail.VerificationStatus = Consts.StateLock;
                    businessOrderDetail.VerificationMoney = businessOrderDetail.SellMoney;
                    businessOrderDetail.ConfirmVerificationMoney = 0;
                    entity.BusinessOrderDetails.Add(businessOrderDetail);
                }
                else
                {
                    //未核销的，未审核的更新
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
                        detail.VerificationMoney = businessOrderDetail.SellMoney;
                        detail.ConfirmVerificationMoney = 0;
                    }
                    //if (detail.VerificationStatus == Consts.StateNormal)
                    //{
                    //    detail.Money = businessOrderDetail.Money;
                    //    detail.SellMoney = businessOrderDetail.SellMoney;
                    //    detail.Tax = businessOrderDetail.Tax;
                    //    detail.TaxMoney = businessOrderDetail.TaxMoney;
                    //    detail.VerificationMoney = 0;
                    //    detail.ConfirmVerificationMoney = businessOrderDetail.SellMoney;
                    //}

                    //可以修改稿件标题
                    detail.MediaTitle = businessOrderDetail.MediaTitle;
                    detail.PrePublishDate = businessOrderDetail.PrePublishDate;
                    detail.Remark = businessOrderDetail.Remark;

                }
            }


            //删除
            foreach (var deleteId in deleteIds)
            {
                var temp = _businessOrderDetailRepository.LoadEntities(d => d.Id == deleteId)
                    .FirstOrDefault();
                var purchase = GetPurchaseOrderDetail(deleteId);
                if (purchase != null)
                {
                    continue;
                }
                _businessOrderDetailRepository.Remove(temp);
            }

            _businessOrderService.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Update", new { id = entity.Id });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.BusinessInvoiceDetails.Count > 0)
            {
                return Json(new { State = 0, Msg = "此订单已开票无法删除" });
            }

            var p = PremissionData();
            if (!p.Any())
            {
                foreach (var entityBusinessOrderDetail in entity.BusinessOrderDetails)
                {
                    if (entityBusinessOrderDetail.VerificationStatus == Consts.StateNormal)
                    {
                        return Json(new { State = 0, Msg = "此销售订单明细有核销的，无法删除" });
                    }
                    var purchase = GetPurchaseOrderDetail(entityBusinessOrderDetail.Id);
                    if (purchase!=null)
                    {
                        //是否已经付款
                        if (purchase.PurchasePaymentOrderDetails.Count > 0)
                        {
                            return Json(new { State = 0, Msg = "有订单采购已请款，无法删除" });
                        }
                        _purchaseOrderDetailRepository.Remove(purchase);
                    }
                }
                _businessOrderService.Remove(entity);
                return Json(new { State = 1, Msg = "删除成功" });
            }
            if (entity.AuditStatus != Consts.StateNormal || entity.BusinessOrderDetails.Count == 0)
            {
                foreach (var entityBusinessOrderDetail in entity.BusinessOrderDetails)
                {
                    var purchase = GetPurchaseOrderDetail(entityBusinessOrderDetail.Id);
                    if (purchase != null)
                    {
                        return Json(new { State = 0, Msg = "此订单状态无法删除" });
                    }
                }
                _businessOrderService.Remove(entity);
                return Json(new { State = 1, Msg = "删除成功" });
            }
            return Json(new { State = 0, Msg = "此订单状态无法删除" });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult DeleteDetail(string id)
        {
            var entity = _businessOrderDetailRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity == null)
            {
                return Json(new { State = 1, Msg = "删除成功" });
            }
            var purchase = GetPurchaseOrderDetail(id);
            if (purchase != null)
            {
                return Json(new { State = 0, Msg = "此订单已下单，无法删除" });
            }
            _businessOrderDetailService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
        /// <summary>
        /// 批量确认
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Confirms()
        {
            var details = Request["rows"];
            if (string.IsNullOrWhiteSpace(details))
            {
                return Json(new { State = 0, Msg = "请先选择要确认的订单." });
            }
            var orderDetails = JsonConvert.DeserializeObject<List<BusinessOrderDetail>>(details);
            int i = 0;
            int count = orderDetails.Count;
            List<BusinessOrderDetail> list = new List<BusinessOrderDetail>();
            foreach (var businessOrderDetail in orderDetails)
            {
                var entity = _businessOrderDetailRepository.LoadEntities(d => d.Id == businessOrderDetail.Id).FirstOrDefault();
                if (entity == null)
                    continue;
                var purchase = GetPurchaseOrderDetail(entity.Id);
                if (purchase.Status != Consts.PurchaseStatusSuccess)
                    continue;
                if (entity.Status == Consts.StateOK)
                    continue;
                if (!(entity.SellMoney > purchase.PurchaseMoney)) continue;
                entity.Status = Consts.StateOK;//订单已完成
                entity.AuditStatus = Consts.StateNormal;//审核通过
                entity.SellMoney = businessOrderDetail.SellMoney;
                entity.VerificationMoney = businessOrderDetail.SellMoney;
                entity.Money = businessOrderDetail.Money;
                list.Add(entity);
                i++;
            }
            if (list.Count > 0)
            {
                _businessOrderDetailService.Update(list);
            }
            else
            {
                return Json(new { State = 0, Msg = "没有可以符合确认条件的订单." });
            }
            var temp = count - i;
            return Json(temp > 0 ? new { State = 1, Msg = "确认成功，其中有" + temp + "笔订单需要提交申请确认！" } : new { State = 1, Msg = "确认成功" });
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
            //已完成的都审核
            foreach (var entityBusinessOrderDetail in entity.BusinessOrderDetails.Where(d => d.Status == Consts.StateOK))
            {
                entityBusinessOrderDetail.AuditStatus = entity.AuditStatus;
            }
            _businessOrderService.Update(entity);
            TempData["Msg"] = "操作成功";
            return RedirectToAction("Update", new { id });
        }
        /// <summary>
        /// 确认订单（采购已完成，销售未完成）
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(BusinessOrderDetailView viewModel)
        {
            var entity = _businessOrderDetailRepository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            if (entity == null) return RedirectToAction("Index");
            var purchase = GetPurchaseOrderDetail(entity.Id);
            if (purchase.Status != Consts.PurchaseStatusSuccess)
                return RedirectToAction("Update", new { id = entity.BusinessOrderId });
            if (entity.Status == Consts.StateOK)
                return RedirectToAction("Update", new { id = entity.BusinessOrderId });
            entity.SellMoney = viewModel.SellMoney;
            entity.VerificationMoney = viewModel.SellMoney;
            entity.Money = viewModel.Money;
            entity.AuditRemark = viewModel.Remark;
            if (entity.SellMoney > purchase.PurchaseMoney)
            {
                entity.Status = Consts.StateOK;//订单已完成
                entity.AuditStatus = Consts.StateNormal;//审核通过
            }
            else
            {
                entity.Status = Consts.AuditConfirmState;//进入审核
                entity.AuditStatus = Consts.StateLock;//未审核
            }
            _businessOrderDetailService.Update(entity);
            return RedirectToAction("Update", new { id = entity.BusinessOrderId });

        }
        /// <summary>
        /// 申请改价
        /// </summary>
        /// <returns></returns>
        public ActionResult EditMoney(string id)
        {
            var entity = _businessOrderDetailRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            BusinessOrderDetailView detail = new BusinessOrderDetailView();
            detail.Id = id;
            detail.SellMoney = entity.SellMoney;
            detail.RequestSellMoney = 0;
            return PartialView("EditMoney", detail);

        }
        /// <summary>
        /// 申请改价
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMoney(BusinessOrderDetailView viewModel)
        {
            var entity = _businessOrderDetailRepository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            if (entity == null) return RedirectToAction("Index");
            if (entity.VerificationStatus == Consts.StateNormal)
            {
                TempData["Msg"] = "该订单已核销，无法改价";
                return RedirectToAction("Update", new { id = entity.BusinessOrderId });
            }
            entity.RequestSellMoney = viewModel.RequestSellMoney;
            entity.AuditRemark = viewModel.AuditRemark;
            entity.Status = Consts.AuditPriceState;//进入审核 
            _businessOrderDetailService.Update(entity);
            TempData["Msg"] = "申请成功";
            return RedirectToAction("Update", new { id = entity.BusinessOrderId });

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
            //int count = orderDetails.Count;
            int i = 0;
            foreach (var order in orderDetails)
            {
                //校验是否重复转单
                var isOrder = _purchaseOrderDetailRepository.LoadEntities(d => d.BusinessOrderDetailId == order.Id).FirstOrDefault();
                if (isOrder != null)
                {
                    continue;
                }
                var temp = _businessOrderDetailRepository.LoadEntities(d => d.Id == order.Id).FirstOrDefault();
                if (temp == null)
                {
                    order.Id = IdBuilder.CreateIdNum();
                    order.Status = Consts.StateNormal;
                    order.VerificationStatus = Consts.StateLock;
                    order.AuditStatus = Consts.StateLock;
                    order.VerificationMoney = order.SellMoney;
                    order.ConfirmVerificationMoney = 0;
                    entity.BusinessOrderDetails.Add(order);
                    i++;
                }
                else
                {
                    //已下单或已完成的，不用转
                    if (temp.Status == Consts.StateNormal || temp.Status == Consts.StateOK)
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
                    temp.VerificationMoney = order.SellMoney;
                    temp.ConfirmVerificationMoney = 0;
                    temp.Status = Consts.StateNormal;//已转采购
                    temp.AuditStatus = Consts.StateLock;
                    i++;
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
                purchaseOrderDetail.AddedDate = DateTime.Now;
                purchaseOrder.PurchaseOrderDetails.Add(purchaseOrderDetail);
            }

            if (i == 0)
            {
                return Json(new { State = 0, Msg = "没有符合转单条件的订单" });
            }
            if (isAdd)
            {
                _purchaseOrderService.Add(purchaseOrder);
            }
            else
            {
                _purchaseOrderService.Update(purchaseOrder);
            }

            return Json(new { State = 1, Msg = "成功转换" + i + "笔订单" });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult ImportOrder(BusinessOrderView viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { State = 0, Msg = "数据校验失败，请核对输入的信息是否准确" });
            }
            UEditorModel uploadConfig = new UEditorModel()
            {
                AllowExtensions = UEditorConfig.GetStringList("fileAllowFiles"),
                PathFormat = UEditorConfig.GetString("filePathFormat"),
                SizeLimit = UEditorConfig.GetInt("fileMaxSize"),
                UploadFieldName = UEditorConfig.GetString("fileFieldName")
            };
            var file = Request.Files[uploadConfig.UploadFieldName];
            if (file == null)
            {
                return Json(new { State = 0, Msg = "请选择要导入的文件" });
            }
            var uploadFileName = file.FileName;
            var fileExtension = Path.GetExtension(uploadFileName).ToLower();
            if (!uploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension))
            {
                return Json(new { State = 0, Msg = "文件类型不匹配" });
            }
            if (!(file.ContentLength < uploadConfig.SizeLimit))
            {
                return Json(new { State = 0, Msg = "上传的文件最大只能为：" + uploadConfig.SizeLimit + "B" });
            }
            //创建工作薄
            IWorkbook wk = new XSSFWorkbook(file.InputStream);
            //1.获取第一个工作表
            ISheet sheet = wk.GetSheetAt(0);
            if (sheet.LastRowNum <= 1)
            {
                return Json(new { State = 0, Msg = "此文件没有导入数据，请填充数据再进行导入" });
            }
            //销售订单
            BusinessOrder entity = new BusinessOrder();
            entity.Id = IdBuilder.CreateIdNum();
            entity.OrderNum = IdBuilder.CreateOrderNum("XD");
            entity.Status = Consts.StateNormal;//已下单
            entity.AuditStatus = Consts.StateNormal;//已审核
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedById = CurrentManager.Id;
            entity.AddedDate = DateTime.Now;
            entity.Remark = viewModel.Remark;
            entity.LinkManId = viewModel.LinkManId;
            entity.LinkManName = viewModel.LinkManName;
            entity.Transactor = CurrentManager.UserName;
            entity.TransactorId = CurrentManager.Id;
            entity.Tax = viewModel.Tax ?? 0;
            entity.OrderDate = DateTime.Now;
            //媒介订单
            PurchaseOrder purchase = new PurchaseOrder();
            purchase.Id = IdBuilder.CreateIdNum();
            purchase.BusinessOrderId = entity.Id;
            purchase.BusinessBy = entity.Transactor;
            purchase.BusinessById = entity.TransactorId;
            purchase.OrderDate = DateTime.Now;
            purchase.TotalMoney = 0;
            purchase.OrderNum = IdBuilder.CreateOrderNum("CD");
            purchase.Status = Consts.StateNormal;
            var count = 0;
            //处理订单明细
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                var mediaName = row.GetCell(0)?.ToString();
                var channal = row.GetCell(2)?.ToString();
                var adpositon = "发布价格";
                if (mediaName != null)
                {
                    mediaName = mediaName.Trim();
                    if (channal != null)
                    {
                        channal = channal.Trim();
                        var mediaPrice = _mediaPriceRepository.LoadEntities(d => d.Id == "X1805211737235784")
                            .FirstOrDefault();
                        decimal.TryParse(row.GetCell(4)?.ToString(), out var pmoney);
                        BusinessOrderDetail detail = new BusinessOrderDetail();
                        detail.Id = IdBuilder.CreateIdNum();
                        detail.Status = Consts.StateNormal; //已转单
                        detail.AdPositionName = adpositon;
                        decimal.TryParse(row.GetCell(3)?.ToString(), out var money);
                        detail.SellMoney = money;
                        detail.Tax = entity.Tax;
                        detail.TaxMoney = detail.SellMoney * (detail.Tax / 100);
                        detail.Money = detail.SellMoney * (1 + detail.Tax / 100);

                        detail.CostMoney = mediaPrice.PurchasePrice;
                        DateTime? date = null;
                        if (row.GetCell(7) != null)
                        {
                            var cellType = row.GetCell(7).CellType;
                            if (cellType == CellType.String)
                            {
                                DateTime.TryParse(row.GetCell(7)?.ToString(), out var temp);
                                date = temp;
                            }
                            if (cellType == CellType.Numeric)
                            {
                                date = row.GetCell(7).DateCellValue;
                            }
                        }
                        detail.PrePublishDate = date;
                        detail.MediaTitle = row.GetCell(5)?.ToString();
                        detail.MediaTypeName = mediaPrice.Media.MediaType.TypeName;
                        detail.MediaName = mediaName + " - " + channal;
                        detail.MediaPriceId = mediaPrice.Id;
                        detail.Remark = viewModel.Remark;
                        detail.MediaByPurchase = mediaPrice.Media.Transactor;
                        detail.VerificationStatus = Consts.StateLock; //待核销
                        detail.VerificationMoney = detail.SellMoney;
                        detail.ConfirmVerificationMoney = 0;
                        detail.AuditStatus = Consts.StateLock;
                        entity.BusinessOrderDetails.Add(detail);
                        //媒介明细
                        PurchaseOrderDetail purchaseOrderDetail = new PurchaseOrderDetail();
                        purchaseOrderDetail.Id = IdBuilder.CreateIdNum();
                        purchaseOrderDetail.BusinessOrderDetailId = detail.Id;
                        purchaseOrderDetail.CostMoney = detail.CostMoney;
                        purchaseOrderDetail.VerificationStatus = Consts.StateLock;
                        purchaseOrderDetail.AdPositionName = detail.AdPositionName;
                        purchaseOrderDetail.MediaTitle = detail.MediaTitle;
                        purchaseOrderDetail.MediaName = detail.MediaName;
                        purchaseOrderDetail.MediaTypeName = detail.MediaTypeName;
                        purchaseOrderDetail.MediaPriceId = detail.MediaPriceId;
                        purchaseOrderDetail.AuditStatus = Consts.StateNormal; //已审核
                        purchaseOrderDetail.Status = Consts.PurchaseStatusSuccess; //已完成
                        purchaseOrderDetail.Transactor = mediaPrice.Media.Transactor;
                        purchaseOrderDetail.TransactorId = mediaPrice.Media.TransactorId;
                        purchaseOrderDetail.LinkMan = mediaPrice.Media.LinkMan;
                        purchaseOrderDetail.LinkManName = row.GetCell(1)?.ToString();
                        purchaseOrderDetail.Tax = 0;
                        purchaseOrderDetail.TaxMoney = 0;
                        purchaseOrderDetail.Money = pmoney;
                        purchaseOrderDetail.ConfirmVerificationMoney = 0;
                        purchaseOrderDetail.VerificationMoney = pmoney;
                        purchaseOrderDetail.DiscountMoney = 0;
                        purchaseOrderDetail.PurchaseMoney = pmoney;
                        purchaseOrderDetail.DiscountRate = 100;
                        purchaseOrderDetail.PublishDate = detail.PrePublishDate;
                        purchaseOrderDetail.PublishLink = row.GetCell(6)?.ToString();
                        purchase.PurchaseOrderDetails.Add(purchaseOrderDetail);
                    }
                }

                count++;
            }
            _purchaseOrderRepository.Add(purchase);
            _businessOrderService.Add(entity);

            return Json(new { State = 1, Msg = "成功导入" + count + "篇网稿订单。" });
        }


        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult ImportOrders(BusinessOrderView viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { State = 0, Msg = "数据校验失败，请核对输入的信息是否准确" });
            }
            UEditorModel uploadConfig = new UEditorModel()
            {
                AllowExtensions = UEditorConfig.GetStringList("fileAllowFiles"),
                PathFormat = UEditorConfig.GetString("filePathFormat"),
                SizeLimit = UEditorConfig.GetInt("fileMaxSize"),
                UploadFieldName = UEditorConfig.GetString("fileFieldName")
            };
            var file = Request.Files[uploadConfig.UploadFieldName];
            if (file == null)
            {
                return Json(new { State = 0, Msg = "请选择要导入的文件" });
            }
            var uploadFileName = file.FileName;
            var fileExtension = Path.GetExtension(uploadFileName).ToLower();
            if (!uploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension))
            {
                return Json(new { State = 0, Msg = "文件类型不匹配" });
            }
            if (!(file.ContentLength < uploadConfig.SizeLimit))
            {
                return Json(new { State = 0, Msg = "上传的文件最大只能为：" + uploadConfig.SizeLimit + "B" });
            }
            //创建工作薄
            IWorkbook wk = new XSSFWorkbook(file.InputStream);
            //1.获取第一个工作表
            ISheet sheet = wk.GetSheetAt(0);
            if (sheet.LastRowNum <= 1)
            {
                return Json(new { State = 0, Msg = "此文件没有导入数据，请填充数据再进行导入" });
            }
            //销售订单
            BusinessOrder entity = new BusinessOrder();
            entity.Id = IdBuilder.CreateIdNum();
            entity.OrderNum = IdBuilder.CreateOrderNum("XD");
            entity.Status = Consts.StateNormal;//已下单
            entity.AuditStatus = Consts.StateNormal;//已审核
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedById = CurrentManager.Id;
            entity.AddedDate = DateTime.Now;
            entity.Remark = viewModel.Remark;
            entity.LinkManId = viewModel.LinkManId;
            entity.LinkManName = viewModel.LinkManName;
            entity.Transactor = CurrentManager.UserName;
            entity.TransactorId = CurrentManager.Id;
            entity.Tax = viewModel.Tax ?? 0;
            entity.OrderDate = DateTime.Now;
            //媒介订单
            PurchaseOrder purchase = new PurchaseOrder();
            purchase.Id = IdBuilder.CreateIdNum();
            purchase.BusinessOrderId = entity.Id;
            purchase.BusinessBy = entity.Transactor;
            purchase.BusinessById = entity.TransactorId;
            purchase.OrderDate = DateTime.Now;
            purchase.TotalMoney = 0;
            purchase.OrderNum = IdBuilder.CreateOrderNum("CD");
            purchase.Status = Consts.StateNormal;
            var count = 0;
            List<string> losts = new List<string>();
            List<string> heightPrice = new List<string>();
            //处理订单明细
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                var mediaName = row.GetCell(0)?.ToString();
                var client = row.GetCell(1)?.ToString();
                var channal = row.GetCell(2)?.ToString();
                var adpositon = row.GetCell(5)?.ToString();
                //根据资源找价格
                if (string.IsNullOrWhiteSpace(mediaName) || string.IsNullOrWhiteSpace(client) || string.IsNullOrWhiteSpace(channal) || string.IsNullOrWhiteSpace(adpositon))
                {
                    continue;
                }
                mediaName = mediaName.Trim();
                client = client.Trim();
                channal = channal.Trim();
                adpositon = adpositon.Trim();
                var mediaPrice = _mediaPriceRepository.LoadEntities(d =>
                     d.Media.MediaName.Equals(mediaName, StringComparison.CurrentCultureIgnoreCase) &&
                     d.Media.Client.Equals(client, StringComparison.CurrentCultureIgnoreCase) &&
                     d.Media.Channel.Equals(channal, StringComparison.CurrentCultureIgnoreCase) &&
                     d.Media.IsDelete == false && d.AdPositionName == adpositon && d.IsDelete == false).FirstOrDefault();
                if (mediaPrice == null)
                {
                    losts.Add(mediaName + " - " + client + " - " + channal);
                    continue;
                }
                decimal.TryParse(row.GetCell(4)?.ToString(), out var pmoney);
                //判断采购金额是否超出成本
                if (pmoney > mediaPrice.PurchasePrice)
                {
                    heightPrice.Add(mediaName + " - " + client + " - " + channal);
                    continue;
                }
                BusinessOrderDetail detail = new BusinessOrderDetail();
                detail.Id = IdBuilder.CreateIdNum();
                detail.Status = Consts.StateNormal;//已转单
                detail.AdPositionName = adpositon;
                decimal.TryParse(row.GetCell(3)?.ToString(), out var money);

                detail.SellMoney = money;
                detail.Tax = entity.Tax;
                detail.TaxMoney = detail.SellMoney * (detail.Tax / 100);
                detail.Money = detail.SellMoney * (1 + detail.Tax / 100);

                detail.CostMoney = mediaPrice.PurchasePrice;
                DateTime.TryParse(row.GetCell(8)?.ToString(), out var date);
                detail.PrePublishDate = date;
                detail.MediaTitle = row.GetCell(6)?.ToString();
                detail.MediaTypeName = mediaPrice.Media.MediaType.TypeName;
                detail.MediaName = mediaName + " - " + client + " - " + channal;
                detail.MediaPriceId = mediaPrice.Id;
                detail.Remark = viewModel.Remark;
                detail.MediaByPurchase = mediaPrice.Media.Transactor;
                detail.VerificationStatus = Consts.StateLock;//待核销
                detail.VerificationMoney = detail.SellMoney;
                detail.ConfirmVerificationMoney = 0;
                detail.AuditStatus = Consts.StateLock;
                entity.BusinessOrderDetails.Add(detail);
                //媒介明细
                PurchaseOrderDetail purchaseOrderDetail = new PurchaseOrderDetail();
                purchaseOrderDetail.Id = IdBuilder.CreateIdNum();
                purchaseOrderDetail.BusinessOrderDetailId = detail.Id;
                purchaseOrderDetail.CostMoney = detail.CostMoney;
                purchaseOrderDetail.VerificationStatus = Consts.StateLock;
                purchaseOrderDetail.AdPositionName = detail.AdPositionName;
                purchaseOrderDetail.MediaTitle = detail.MediaTitle;
                purchaseOrderDetail.MediaName = detail.MediaName;
                purchaseOrderDetail.MediaTypeName = detail.MediaTypeName;
                purchaseOrderDetail.MediaPriceId = detail.MediaPriceId;
                purchaseOrderDetail.AuditStatus = Consts.StateNormal;//已审核
                purchaseOrderDetail.Status = Consts.PurchaseStatusSuccess;//已完成
                purchaseOrderDetail.Transactor = mediaPrice.Media.Transactor;
                purchaseOrderDetail.TransactorId = mediaPrice.Media.TransactorId;
                purchaseOrderDetail.LinkMan = mediaPrice.Media.LinkMan;
                purchaseOrderDetail.LinkManName = mediaPrice.Media.LinkMan.Name;
                purchaseOrderDetail.Tax = 0;
                purchaseOrderDetail.TaxMoney = 0;

                purchaseOrderDetail.Money = pmoney;
                purchaseOrderDetail.ConfirmVerificationMoney = 0;
                purchaseOrderDetail.VerificationMoney = pmoney;
                purchaseOrderDetail.DiscountMoney = 0;
                purchaseOrderDetail.PurchaseMoney = pmoney;
                purchaseOrderDetail.DiscountRate = 100;
                purchaseOrderDetail.PublishDate = detail.PrePublishDate;
                purchaseOrderDetail.PublishLink = row.GetCell(7)?.ToString();
                purchase.PurchaseOrderDetails.Add(purchaseOrderDetail);
                count++;
            }
            _purchaseOrderRepository.Add(purchase);
            _businessOrderService.Add(entity);
            var lostStr = string.Empty;
            if (losts.Count > 0)
            {
                lostStr = "其中以下资源不存在：【" + string.Join("，", losts) + "】";
            }
            var heightPriceStr = string.Empty;
            if (heightPrice.Count > 0)
            {
                heightPriceStr = "其中以下资源采购成本超出参考成本：【" + string.Join("，", heightPrice) + "】";
            }
            return Json(new { State = 1, Msg = "成功导入" + count + "篇网稿订单。" + lostStr + heightPriceStr });
        }


        public ActionResult Comment(string id)
        {
            var oreder = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            //var count = oredr.BusinessOrderDetails.Count(d => d.MediaPrice.Media.MediaType.IsComment == true);
            return View(oreder);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comment(CommentsView comments)
        {
            //
            List<OrderDetailComment> list = new List<OrderDetailComment>();
            foreach (var comment in comments.OrderComments)
            {
                OrderDetailComment item = new OrderDetailComment();
                item.CommentDate = DateTime.Now;
                item.BusinessOrderDetailId = comment.BusinessOrderDetailId;
                item.Score = comment.Score;
                item.Content = comment.Content;
                item.Transactor = CurrentManager.UserName;
                item.TransactorId = CurrentManager.Id;
                item.Id = IdBuilder.CreateIdNum();
                list.Add(item);
            }
            _orderDetailCommentService.Add(list);
            var order = _repository.LoadEntities(d => d.Id == comments.OrderId).FirstOrDefault();
            return View(order);
        }
        /// <summary>
        /// 转移订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult ChangeOrder()
        {
            var ids = Request["ids"];
            if (string.IsNullOrWhiteSpace(ids))
            {
                return Json(new { State = 0, Msg = "请先选择要转移的订单." });
            }
            var on = Request["on"];
            var order = _repository.LoadEntities(d => d.OrderNum == on).FirstOrDefault();
            if (order == null)
            {
                return Json(new { State = 0, Msg = "不存在的订单." });
            }
            var idList = ids.Split(',').ToList();
            if (!order.BusinessOrderDetails.Any())
            {
                order.Status = Consts.StateNormal;
                order.AuditStatus = Consts.StateNormal;
            }
            _businessOrderDetailService.Update(d => idList.Contains(d.Id), d => new BusinessOrderDetail { BusinessOrderId = order.Id,Tax = order.Tax});
            return Json(new { State = 0, Msg = "转移成功." });
        }
    }
}