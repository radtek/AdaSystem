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
    public class WriteOffController : BaseController
    {
        private readonly IBusinessWriteOffService _businessWriteOffService;
        private readonly IRepository<BusinessWriteOff> _repository;

        public WriteOffController(IBusinessWriteOffService businessWriteOffService, IRepository<BusinessWriteOff> repository)
        {
            _businessWriteOffService = businessWriteOffService;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessWriteOffView viewModel)
        {
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
        public ActionResult Add()
        {
            BusinessWriteOffView viewModel=new BusinessWriteOffView();
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
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.BusinessOrders.Clear();
            entity.BusinessPayees.Clear();
            //状态金额恢复
            //_businessWriteOffService.Delete(entity);//物理删除
            return Json(new { State = 1, Msg = "撤销成功" });
        }
    }
}