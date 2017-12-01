using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
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
        private readonly IRepository<BusinessPayee> _repository;
        public PayeeController(IBusinessPayeeService businessPayeeService, IRepository<BusinessPayee> repository)
        {
            _businessPayeeService = businessPayeeService;
            _repository = repository;
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
            BusinessPaymentView viewModel=new BusinessPaymentView();
            viewModel.BusinessPayeeId = id;
            viewModel.PayMoney = 0;
            viewModel.LinkmanName = entity.LinkManName;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(BusinessPaymentView viewModel)
        {
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.BusinessPayments.Count>0)//如果有申请单，就不能撤销
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