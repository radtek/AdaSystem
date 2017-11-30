using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Finance;
using Ada.Framework.Filter;
using Ada.Services.Finance;

namespace Finance.Controllers
{
    public class ReceivablesController : BaseController
    {
        private readonly IReceivablesService _receivablesService;
        private readonly IRepository<Receivables> _repository;
        public ReceivablesController(IReceivablesService receivablesService, IRepository<Receivables> repository)
        {
            _receivablesService = receivablesService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(ReceivablesView viewModel)
        {
            var result = _receivablesService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new ReceivablesView
                {
                    Id = d.Id,
                    AccountBank = d.AccountBank,
                    AccountName = d.AccountName,
                    AccountNum = d.AccountNum,
                    Transactor = d.Transactor,
                    Money = d.Money,
                    BalanceMoney = d.BalanceMoney,
                    TaxMoney = d.TaxMoney,
                    IncomeExpendName = d.IncomeExpend.SubjectName,
                    SettleAccountName = d.SettleAccount.SettleName,
                    BillNum = d.BillNum,
                    BillDate = d.BillDate
                  
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            ReceivablesView viewModel = new ReceivablesView();
            viewModel.BillDate=DateTime.Now;
            viewModel.Money = 0;
            viewModel.TaxMoney = 0;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ReceivablesView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }

            Receivables entity = new Receivables();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedById = CurrentManager.Id;
            entity.AddedDate = DateTime.Now;
            entity.AccountBank = viewModel.AccountBank;
            entity.AccountName = viewModel.AccountName;
            entity.AccountNum = viewModel.AccountNum;
            entity.Transactor = CurrentManager.UserName;
            entity.TransactorId = CurrentManager.Id;
            entity.Money = viewModel.Money;
            entity.BalanceMoney = viewModel.Money;
            entity.TaxMoney = viewModel.TaxMoney;
            entity.IncomeExpendId = viewModel.IncomeExpendId;
            entity.IncomeExpendName = viewModel.IncomeExpendName;
            entity.SettleAccountName = viewModel.SettleAccountName;
            entity.SettleAccountId = viewModel.SettleAccountId;
            entity.SettleType = viewModel.SettleType;
            entity.BillNum = IdBuilder.CreateOrderNum("SK");
            entity.BillDate = viewModel.BillDate;
            entity.Remark = viewModel.Remark;
            _receivablesService.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            ReceivablesView viewModel = new ReceivablesView();
            viewModel.Id = id;
            viewModel.AccountBank = entity.AccountBank;
            viewModel.AccountName = entity.AccountName;
            viewModel.AccountNum = entity.AccountNum;
            viewModel.Money = entity.Money;
            viewModel.BalanceMoney = entity.Money;
            viewModel.TaxMoney = entity.TaxMoney;
            viewModel.IncomeExpendId = entity.IncomeExpendId;
            viewModel.IncomeExpendName = entity.IncomeExpendName;
            viewModel.SettleAccountName = entity.SettleAccountName;
            viewModel.SettleAccountId = entity.SettleAccountId;
            viewModel.SettleType = entity.SettleType;
            viewModel.BillNum = entity.BillNum;
            viewModel.BillDate = entity.BillDate;
            viewModel.Remark = entity.Remark;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(ReceivablesView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }

            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedDate = DateTime.Now;
            entity.AccountBank = viewModel.AccountBank;
            entity.AccountName = viewModel.AccountName;
            entity.AccountNum = viewModel.AccountNum;
            entity.Money = viewModel.Money;
            entity.BalanceMoney = viewModel.Money;
            entity.TaxMoney = viewModel.TaxMoney;
            entity.IncomeExpendId = viewModel.IncomeExpendId;
            entity.IncomeExpendName = viewModel.IncomeExpendName;
            entity.SettleAccountName = viewModel.SettleAccountName;
            entity.SettleAccountId = viewModel.SettleAccountId;
            entity.SettleType = viewModel.SettleType;
            entity.BillDate = viewModel.BillDate;
            entity.Remark = viewModel.Remark;
            _receivablesService.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _receivablesService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}