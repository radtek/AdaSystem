﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Finance;
using Ada.Framework.Filter;
using Ada.Services.Finance;
using Newtonsoft.Json;

namespace Finance.Controllers
{
    /// <summary>
    /// 销售请款
    /// </summary>
    public class BusinessPayController : BaseController
    {
        private readonly IBillPaymentService _billPaymentService;
        private readonly IRepository<BusinessPayment> _repository;
        public BusinessPayController(IBillPaymentService billPaymentService, IRepository<BusinessPayment> repository)
        {
            _billPaymentService = billPaymentService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Pay(string id)
        {
            var payment = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            BillPaymentView viewModel = new BillPaymentView();
            viewModel.BillDate = DateTime.Now;
            viewModel.AccountBank = payment.AccountBank;
            viewModel.AccountName = payment.AccountName;
            viewModel.AccountNum = payment.AccountNum;
            viewModel.PayMoney = payment.PayMoney;
            viewModel.RequestNum = payment.ApplicationNum;
            viewModel.PaymentType = payment.PaymentType;
            return View(viewModel);
        }
        [HttpPost]
        
        public ActionResult Pay(BillPaymentView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var payDetails = JsonConvert.DeserializeObject<List<BillPaymentDetail>>(viewModel.PayDetails);
            if (payDetails.Count <= 0)
            {
                ModelState.AddModelError("message", "请录入结算账户！");
                return View(viewModel);
            }
            var payment = _repository.LoadEntities(d => d.ApplicationNum == viewModel.RequestNum).FirstOrDefault();
            if (payment.Status == Consts.StateNormal)
            {
                ModelState.AddModelError("message", "此请款已生成付款单据！");
                return View(viewModel);
            }
            BillPayment entity = new BillPayment();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedById = CurrentManager.Id;
            entity.Transactor = CurrentManager.UserName;
            entity.TransactorId = CurrentManager.Id;
            entity.AccountBank = viewModel.AccountBank;
            entity.AccountName = viewModel.AccountName;
            entity.AccountNum = viewModel.AccountNum;
            entity.BillDate = viewModel.BillDate;
            entity.BillNum = IdBuilder.CreateOrderNum("FK");
            entity.Image = viewModel.Image;
            entity.LinkManId = payment.BusinessPayee.LinkManId;
            entity.LinkManName = payment.BusinessPayee.LinkManName;
            entity.PaymentType = viewModel.PaymentType;
            entity.RequestNum = viewModel.RequestNum;
            entity.RequestType = Consts.StateNormal;//销售付款单
            decimal? money = 0;
            foreach (var billPaymentDetail in payDetails)
            {
                if (string.IsNullOrWhiteSpace(billPaymentDetail.IncomeExpendId) && string.IsNullOrWhiteSpace(billPaymentDetail.SettleAccountId))
                {
                    ModelState.AddModelError("message", "收入项目或结算账户不能为空！");
                    return View(viewModel);
                }
                billPaymentDetail.Id = IdBuilder.CreateIdNum();
                billPaymentDetail.AddedBy = CurrentManager.UserName;
                billPaymentDetail.AddedById = CurrentManager.Id;
                billPaymentDetail.AddedDate = DateTime.Now;
                money += billPaymentDetail.Money;
                entity.BillPaymentDetails.Add(billPaymentDetail);
            }
            if (money > payment.PayMoney)
            {
                ModelState.AddModelError("message", "付款金额已超出申请金额！");
                return View(viewModel);
            }
            //修改付款状态
            payment.Status = Consts.StateNormal;
            _billPaymentService.Add(entity);
            TempData["Msg"] = "付款成功";
            return RedirectToAction("Index");
        }
    }
}