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
        public PaymentController(IPurchasePaymentService purchasePaymentService,
            IRepository<PurchasePayment> repository,
            IRepository<PurchasePaymentDetail> purchasePaymentDetailRepository)
        {
            _purchasePaymentService = purchasePaymentService;
            _repository = repository;
            _purchasePaymentDetailRepository = purchasePaymentDetailRepository;
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
                    Status = d.Status,
                    LinkManName = d.LinkManName,
                    Transactor = d.Transactor,
                    PayMoney = d.PayMoney
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(PurchasePaymentView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
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
            payment.PayMoney = viewModel.PayMoney;

            payment.Transactor = viewModel.Transactor;
            payment.TransactorId = viewModel.TransactorId;
            payment.Status = Consts.StateLock;//待付款
            payment.LinkManName = viewModel.LinkManName;
            payment.LinkManId = viewModel.LinkManId;
            decimal? money = 0;
            foreach (var item in details)
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
            if (viewModel.PayMoney > money)
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
            List<PurchaseOrderDetail> details= entity.PurchasePaymentOrderDetails.Select(entityPurchasePaymentDetail => entityPurchasePaymentDetail.PurchaseOrderDetail).ToList();
            viewModel.OrderDetails = JsonConvert.SerializeObject(details.Select(d => new
            {
                d.Id,d.CostMoney,d.MediaName,d.MediaTypeName,d.Money,d.AdPositionName
            }));
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(PurchasePaymentView viewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
            //    return View(viewModel);
            //}
            //var details = JsonConvert.DeserializeObject<List<PurchaseOrderDetail>>(viewModel.Details);
            //if (details.Count <= 0)
            //{
            //    ModelState.AddModelError("message", "请款订单明细不能为空");
            //    return View(viewModel);
            //}
            //PurchasePayment payment = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            //if (payment.AuditStatus ==Consts.StateNormal)
            //{
            //    ModelState.AddModelError("message", "请款订单状态已审无法修改");
            //    return View(viewModel);
            //}
            //payment.AccountBank = viewModel.AccountBank;
            //payment.AccountName = viewModel.AccountName;
            //payment.AccountNum = viewModel.AccountNum;
            //payment.PayMoney = viewModel.PayMoney;
            //payment.PaymentType = viewModel.PaymentType;
            //payment.Transactor = viewModel.Transactor;
            //payment.TransactorId = viewModel.TransactorId;
            //payment.LinkManName = viewModel.LinkManName;
            //payment.LinkManId = viewModel.LinkManId;
            //decimal? money = 0;
            ////清空明细，重新增加
            //_purchasePaymentDetailRepository.Remove(payment.PurchasePaymentDetails);
            //foreach (var item in details)
            //{
            //    PurchasePaymentDetail paymentDetail = new PurchasePaymentDetail();
            //    paymentDetail.Id = IdBuilder.CreateIdNum();
            //    paymentDetail.PurchaseOrderDetailId = item.Id;
            //    paymentDetail.AddedBy = CurrentManager.UserName;
            //    paymentDetail.AddedById = CurrentManager.Id;
            //    paymentDetail.AddedDate = DateTime.Now;
            //    payment.PurchasePaymentDetails.Add(paymentDetail);
            //    money += item.CostMoney;
            //}
            //if (viewModel.PayMoney > money)
            //{
            //    ModelState.AddModelError("message", "申请付款金额超出成本金额");
            //    return View(viewModel);
            //}
            //payment.ModifiedBy = CurrentManager.UserName;
            //payment.ModifiedById = CurrentManager.Id;
            //payment.ModifiedDate = DateTime.Now;
            //payment.ApplicationDate = viewModel.ApplicationDate;
            //_purchasePaymentService.Update(payment);
            TempData["Msg"] = "修改成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            //if (entity.AuditStatus==Consts.StateNormal)
            //{
            //    return Json(new { State = 0, Msg = "申请已通过，无法删除" });
            //}
            //if (entity.Status == Consts.StateNormal)
            //{
            //    return Json(new { State = 0, Msg = "申请已付款，无法删除" });
            //}
            //_purchasePaymentService.Delete(entity);//物理删除
            return Json(new { State = 1, Msg = "撤销成功" });
        }
    }
}