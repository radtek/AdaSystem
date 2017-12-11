using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Purchase;
using Ada.Framework.Filter;
using Ada.Services.Purchase;
using Newtonsoft.Json;

namespace Purchase.Controllers
{
    /// <summary>
    /// 媒介请款
    /// </summary>
    public class PaymentController : BaseController
    {
        private readonly IPurchasePaymentService _purchasePaymentService;
        private readonly IRepository<PurchasePayment> _repository;
        private readonly IRepository<PurchasePaymentDetail> _purchasePaymentDetailRepository;
        private readonly IRepository<PurchasePaymentOrderDetail> _purchasePaymentOrderDetailRepository;
        public PaymentController(IPurchasePaymentService purchasePaymentService,
            IRepository<PurchasePayment> repository,
            IRepository<PurchasePaymentDetail> purchasePaymentDetailRepository,
            IRepository<PurchasePaymentOrderDetail> purchasePaymentOrderDetailRepository)
        {
            _purchasePaymentService = purchasePaymentService;
            _repository = repository;
            _purchasePaymentDetailRepository = purchasePaymentDetailRepository;
            _purchasePaymentOrderDetailRepository = purchasePaymentOrderDetailRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(PurchasePaymentView viewModel)
        {
            var result = _purchasePaymentService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new PurchasePaymentView
                {
                    Id = d.Id,
                    //Status = d.Status,
                    LinkManName = d.LinkManName,
                    Transactor = d.Transactor,
                    BillDate = d.BillDate,
                    BillNum = d.BillNum,
                    PayMoney = d.PurchasePaymentDetails.Sum(m => m.PayMoney)
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            PurchasePaymentView viewModel = new PurchasePaymentView();
            viewModel.BillDate = DateTime.Now;
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
            viewModel.PayMoney = 0;
            return View(viewModel);
        }


        public ActionResult OrderDetails()
        {
            return PartialView("OrderDetails");
        }
        public ActionResult PaymentDetails(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("PaymentDetails", entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(PurchasePaymentView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var paydetails = JsonConvert.DeserializeObject<List<PurchasePaymentDetail>>(viewModel.PayDetails);
            if (paydetails.Count <= 0)
            {
                ModelState.AddModelError("message", "付款信息不能为空");
                return View(viewModel);
            }
            var details = JsonConvert.DeserializeObject<List<PurchaseOrderDetail>>(viewModel.OrderDetails);
            if (details.Count <= 0)
            {
                ModelState.AddModelError("message", "请款订单明细不能为空");
                return View(viewModel);
            }
            PurchasePayment payment = new PurchasePayment();
            payment.Id = IdBuilder.CreateIdNum();
            //payment.PayMoney = viewModel.PayMoney;

            payment.Transactor = viewModel.Transactor;
            payment.TransactorId = viewModel.TransactorId;
            payment.Status = Consts.StateLock;//待付款
            payment.LinkManName = viewModel.LinkManName;
            payment.LinkManId = viewModel.LinkManId;
            decimal? ordermoney = 0;
            decimal? paymoney = 0;
            //付款信息
            foreach (var item in paydetails)
            {

                item.Id = IdBuilder.CreateIdNum();
                item.AddedBy = CurrentManager.UserName;
                item.AddedById = CurrentManager.Id;
                item.AddedDate = DateTime.Now;
                item.Status = Consts.StateLock;//待付款
                item.AuditStatus = Consts.StateLock;//待审批
                payment.PurchasePaymentDetails.Add(item);
                paymoney += item.PayMoney;
            }
            //订单明细
            foreach (var item in details)
            {
                PurchasePaymentOrderDetail orderDetail = new PurchasePaymentOrderDetail();
                orderDetail.Id = IdBuilder.CreateIdNum();
                orderDetail.PurchaseOrderDetailId = item.Id;
                orderDetail.AddedBy = CurrentManager.UserName;
                orderDetail.AddedById = CurrentManager.Id;
                orderDetail.AddedDate = DateTime.Now;
                payment.PurchasePaymentOrderDetails.Add(orderDetail);
                ordermoney += item.CostMoney;
            }
            if (paymoney > ordermoney)
            {
                ModelState.AddModelError("message", "申请付款金额超出成本金额");
                return View(viewModel);
            }
            payment.AddedBy = CurrentManager.UserName;
            payment.AddedById = CurrentManager.Id;
            payment.AddedDate = DateTime.Now;
            payment.BillNum = IdBuilder.CreateOrderNum("QK");
            payment.BillDate = viewModel.BillDate;
            _purchasePaymentService.Add(payment);
            TempData["Msg"] = "申请成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            PurchasePaymentView viewModel = new PurchasePaymentView();
            viewModel.BillDate = entity.BillDate;
            viewModel.Transactor = entity.Transactor;
            viewModel.TransactorId = entity.TransactorId;
            viewModel.PayMoney = entity.PayMoney;
            viewModel.LinkManId = entity.LinkManId;
            viewModel.LinkManName = entity.LinkManName;
            viewModel.BillNum = entity.BillNum;
            viewModel.Id = id;
            List<PurchaseOrderDetail> details = entity.PurchasePaymentOrderDetails.Select(d => d.PurchaseOrderDetail).ToList();
            viewModel.IsDisable = entity.PurchasePaymentDetails.Count(d => d.AuditStatus == Consts.StateNormal) == 0;
            viewModel.OrderDetails = JsonConvert.SerializeObject(details.Select(d => new
            {
                d.Id,
                d.CostMoney,
                d.MediaName,
                d.MediaTypeName,
                d.Money,
                d.AdPositionName
            }));
            var paydetails = entity.PurchasePaymentDetails.Select(d => new
            {
                d.Id,
                d.AuditStatus,
                d.PayMoney,
                d.AccountBank,
                d.AccountName,
                d.AccountNum,
                d.PaymentType
            }).ToList();
            viewModel.PayDetails = JsonConvert.SerializeObject(paydetails);
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(PurchasePaymentView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var orderdetails = JsonConvert.DeserializeObject<List<PurchaseOrderDetail>>(viewModel.OrderDetails);
            if (orderdetails.Count <= 0)
            {
                ModelState.AddModelError("message", "请款订单明细不能为空");
                return View(viewModel);
            }
            var paydetails = JsonConvert.DeserializeObject<List<PurchasePaymentDetail>>(viewModel.PayDetails);
            if (paydetails.Count <= 0)
            {
                ModelState.AddModelError("message", "请款付款信息不能为空");
                return View(viewModel);
            }
            PurchasePayment payment = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            payment.Transactor = viewModel.Transactor;
            payment.TransactorId = viewModel.TransactorId;
            decimal? ordermoney;
            decimal? paymoney = 0;
            //如果审核了，以下信息就不作更新
            if (payment.PurchasePaymentDetails.Count(d => d.AuditStatus == Consts.StateNormal) == 0)
            {
                decimal? money = 0;
                payment.LinkManName = viewModel.LinkManName;
                payment.LinkManId = viewModel.LinkManId;
                payment.BillDate = viewModel.BillDate;
                //清空明细，重新增加
                _purchasePaymentOrderDetailRepository.Remove(payment.PurchasePaymentOrderDetails);
                foreach (var item in orderdetails)
                {
                    PurchasePaymentOrderDetail orderDetail = new PurchasePaymentOrderDetail();
                    orderDetail.Id = IdBuilder.CreateIdNum();
                    orderDetail.PurchaseOrderDetailId = item.Id;
                    orderDetail.AddedBy = CurrentManager.UserName;
                    orderDetail.AddedById = CurrentManager.Id;
                    orderDetail.AddedDate = DateTime.Now;
                    payment.PurchasePaymentOrderDetails.Add(orderDetail);
                    money += item.CostMoney;
                }
                ordermoney = money;
                _purchasePaymentDetailRepository.Remove(payment.PurchasePaymentDetails);
                foreach (var item in paydetails)
                {
                    item.Id = IdBuilder.CreateIdNum();
                    item.AddedBy = CurrentManager.UserName;
                    item.AddedById = CurrentManager.Id;
                    item.AddedDate = DateTime.Now;
                    item.Status = Consts.StateLock;//待付款
                    item.AuditStatus = Consts.StateLock;//待审批
                    payment.PurchasePaymentDetails.Add(item);
                    paymoney += item.PayMoney;
                }
            }
            else
            {
                paymoney = payment.PurchasePaymentDetails.Where(d => d.AuditStatus == Consts.StateNormal).Sum(d => d.PayMoney);//审核通过的金额
                ordermoney = payment.PurchasePaymentOrderDetails.Sum(d => d.PurchaseOrderDetail.CostMoney);
                //只增加
                foreach (var item in paydetails)
                {
                    var temp = _purchasePaymentDetailRepository.LoadEntities(d => d.Id == item.Id).FirstOrDefault();
                    if (temp == null)
                    {
                        item.Id = IdBuilder.CreateIdNum();
                        item.AddedBy = CurrentManager.UserName;
                        item.AddedById = CurrentManager.Id;
                        item.AddedDate = DateTime.Now;
                        item.Status = Consts.StateLock;//待付款
                        item.AuditStatus = Consts.StateLock;//待审批
                        payment.PurchasePaymentDetails.Add(item);
                        paymoney += item.PayMoney;
                    }
                    else
                    {
                        if (temp.AuditStatus != Consts.StateLock) continue;
                        temp.ModifiedBy = CurrentManager.UserName;
                        temp.ModifiedById = CurrentManager.Id;
                        temp.ModifiedDate = DateTime.Now;
                        temp.AccountNum = item.AccountNum;
                        temp.AccountBank = item.AccountBank;
                        temp.AccountName = item.AccountName;
                        temp.PayMoney = item.PayMoney;
                        paymoney += item.PayMoney;
                    }

                }
            }
            if (paymoney > ordermoney)
            {
                ModelState.AddModelError("message", "申请付款金额超出成本金额");
                return View(viewModel);
            }
            payment.ModifiedBy = CurrentManager.UserName;
            payment.ModifiedById = CurrentManager.Id;
            payment.ModifiedDate = DateTime.Now;

            _purchasePaymentService.Update(payment);
            TempData["Msg"] = "修改成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.PurchasePaymentDetails.Any(entityPurchasePaymentDetail => entityPurchasePaymentDetail.AuditStatus == Consts.StateNormal))
            {
                return Json(new { State = 0, Msg = "申请审核已通过，无法删除" });
            }
            if (entity.PurchasePaymentDetails.Any(entityPurchasePaymentDetail => entityPurchasePaymentDetail.Status == Consts.StateNormal))
            {
                return Json(new { State = 0, Msg = "申请审核已付款，无法删除" });
            }
            _purchasePaymentService.Delete(entity);//物理删除
            return Json(new { State = 1, Msg = "撤销成功" });
        }
    }
}