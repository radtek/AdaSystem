﻿using System;
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
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace Business.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IBusinessOrderService _businessOrderService;
        private readonly IRepository<BusinessOrder> _repository;
        private readonly IRepository<BusinessOrderDetail> _businessOrderDetailRepository;
        private readonly IRepository<MediaType> _mediaTypeRepository;
        public OrderController(IBusinessOrderService businessOrderService,
            IRepository<BusinessOrder> repository, IRepository<MediaType> mediaTypeRepository, IRepository<BusinessOrderDetail> businessOrderDetailRepository
        )
        {
            _businessOrderService = businessOrderService;
            _repository = repository;
            _mediaTypeRepository = mediaTypeRepository;
            _businessOrderDetailRepository = businessOrderDetailRepository;
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
            var orderDetails = JsonConvert.DeserializeObject<List<BusinessOrderDetail>>(viewModel.OrderDetails);
            if (orderDetails.Count<=0)
            {
                ModelState.AddModelError("message", "请录入订单明细！");
                return View(viewModel);
            }
            BusinessOrder entity = new BusinessOrder();
            entity.Id = IdBuilder.CreateIdNum();
            entity.OrderNum = IdBuilder.CreateOrderNum("XD");
            entity.Status = Consts.StateLock;
            entity.AuditStatus = Consts.StateLock;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedById = CurrentManager.Id;
            entity.AddedDate=DateTime.Now;
            entity.BusinessType = viewModel.BusinessType;
            entity.ConfirmVerificationMoney = 0;
            entity.LinkManId = viewModel.LinkManId;
            entity.LinkManName = viewModel.LinkManName;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            entity.Tax = viewModel.Tax;
            entity.DiscountRate = viewModel.DiscountRate;
            entity.SettlementType = viewModel.SettlementType;
            entity.OrderDate = viewModel.OrderDate;
            entity.TotalDiscountMoney = viewModel.DiscountMoney;
            entity.VerificationStatus = Consts.StateLock;
            foreach (var businessOrderDetail in orderDetails)
            {
                businessOrderDetail.Id = IdBuilder.CreateIdNum();
                entity.BusinessOrderDetails.Add(businessOrderDetail);
            }
            entity.TotalTaxMoney = orderDetails.Sum(d => d.TaxMoney);
            entity.TotalMoney = orderDetails.Sum(d => d.Money);
            entity.TotalSellMoney = orderDetails.Sum(d => d.SellMoney);
            _businessOrderService.Add(entity);
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
            entity.Id = item.Id;
            entity.OrderNum = item.OrderNum;
            entity.BusinessType = item.BusinessType;
            entity.LinkManId = item.LinkManId;
            entity.LinkManName = item.LinkManName;
            entity.Transactor = item.Transactor;
            entity.TransactorId = item.TransactorId;
            entity.Tax = item.Tax;
            entity.DiscountRate = item.DiscountRate;
            entity.SettlementType = item.SettlementType;
            entity.DiscountMoney = item.TotalDiscountMoney;
            entity.OrderDate = item.OrderDate;
            entity.Remark = item.Remark;
            var orderDetails = item.BusinessOrderDetails.Where(d=>d.IsDelete==false).Select(d => new
            {
                d.Id,
                d.MediaTypeName,
                d.MediaName,
                d.AdPositionName,
                d.MediaTitle,
                PrePublishDate=d.PrePublishDate.IfNotNull(t=>t.Value.ToString("yyyy-MM-dd"),DateTime.Now.ToString("yyyy-MM-dd")),
                d.SellMoney,
                d.Money,
                d.Tax,
                d.TaxMoney,
                d.MediaPriceId
            });
            entity.OrderDetails = JsonConvert.SerializeObject(orderDetails);

            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(BusinessOrderView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var orderDetails = JsonConvert.DeserializeObject<List<BusinessOrderDetail>>(viewModel.OrderDetails);
            if (orderDetails.Count <= 0)
            {
                ModelState.AddModelError("message", "请录入订单明细！");
                return View(viewModel);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.BusinessType = viewModel.BusinessType;
            entity.LinkManId = viewModel.LinkManId;
            entity.LinkManName = viewModel.LinkManName;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            entity.Tax = viewModel.Tax;
            entity.DiscountRate = viewModel.DiscountRate;
            entity.SettlementType = viewModel.SettlementType;
            entity.OrderDate = viewModel.OrderDate;
            entity.TotalDiscountMoney = viewModel.DiscountMoney;
            //删除已剔除的
            var ids = orderDetails.Select(d => d.Id).ToList();
            foreach (var entityBusinessOrderDetail in entity.BusinessOrderDetails)
            {
                if (!ids.Contains(entityBusinessOrderDetail.Id))
                {
                    entityBusinessOrderDetail.IsDelete = true;
                }
            }
            //更新，增加
            foreach (var businessOrderDetail in orderDetails)
            {
                if (string.IsNullOrWhiteSpace(businessOrderDetail.Id))
                {
                    businessOrderDetail.Id = IdBuilder.CreateIdNum();
                    entity.BusinessOrderDetails.Add(businessOrderDetail);
                }
                else
                {
                    //更新
                    var detail = _businessOrderDetailRepository.LoadEntities(d => d.Id == businessOrderDetail.Id)
                        .FirstOrDefault();
                    detail.MediaPriceId = businessOrderDetail.MediaPriceId;
                    detail.MediaTitle = businessOrderDetail.MediaTitle;
                    detail.PrePublishDate = businessOrderDetail.PrePublishDate;
                    detail.Money = businessOrderDetail.Money;
                    detail.SellMoney = businessOrderDetail.SellMoney;
                    detail.Tax = businessOrderDetail.Tax;
                    detail.TaxMoney = businessOrderDetail.TaxMoney;
                    detail.AdPositionName = businessOrderDetail.AdPositionName;
                    detail.MediaName = businessOrderDetail.MediaName;
                    detail.MediaTypeName = businessOrderDetail.MediaTypeName;
                }
            }
            entity.TotalTaxMoney = orderDetails.Sum(d => d.TaxMoney);
            entity.TotalMoney = orderDetails.Sum(d => d.Money);
            entity.TotalSellMoney = orderDetails.Sum(d => d.SellMoney);
            _businessOrderService.Update(entity);
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