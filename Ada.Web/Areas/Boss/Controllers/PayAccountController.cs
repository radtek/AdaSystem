using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;
using Ada.Framework.Filter;
using Ada.Services.Customer;

namespace Boss.Controllers
{
    public class PayAccountController : BaseController
    {
        private readonly IPayAccountService _payAccountService;
        private readonly IRepository<PayAccount> _repository;
        public PayAccountController(IPayAccountService payAccountService, IRepository<PayAccount> repository)
        {
            _payAccountService = payAccountService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(PayAccountView viewModel)
        {
          var result=  _payAccountService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new PayAccountView
                {
                    Id = d.Id,
                    AccountName = d.AccountName,
                    LinkManName = d.LinkMan.Name,
                    Remark = d.Remark,
                    CompanyName = d.LinkMan.Commpany.Name,
                    Status = d.Status,
                    Transactor = d.LinkMan.Transactor
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Audit(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Audit(PayAccountView viewModel)
        {
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.Remark = "收款账户";
            entity.Status = viewModel.Status;
            _payAccountService.Update(entity);
            TempData["Msg"] = "审批成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _payAccountService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}