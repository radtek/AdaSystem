using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Business;

namespace Business.Controllers
{
    public class ReceivablesController : BaseController
    {

        private readonly IBusinessPayeeService _businessPayeeService;
        private readonly IRepository<Receivables> _receivablesRepository;
        public ReceivablesController(
            IRepository<Receivables> receivablesRepository,
            IBusinessPayeeService businessPayeeService
        )
        {
            _receivablesRepository = receivablesRepository;
            _businessPayeeService = businessPayeeService;
        }
        /// <summary>
        /// 收款记录（领款）
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 领款
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ReceivedView(string id)
        {
            var receivables = _receivablesRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            BusinessPayeeView entity = new BusinessPayeeView();
            entity.ReceivablesId = receivables.Id;
            entity.TotalMoney = receivables.BalanceMoney;
            entity.Money = 0;
            return PartialView("Received", entity);
        }
        /// <summary>
        /// 领款
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Received(BusinessPayeeView viewModel)
        {

            if (!ModelState.IsValid)
            {
                return Json(new { State = 0, Msg = "数据校验失败，请核对输入的信息是否准确" });
            }
            //校验金额
            var receivables = _receivablesRepository.LoadEntities(d => d.Id == viewModel.ReceivablesId).FirstOrDefault();
            if (viewModel.Money > receivables.BalanceMoney || viewModel.Money <= 0)
            {
                return Json(new { State = 0, Msg = "领款金额超出可领金额" });
            }
            BusinessPayee entity = new BusinessPayee();
            entity.Id = IdBuilder.CreateIdNum();
            entity.ClaimDate = DateTime.Now;
            entity.Transactor = CurrentManager.UserName;
            entity.TransactorId = CurrentManager.Id;
            entity.LinkManId = viewModel.LinkManId;
            entity.LinkManName = viewModel.LinkManName;
            entity.ConfirmVerificationMoney = 0;
            entity.VerificationMoney = viewModel.Money;
            entity.VerificationStatus = Consts.StateLock;
            entity.Money = viewModel.Money;
            entity.ReceivablesId = viewModel.ReceivablesId;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedById = CurrentManager.Id;
            entity.AddedDate = DateTime.Now;
            receivables.BalanceMoney = receivables.BalanceMoney - viewModel.Money;
            _businessPayeeService.Add(entity);
            return Json(new { State = 1, Msg = "领款成功" });
        }
    }
}