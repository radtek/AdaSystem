using System;
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
    /// 核销
    /// </summary>
    public class WriteOffController : BaseController
    {
        private readonly IBusinessWriteOffService _businessWriteOffService;
        private readonly IRepository<BusinessWriteOff> _repository;
        private readonly IRepository<BusinessOrder> _businessOrderrepository;
        private readonly IRepository<BusinessPayee> _businessPayeerepository;
        public WriteOffController(IBusinessWriteOffService businessWriteOffService,
            IRepository<BusinessWriteOff> repository,
            IRepository<BusinessOrder> businessOrderrepository,
            IRepository<BusinessPayee> businessPayeerepository)
        {
            _businessWriteOffService = businessWriteOffService;
            _repository = repository;
            _businessOrderrepository = businessOrderrepository;
            _businessPayeerepository = businessPayeerepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessWriteOffView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _businessWriteOffService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessWriteOffView
                {
                    Id = d.Id,
                    LinkManName = d.BusinessOrders.FirstOrDefault()?.LinkManName,
                    Transactor = d.Transactor,
                    Money = d.Money,
                    WriteOffDate = d.WriteOffDate
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return PartialView("Detail",entity);
        }
        public ActionResult Add()
        {
            BusinessWriteOffView viewModel = new BusinessWriteOffView();
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
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
            var orderIds = viewModel.Orders.Split(',');
            var payeeIds = viewModel.Payees.Split(',');
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
            foreach (var orderId in orderIds)
            {
                var order = _businessOrderrepository.LoadEntities(d => d.Id == orderId).FirstOrDefault();
                orderMoney += order.VerificationMoney;
                businessWriteOff.BusinessOrders.Add(order);
                order.VerificationStatus = Consts.StateNormal;
                order.ConfirmVerificationMoney = order.VerificationMoney;
                order.VerificationMoney = 0;
            }
            foreach (var payeeId in payeeIds)
            {
                var payee = _businessPayeerepository.LoadEntities(d => d.Id == payeeId).FirstOrDefault();
                if (payee.BusinessPayments.Count>0)
                {
                    //校验，请款是否都审核通过了
                    if (payee.BusinessPayments.Count(d => d.AuditStatus != Consts.StateNormal)>0)
                    {
                        ModelState.AddModelError("message", "销账款项中存在请款还未审核通过的，请审核通过后再进行核销！");
                        return View(viewModel);
                    }
                }
                payeeMoney += payee.VerificationMoney;
                businessWriteOff.BusinessPayees.Add(payee);
                payee.VerificationStatus = Consts.StateNormal;
                payee.ConfirmVerificationMoney = payee.VerificationMoney;
                payee.VerificationMoney = 0;
            }
            //校验金额
            if (orderMoney!=payeeMoney)
            {
                ModelState.AddModelError("message", "核销款项和订单金额不一致！");
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
            foreach (var entityBusinessOrder in entity.BusinessOrders)
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
            entity.BusinessOrders.Clear();
            entity.BusinessPayees.Clear();
           _businessWriteOffService.Delete(entity);//物理删除
            return Json(new { State = 1, Msg = "撤销成功" });
        }
    }
}