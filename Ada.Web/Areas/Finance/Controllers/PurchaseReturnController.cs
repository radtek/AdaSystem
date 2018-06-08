using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Finance;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Finance;
using Ada.Framework.Filter;
using Ada.Services.Finance;
using Ada.Services.Purchase;

namespace Finance.Controllers
{
    /// <summary>
    /// 媒介退款
    /// </summary>
    public class PurchaseReturnController : BaseController
    {
        private readonly IRepository<PurchaseReturnOrder> _repository;
        private readonly IReceivablesService _receivablesService;
        private readonly IPurchaseReturnOrderService _service;
        private readonly IRepository<Receivables> _receivablesRepository;
        private readonly IRepository<SettleAccount> _settleAccountrepository;
        private readonly IRepository<PurchaseReturnOrder> _purchaseReturnOrderrepository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseOrderDetailrepository;
        public PurchaseReturnController(IRepository<PurchaseReturnOrder> repository,
            IReceivablesService receivablesService,
            IRepository<SettleAccount> settleAccountrepository,
            IRepository<PurchaseReturnOrder> purchaseReturnOrderrepository,
            IPurchaseReturnOrderService service,
            IRepository<Receivables> receivablesRepository,
            IRepository<PurchaseOrderDetail> purchaseOrderDetailrepository)
        {
            _repository = repository;
            _receivablesService = receivablesService;
            _settleAccountrepository = settleAccountrepository;
            _purchaseReturnOrderrepository = purchaseReturnOrderrepository;
            _service = service;
            _receivablesRepository = receivablesRepository;
            _purchaseOrderDetailrepository = purchaseOrderDetailrepository;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddReceivable(string id)
        {
            var returnorder = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            ReceivablesView viewModel = new ReceivablesView();
            viewModel.BillDate = DateTime.Now;
            viewModel.Money = returnorder.TotalMoney;
            viewModel.ReceivablesType = "P";
            viewModel.RelationshipNum = id;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReceivable(ReceivablesView viewModel)
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
            entity.AccountName = viewModel.AccountName.Trim();
            entity.AccountNum = viewModel.AccountNum;
            entity.Transactor = CurrentManager.UserName;
            entity.TransactorId = CurrentManager.Id;
            entity.Money = viewModel.Money;

            var account = _settleAccountrepository.LoadEntities(d => d.Id == viewModel.SettleAccountId).FirstOrDefault();
            var tax = account.Tax ?? 0;
            decimal money = (decimal)viewModel.Money;
            decimal taxMoney = money - money / (1 + tax / 100);
            entity.TaxMoney = Math.Round(taxMoney);
            entity.IncomeExpendId = viewModel.IncomeExpendId;
            entity.BalanceMoney = entity.Money - entity.TaxMoney;
            entity.SettleAccountId = viewModel.SettleAccountId;
            entity.SettleType = viewModel.SettleType;
            entity.BillNum = IdBuilder.CreateOrderNum("SK");
            entity.BillDate = viewModel.BillDate;
            entity.Remark = viewModel.Remark;
            entity.ReceivablesType = viewModel.ReceivablesType;//收款类型
            entity.RelationshipNum = viewModel.RelationshipNum;//关联单据
            //采购订单减去相应的成本，改变收款状态
            var returnorder = _purchaseReturnOrderrepository.LoadEntities(d => d.Id == entity.RelationshipNum)
                .FirstOrDefault();
            returnorder.Status = Consts.StateNormal;
            _receivablesService.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var returnorder = _purchaseReturnOrderrepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            var receivables = _receivablesRepository.LoadEntities(d => d.IsDelete == false && d.RelationshipNum == id).FirstOrDefault();
            if (receivables == null)
            {
                return Json(new { State = 0, Msg = "不存在的退款收款单据" });
            }

            receivables.DeletedBy = CurrentManager.UserName;
            receivables.DeletedById = CurrentManager.Id;
            receivables.DeletedDate=DateTime.Now;
            receivables.IsDelete = true;
            _service.Delete(returnorder);
            return Json(new { State = 1, Msg = "撤销成功" });
        }
    }
}