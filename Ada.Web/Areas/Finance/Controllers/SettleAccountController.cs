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
    public class SettleAccountController : BaseController
    {
        private readonly ISettleAccountService _settleAccountService;
        private readonly IRepository<SettleAccount> _repository;
        public SettleAccountController(ISettleAccountService settleAccountService, IRepository<SettleAccount> repository)
        {
            _settleAccountService = settleAccountService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList()
        {
            var result = _repository.LoadEntities(d=>d.IsDelete==false).ToList();
            return Json(new
            {
                total= result.Count,
                rows = result.Select(d => new SettleAccountView
                {
                    Id = d.Id,
                    SettleName = d.SettleName,
                    AccountBank = d.AccountBank,
                    AccountName = d.AccountName,
                    AccountNum = d.AccountNum,
                    Money = d.Money,
                    Tax = d.Tax
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            SettleAccountView viewModel=new SettleAccountView();
            viewModel.Money = 0;
            viewModel.Tax = 0;
            return View(viewModel);
        }
        [HttpPost]
        
        public ActionResult Add(SettleAccountView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }

            SettleAccount entity = new SettleAccount();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedById = CurrentManager.Id;
            entity.AddedDate = DateTime.Now;
            entity.AccountName = viewModel.AccountName;
            entity.SettleName = viewModel.SettleName;
            entity.AccountBank = viewModel.AccountBank;
            entity.AccountNum = viewModel.AccountNum;
            entity.Money = viewModel.Money;
            entity.Tax = viewModel.Tax;
            _settleAccountService.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            SettleAccountView viewModel = new SettleAccountView();
            viewModel.Id = id;
            viewModel.AccountName = entity.AccountName;
            viewModel.SettleName = entity.SettleName;
            viewModel.AccountBank = entity.AccountBank;
            viewModel.AccountNum = entity.AccountNum;
            viewModel.Money = entity.Money;
            viewModel.Tax = entity.Tax;
            return View(viewModel);
        }
        [HttpPost]
        
        public ActionResult Update(SettleAccountView viewModel)
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
            entity.AccountName = viewModel.AccountName;
            entity.SettleName = viewModel.SettleName;
            entity.AccountBank = viewModel.AccountBank;
            entity.AccountNum = viewModel.AccountNum;
            entity.Money = viewModel.Money;
            entity.Tax = viewModel.Tax;
            _settleAccountService.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _settleAccountService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}