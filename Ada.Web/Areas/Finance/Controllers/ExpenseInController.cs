﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Finance;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Finance;
using Ada.Framework.Filter;
using Ada.Services.Finance;
using Newtonsoft.Json;

namespace Finance.Controllers
{
    /// <summary>
    /// 费用收入
    /// </summary>
    public class ExpenseInController : BaseController
    {
        private readonly IExpenseService _expenseService;
        private readonly IRepository<Expense> _repository;

        public ExpenseInController(IExpenseService expenseService,
            IRepository<Expense> repository)
        {
            _expenseService = expenseService;
            _repository = repository;

        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(ExpenseView viewModel)
        {
            var result = _expenseService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new ExpenseView
                {
                    Id = d.Id,
                    AccountBank = d.AccountBank,
                    AccountName = d.AccountName,
                    AccountNum = d.AccountNum,
                    Transactor = d.Transactor,
                    LinkManName = d.LinkManName,
                    Employe = d.Employe,
                    BillNum = d.BillNum,
                    BillDate = d.BillDate,
                    Money = d.ExpenseDetails.Sum(e=>e.Money)

                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            ExpenseView viewModel = new ExpenseView();
            viewModel.BillDate = DateTime.Now;
            viewModel.Money = 0;
            viewModel.IsIncom = true;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ExpenseView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var payDetails = JsonConvert.DeserializeObject<List<ExpenseDetail>>(viewModel.Details);
            if (payDetails.Count <= 0)
            {
                ModelState.AddModelError("message", "请录入结算账户！");
                return View(viewModel);
            }
            Expense entity = new Expense();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedById = CurrentManager.Id;
            entity.AddedDate = DateTime.Now;
            entity.AccountBank = viewModel.AccountBank;
            entity.AccountName = viewModel.AccountName;
            entity.AccountNum = viewModel.AccountNum;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            entity.Employe = viewModel.Employe;
            entity.EmployerId = viewModel.EmployerId;
            entity.LinkManName = viewModel.LinkManName;
            entity.LinkManId = viewModel.LinkManId;
            entity.IsIncom = true;
            entity.Image = viewModel.Image;
            entity.BillNum = IdBuilder.CreateOrderNum("SR");
            entity.BillDate = viewModel.BillDate;
            entity.Remark = viewModel.Remark;
            foreach (var detail in payDetails)
            {
                if (string.IsNullOrWhiteSpace(detail.IncomeExpendId) && string.IsNullOrWhiteSpace(detail.SettleAccountId))
                {
                    ModelState.AddModelError("message", "收入项目或结算账户不能为空！");
                    return View(viewModel);
                }
                detail.Id = IdBuilder.CreateIdNum();
                detail.AddedBy = CurrentManager.UserName;
                detail.AddedById = CurrentManager.Id;
                detail.AddedDate = DateTime.Now;
                entity.ExpenseDetails.Add(detail);
            }
            _expenseService.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            ExpenseView viewModel = new ExpenseView();
            viewModel.Id = id;
            viewModel.AccountBank = entity.AccountBank;
            viewModel.AccountName = entity.AccountName;
            viewModel.AccountNum = entity.AccountNum;
            viewModel.BillNum = entity.BillNum;
            viewModel.BillDate = entity.BillDate;
            viewModel.Remark = entity.Remark;
            viewModel.ThumbnailImage = Thumbnail.MakeThumbnailImageToBase64(Utils.GetMapPath(entity.Image));
            var paydetails = entity.ExpenseDetails.Where(d => d.IsDelete == false).Select(d => new
            {
                d.Id,
                d.SettleAccountId,
                d.IncomeExpendId,
                d.Money
            });
            viewModel.Details = JsonConvert.SerializeObject(paydetails);
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(ExpenseView viewModel)
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
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            entity.Employe = viewModel.Employe;
            entity.EmployerId = viewModel.EmployerId;
            entity.LinkManName = viewModel.LinkManName;
            entity.LinkManId = viewModel.LinkManId;
            entity.Image = viewModel.Image;

            entity.BillDate = viewModel.BillDate;
            entity.Remark = viewModel.Remark;
            _expenseService.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _expenseService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}