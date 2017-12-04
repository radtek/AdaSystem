﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Business;

namespace Business.Controllers
{

    /// <summary>
    /// 领款记录  撤销  查看 请款申请
    /// </summary>
    public class PayeeController : BaseController
    {
        private readonly IBusinessPayeeService _businessPayeeService;
        private readonly IBusinessPaymentService _businessPaymentService;
        private readonly IRepository<BusinessPayee> _repository;
        public PayeeController(IBusinessPayeeService businessPayeeService,
            IRepository<BusinessPayee> repository,
            IBusinessPaymentService businessPaymentService)
        {
            _businessPayeeService = businessPayeeService;
            _repository = repository;
            _businessPaymentService = businessPaymentService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessPayeeView viewModel)
        {
            var result = _businessPayeeService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessPayeeView
                {
                    Id = d.Id,
                    LinkManName = d.LinkManName,
                    Transactor = d.Transactor,
                    Money = d.Money,
                    ClaimDate = d.ClaimDate,
                    VerificationStatus = d.VerificationStatus,
                    VerificationMoney = d.VerificationMoney
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Payment(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            BusinessPaymentView viewModel = new BusinessPaymentView();
            viewModel.BusinessPayeeId = id;
            viewModel.PayMoney = 0;
            viewModel.LinkmanName = entity.LinkManName;
            var temp = entity.BusinessPayments.Sum(d => d.PayMoney);
            viewModel.TotalMoney = entity.Money-temp;
            return View(viewModel);
        }
        public ActionResult Payments(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            var payments = entity.BusinessPayments.Select(d => new BusinessPaymentView
            {
                ApplicationNum = d.ApplicationNum,
                AccountName = d.AccountName,
                AccountBank = d.AccountBank,
                AccountNum = d.AccountNum,
                PayMoney = d.PayMoney,
                AuditStatus = d.AuditStatus,
                PaymentType = d.PaymentType,
                ApplicationDate = d.ApplicationDate

            });
            return PartialView("Payments", payments);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(BusinessPaymentView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.BusinessPayeeId).FirstOrDefault();
            var temp = entity.BusinessPayments.Sum(d => d.PayMoney);
            //校验金额不能超出领款金额
            if (viewModel.PayMoney > entity.Money-temp)
            {
                ModelState.AddModelError("message", "申请金额超出领款金额");
                return View(viewModel);
            }
            BusinessPayment payment = new BusinessPayment();
            payment.Id = IdBuilder.CreateIdNum();
            payment.AccountBank = viewModel.AccountBank;
            payment.AccountName = viewModel.AccountName;
            payment.AccountNum = viewModel.AccountNum;
            payment.ApplicationDate = DateTime.Now;
            payment.AuditStatus = Consts.StateLock;//待审核
            payment.PayMoney = viewModel.PayMoney;
            payment.BusinessPayeeId = viewModel.BusinessPayeeId;
            payment.PaymentType = viewModel.PaymentType;
            payment.Image = viewModel.Image;
            payment.Remark = viewModel.Remark;
            payment.Transactor = CurrentManager.UserName;
            payment.TransactorId = CurrentManager.Id;
            payment.Status = Consts.StateLock;//待付款
            payment.AddedBy = CurrentManager.UserName;
            payment.AddedById = CurrentManager.Id;
            payment.AddedDate = DateTime.Now;
            payment.ApplicationNum = IdBuilder.CreateOrderNum("QK");
            _businessPaymentService.Add(payment);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.BusinessPayments.Count > 0)//如果有申请单，就不能撤销
            {
                return Json(new { State = 0, Msg = "此单据有请款记录，无法撤销" });
            }
            if (entity.BusinessWriteOffs.Count > 0)//如果有核销，就不能撤销
            {
                return Json(new { State = 0, Msg = "此单据有核销记录，无法撤销" });
            }
            entity.Receivables.BalanceMoney = entity.Receivables.BalanceMoney + entity.Money;
            _businessPayeeService.Delete(entity);//物理删除
            return Json(new { State = 1, Msg = "撤销成功" });
        }
    }
}