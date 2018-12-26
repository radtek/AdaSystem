using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// 采购退款
    /// </summary>
    public class OrderReturnController : BaseController
    {
        private readonly IPurchaseReturnOrderService _service;
        private readonly IRepository<PurchaseReturnOrder> _purchaseReturnOrderrepository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseOrderDetailrepository;
        private readonly IRepository<PurchaseReturenOrderDetail> _purchaseReturenOrderDetailRepository;

        public OrderReturnController(IPurchaseReturnOrderService service,
            IRepository<PurchaseReturenOrderDetail> purchaseReturenOrderDetailRepository,
            IRepository<PurchaseOrderDetail> purchaseOrderDetailrepository,
            IRepository<PurchaseReturnOrder> purchaseReturnOrderrepository)
        {
            _service = service;
            _purchaseReturenOrderDetailRepository = purchaseReturenOrderDetailRepository;
            _purchaseOrderDetailrepository = purchaseOrderDetailrepository;
            _purchaseReturnOrderrepository = purchaseReturnOrderrepository;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(PurchaseReturnOrderView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _service.LoadEntitiesFilter(viewModel).AsNoTracking().ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new PurchaseReturnOrderView
                {
                    Id = d.Id,
                    Status = d.Status,
                    LinkManName = d.LinkManName,
                    Transactor = d.Transactor,
                    ReturnOrderNum = d.ReturnOrderNum,
                    TotalMoney = d.TotalMoney,
                    ReturnDate = d.ReturnDate


                })
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add()
        {
            PurchaseReturnOrderView viewModel = new PurchaseReturnOrderView();
            viewModel.ReturnDate = DateTime.Now;
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
            viewModel.AuditStatus = Consts.StateLock;
            return View(viewModel);
        }

        [HttpPost]
        
        public ActionResult Add(PurchaseReturnOrderView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var details = JsonConvert.DeserializeObject<List<PurchaseReturenOrderDetail>>(viewModel.OrderDetails);
            if (details.Count <= 0)
            {
                ModelState.AddModelError("message", "请款订单明细不能为空");
                return View(viewModel);
            }
            PurchaseReturnOrder entity = new PurchaseReturnOrder();
            entity.Id = IdBuilder.CreateIdNum();
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            entity.AuditStatus = Consts.StateLock;//待审核
            entity.Status = Consts.StateLock;//待收款
            entity.LinkManName = viewModel.LinkManName;
            entity.LinkManId = viewModel.LinkManId;
            //订单明细
            decimal? money = 0;
            foreach (var item in details)
            {
                if (item.Money==0||item.Money==null)
                {
                    ModelState.AddModelError("message", "订单明细退款金额不能为空或者为0");
                    return View(viewModel);
                }
                if (string.IsNullOrWhiteSpace(item.ReturnReason))
                {
                    ModelState.AddModelError("message", "订单明细退款原因必须填写");
                    return View(viewModel);
                }
                //校验此订单明细是否总的申请退款超出采购无税金额
                var order = _purchaseOrderDetailrepository.LoadEntities(d => d.Id == item.Id).FirstOrDefault();
                if (order == null) continue;
                //验证是否请款了
                if (!order.PurchasePaymentOrderDetails.Any())
                {
                    ModelState.AddModelError("message", order.MediaName + " 此订单并未请款");
                    return View(viewModel);
                }

                if (order.Status==Consts.PurchaseStatusFail)
                {
                    ModelState.AddModelError("message", order.MediaName + " 此订单状态无法退款");
                    return View(viewModel);
                }
                var purchaseMoney = order.PurchaseMoney;
                var temp = _purchaseReturenOrderDetailRepository.LoadEntities(d => d.PurchaseOrderDetailId == item.Id).Sum(d => d.Money);
                if (purchaseMoney < (temp + item.Money))
                {
                    ModelState.AddModelError("message", order.MediaName + " 此订单退款总金额已超出采购金额");
                    return View(viewModel);
                }
                PurchaseReturenOrderDetail orderDetail = new PurchaseReturenOrderDetail();
                orderDetail.Id = IdBuilder.CreateIdNum();
                orderDetail.PurchaseOrderDetailId = item.Id;
                orderDetail.Money = item.Money;
                orderDetail.ReturnReason = item.ReturnReason;
                entity.PurchaseReturenOrderDetails.Add(orderDetail);
                money += item.Money;

            }

            if (money == 0)
            {
                ModelState.AddModelError("message", "申请退款总金额需大于0");
                return View(viewModel);
            }
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedById = CurrentManager.Id;
            entity.AddedDate = DateTime.Now;
            entity.ReturnOrderNum = IdBuilder.CreateOrderNum("TK");
            entity.ReturnDate = viewModel.ReturnDate;
            entity.PurchaseOrderId = entity.ReturnOrderNum;
            entity.TotalMoney = money;
            _service.Add(entity);
            TempData["Msg"] = "申请成功";
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public ActionResult Details(string id)
        {
            var returnorder = _purchaseReturnOrderrepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("Details", returnorder);
        }
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            var returnorder = _purchaseReturnOrderrepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (returnorder.Status==Consts.StateNormal)
            {
                return Json(new { State = 0, Msg = "此退款单据已收款，请联系财务处理！" });
            }
           
            _service.Delete(returnorder);
            return Json(new {State = 1, Msg = "撤销成功"});
        }
    }
}