using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Business;
using Newtonsoft.Json;

namespace Business.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IBusinessOrderService _businessOrderService;
        private readonly IRepository<BusinessOrder> _repository;
        private readonly IRepository<MediaType> _mediaTypeRepository;
        public OrderController(IBusinessOrderService businessOrderService,
            IRepository<BusinessOrder> repository, IRepository<MediaType> mediaTypeRepository
        )
        {
            _businessOrderService = businessOrderService;
            _repository = repository;
            _mediaTypeRepository = mediaTypeRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessOrderView viewModel)
        {
            var result = _businessOrderService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new BusinessOrderView
                {
                    Id = d.Id,
                    OrderNum = d.OrderNum,
                    LinkManName = d.LinkManName,
                    TotalMoney = d.TotalMoney,
                    Transactor = d.Transactor,
                    Status = d.Status,
                    OrderDate = d.OrderDate,
                    TotalSellMoney = d.TotalSellMoney,
                    TotalTaxMoney = d.TotalTaxMoney,
                    DiscountMoney = d.TotalDiscountMoney,
                    AdderBy = d.AddedBy
                    
                    
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            BusinessOrderView viewModel = new BusinessOrderView();
            viewModel.DiscountRate = 100;
            viewModel.Tax = 0;
            viewModel.DiscountMoney = 0;
            viewModel.OrderDate = DateTime.Now;
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(BusinessOrderView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            BusinessOrder order = new BusinessOrder();
            order.Id = IdBuilder.CreateIdNum();
            order.OrderNum = IdBuilder.CreateOrderNum("XD");
            order.Status = Consts.StateLock;
            order.AuditStatus = Consts.StateLock;
            order.AddedBy = CurrentManager.UserName;
            order.AddedById = CurrentManager.Id;
            order.AddedDate=DateTime.Now;
            order.BusinessType = viewModel.BusinessType;
            order.ConfirmVerificationMoney = 0;
            order.LinkManId = viewModel.LinkManId;
            order.LinkManName = viewModel.LinkManName;
            order.Transactor = viewModel.Transactor;
            order.TransactorId = viewModel.TransactorId;
            order.Tax = viewModel.Tax;
            order.DiscountRate = viewModel.DiscountRate;
            order.SettlementType = viewModel.SettlementType;
            order.OrderDate = viewModel.OrderDate;
            order.TotalDiscountMoney = viewModel.DiscountMoney;
            order.VerificationStatus = Consts.StateLock;
            var orderDetails = JsonConvert.DeserializeObject<List<BusinessOrderDetail>>(viewModel.OrderDetails);
            foreach (var businessOrderDetail in orderDetails)
            {
                businessOrderDetail.Id = IdBuilder.CreateIdNum();
                order.BusinessOrderDetails.Add(businessOrderDetail);
            }
            order.TotalTaxMoney = orderDetails.Sum(d => d.TaxMoney);
            order.TotalMoney = orderDetails.Sum(d => d.Money);
            order.TotalSellMoney = orderDetails.Sum(d => d.SellMoney);
            _businessOrderService.Add(order);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }

        public ActionResult SelectMedia()
        {
            //媒体类型
            var mediaTypes = _mediaTypeRepository.LoadEntities(d => d.IsDelete == false).ToList();
            return PartialView("SelectMedia", mediaTypes);
        }
        public ActionResult Update(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            BusinessOrderView entity = new BusinessOrderView();
            //entity.Id = item.Id;
            //entity.CallIndex = item.CallIndex;
            //entity.TypeName = item.TypeName;
            //entity.AdPositions = item.AdPositions.Select(d => d.Name).ToList();
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(MediaTypeView viewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
            //    return View(viewModel);
            //}
            //var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            //entity.ModifiedById = CurrentManager.Id;
            //entity.ModifiedBy = CurrentManager.UserName;
            //entity.ModifiedDate = DateTime.Now;
            //entity.TypeName = viewModel.TypeName;
            //entity.CallIndex = viewModel.CallIndex;
            //_mediaTypeService.Update(entity, viewModel.AdPositions);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            //var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            //entity.DeletedBy = CurrentManager.UserName;
            //entity.DeletedById = CurrentManager.Id;
            //entity.DeletedDate = DateTime.Now;
            //_mediaTypeService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}