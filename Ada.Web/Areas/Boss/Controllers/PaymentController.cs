using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Business;

namespace Boss.Controllers
{
    /// <summary>
    /// 请款  审批 删除 修改
    /// </summary>
    public class PaymentController : BaseController
    {
        private readonly IBusinessPaymentService _businessPaymentService;
        private readonly IRepository<BusinessPayment> _repository;

        public PaymentController(IBusinessPaymentService businessPaymentService, IRepository<BusinessPayment> repository)
        {
            _businessPaymentService = businessPaymentService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessPaymentView viewModel)
        {
            var result = _businessPaymentService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessPaymentView
                {
                    Id = d.Id,
                    AccountBank = d.AccountBank,
                    Transactor = d.Transactor,
                    AccountName = d.AccountName,
                    AccountNum = d.AccountNum,
                    PayMoney = d.PayMoney,
                    ApplicationNum = d.ApplicationNum,
                    PaymentType = d.PaymentType,
                    ApplicationDate = d.ApplicationDate,
                    LinkmanName = d.BusinessPayee.LinkManName
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Audit(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            BusinessPaymentView viewModel=new BusinessPaymentView();
            viewModel.Transactor = entity.Transactor;
            viewModel.Id = entity.Id;
            viewModel.ApplicationDate = entity.ApplicationDate;
            viewModel.AccountBank = entity.AccountBank;
            viewModel.AccountName = entity.AccountName;
            viewModel.AccountNum = entity.AccountNum;
            viewModel.PaymentType = entity.PaymentType;
            viewModel.PayMoney = entity.PayMoney;
            viewModel.AuditStatus = entity.AuditStatus;
            viewModel.ApplicationNum = entity.ApplicationNum;
            viewModel.Image = entity.Image;
            viewModel.Remark = entity.Remark;
            ViewBag.LinkMan = entity.BusinessPayee.LinkMan;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Audit(BusinessPaymentView viewModel)
        {
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.Remark = viewModel.Remark;
            entity.AuditStatus = viewModel.AuditStatus;
            entity.AuditBy = CurrentManager.UserName;
            entity.AuditById = CurrentManager.Id;
            entity.AuditDate=DateTime.Now;
            //通过就增加客户账户信息
            if (entity.AuditStatus==Consts.StateNormal)
            {
                //判断是否增加账户
                var account = entity.BusinessPayee.LinkMan.PayAccounts.FirstOrDefault(d =>
                    d.AccountName == viewModel.AccountName && d.AccountNum == viewModel.AccountNum && d.IsDelete==false);
                if (account==null)
                {
                    PayAccount payAccount=new PayAccount();
                    payAccount.Id = IdBuilder.CreateIdNum();
                    payAccount.AccountName = viewModel.AccountName;
                    payAccount.AccountNum = viewModel.AccountNum;
                    payAccount.AccountType = viewModel.AccountBank;
                    payAccount.AddedBy = CurrentManager.UserName;
                    payAccount.AddedById = CurrentManager.Id;
                    payAccount.AddedDate=DateTime.Now;
                    entity.BusinessPayee.LinkMan.PayAccounts.Add(payAccount);
                }
            }
            _businessPaymentService.Update(entity);
            TempData["Msg"] = "审批成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _businessPaymentService.Delete(entity);//物理删除
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}