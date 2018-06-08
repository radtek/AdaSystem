using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Purchase;
using Ada.Framework.Filter;
using Ada.Services.Purchase;

namespace Boss.Controllers
{
    /// <summary>
    /// 媒介退款
    /// </summary>
    public class PurchaseReturnController : BaseController
    {
        private readonly IRepository<PurchaseReturnOrder> _repository;
        private readonly IPurchaseReturnOrderService _service;

        public PurchaseReturnController(IRepository<PurchaseReturnOrder> repository, IPurchaseReturnOrderService service)
        {
            _repository = repository;
            _service = service;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Audit(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Audit(PurchaseReturnOrderView viewModel)
        {
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.AuditStatus = viewModel.AuditStatus;
            entity.AuditBy = CurrentManager.UserName;
            entity.AuditById = CurrentManager.Id;
            entity.AuditDate = DateTime.Now;
            _service.Update(entity);
            TempData["Msg"] = "审批成功";
            return RedirectToAction("Index");
        }
    }
}