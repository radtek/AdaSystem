using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Finance;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Business;

namespace Business.Controllers
{
    /// <summary>
    /// 核销
    /// </summary>
    public class WriteOffController : BaseController
    {
        private readonly IBusinessWriteOffService _businessWriteOffService;
        private readonly IRepository<BusinessWriteOff> _repository;
        private readonly IRepository<BusinessOrderDetail> _businessOrderDetailRepository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseOrderDetailRepository;
        private readonly IRepository<BusinessPayee> _businessPayeerepository;
        private readonly IRepository<BusinessPayment> _businessPaymentRepository;
        private readonly IRepository<Receivables> _receivablesRepository;
        public WriteOffController(IBusinessWriteOffService businessWriteOffService,
            IRepository<BusinessWriteOff> repository,
            IRepository<BusinessOrderDetail> businessOrderDetailRepository,
            IRepository<BusinessPayee> businessPayeerepository,
            IRepository<PurchaseOrderDetail> purchaseOrderDetailRepository,
            IRepository<BusinessPayment> businessPaymentRepository,
            IRepository<Receivables> receivablesRepository)
        {
            _businessWriteOffService = businessWriteOffService;
            _repository = repository;
            _businessOrderDetailRepository = businessOrderDetailRepository;
            _businessPayeerepository = businessPayeerepository;
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
            _businessPaymentRepository = businessPaymentRepository;
            _receivablesRepository = receivablesRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessWriteOffView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _businessWriteOffService.LoadEntitiesFilter(viewModel).AsNoTracking().ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessWriteOffView
                {
                    Id = d.Id,
                    //LinkManName = d.BusinessOrderDetails.FirstOrDefault()?.BusinessOrder.LinkManName,
                    //OrderNum = d.BusinessOrderDetails.FirstOrDefault()?.BusinessOrder.OrderNum,
                    Transactor = d.Transactor,
                    Money = d.Money,
                    WriteOffDate = d.WriteOffDate
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail(string id,string pId=null,string rId=null)
        {
            if (!string.IsNullOrWhiteSpace(pId))
            {
                
                var pay = _businessPaymentRepository.LoadEntities(d => d.ApplicationNum == pId).FirstOrDefault();
                return PartialView("Details", pay.BusinessPayee.BusinessWriteOffs);
            }

            if (!string.IsNullOrWhiteSpace(rId))
            {
                var receivables = _receivablesRepository.LoadEntities(d => d.Id == rId).FirstOrDefault();
                return PartialView("Detailss", receivables.BusinessPayees);
            }
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("Detail", entity);

        }
        public ActionResult Add(string name)
        {
            BusinessWriteOffView viewModel = new BusinessWriteOffView();
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
            ViewBag.Name = name;
            return View(viewModel);
        }
        /// <summary>
        /// 新增核销记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(BusinessWriteOffView viewModel)
        {
            //校验数据
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "请选择核销款项或核销订单！");
                return View(viewModel);
            }
            var orderIds = viewModel.Orders.Split(',').ToList().Distinct();
            var payeeIds = viewModel.Payees.Split(',').ToList().Distinct();
            BusinessWriteOff businessWriteOff = new BusinessWriteOff();
            businessWriteOff.Id = IdBuilder.CreateIdNum();
            businessWriteOff.WriteOffDate = DateTime.Now;
            businessWriteOff.Transactor = viewModel.Transactor;
            businessWriteOff.TransactorId = viewModel.TransactorId;
            businessWriteOff.AddedBy = CurrentManager.UserName;
            businessWriteOff.AddedById = CurrentManager.Id;
            businessWriteOff.AddedDate = DateTime.Now;
            decimal? orderMoney = 0;
            decimal? payeeMoney = 0;
            List<string> linkman=new List<string>();
            foreach (var orderId in orderIds)
            {
                var order = _businessOrderDetailRepository.LoadEntities(d => d.Id == orderId).FirstOrDefault();
                //未核销 已完成 已采购完成
                if (order.VerificationStatus != Consts.StateNormal && order.Status == Consts.StateOK && IsPurchased(orderId))
                {
                    orderMoney += order.VerificationMoney;
                    order.VerificationStatus = Consts.StateNormal;
                    order.ConfirmVerificationMoney = order.VerificationMoney;
                    order.VerificationMoney = 0;
                    businessWriteOff.BusinessOrderDetails.Add(order);
                    linkman.Add(order.BusinessOrder.LinkManId);
                }

            }
          
            foreach (var payeeId in payeeIds)
            {
                var payee = _businessPayeerepository.LoadEntities(d => d.Id == payeeId).FirstOrDefault();
                //未核销
                if (payee.VerificationStatus != Consts.StateNormal)
                {
                    var verificaitonMoney = payee.Money - payee.BusinessPayments.Where(d => d.AuditStatus == Consts.StateNormal)
                                                .Sum(d => d.PayMoney);
                    payeeMoney += verificaitonMoney;
                    payee.VerificationStatus = Consts.StateNormal;
                    payee.ConfirmVerificationMoney = verificaitonMoney;
                    payee.VerificationMoney = 0;
                    businessWriteOff.BusinessPayees.Add(payee);
                    linkman.Add(payee.LinkManId);
                }

            }

            var temp = linkman.Distinct();
            if (temp.Count()>1)
            {
                ModelState.AddModelError("message", "客户名称不一致，请重新选择！");
                return View(viewModel);
            }
            //校验金额
            if (orderMoney != payeeMoney || orderMoney == 0 || payeeMoney == 0)
            {
                ModelState.AddModelError("message", "核销金额不一致！");
                return View(viewModel);
            }
            businessWriteOff.Money = orderMoney;
            _businessWriteOffService.Add(businessWriteOff);
            TempData["Msg"] = "核销成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            //状态金额恢复
            foreach (var entityBusinessOrder in entity.BusinessOrderDetails)
            {
                entityBusinessOrder.VerificationStatus = Consts.StateLock;
                entityBusinessOrder.VerificationMoney = entityBusinessOrder.ConfirmVerificationMoney;
                entityBusinessOrder.ConfirmVerificationMoney = 0;
            }
            foreach (var entityBusinessPayee in entity.BusinessPayees)
            {
                entityBusinessPayee.VerificationStatus = Consts.StateLock;
                entityBusinessPayee.VerificationMoney = entityBusinessPayee.ConfirmVerificationMoney;
                entityBusinessPayee.ConfirmVerificationMoney = 0;
            }
            entity.BusinessOrderDetails.Clear();
            entity.BusinessPayees.Clear();
            _businessWriteOffService.Delete(entity);//物理删除
            return Json(new { State = 1, Msg = "撤销成功" });
        }

        private bool IsPurchased(string id)
        {
            var purchase = _purchaseOrderDetailRepository.LoadEntities(d => d.BusinessOrderDetailId == id).FirstOrDefault();
            return purchase.Status == Consts.PurchaseStatusSuccess;
        }
    }
}