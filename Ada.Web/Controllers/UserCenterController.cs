﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Resource;
using Ada.Core.ViewModel.Statistics;
using Ada.Framework.Filter;
using Ada.Services.Cache;
using Ada.Services.Resource;
using Ada.Web.Models;

namespace Ada.Web.Controllers
{
    public class UserCenterController : UserController
    {
        private readonly IRepository<BusinessOrderDetail> _businessRepository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseRepository;
        private readonly IRepository<MediaGroup> _mediaGroupRepository;
        private readonly ICacheService _cacheService;
        private readonly IMediaGroupService _mediaGroupService;

        public UserCenterController(IRepository<BusinessOrderDetail> businessRepository,
            IRepository<PurchaseOrderDetail> purchaseRepository, ICacheService cacheService,
            IRepository<MediaGroup> mediaGroupRepository,
            IMediaGroupService mediaGroupService)
        {
            _businessRepository = businessRepository;
            _purchaseRepository = purchaseRepository;
            _cacheService = cacheService;
            _mediaGroupRepository = mediaGroupRepository;
            _mediaGroupService = mediaGroupService;
        }
        public ActionResult Order()
        {
            BusinessTotal viewModel = new BusinessTotal();
            var userId = CurrentUser.Id;

            var business =
                _businessRepository.LoadEntities(d => d.BusinessOrder.IsDelete == false && d.IsDelete == false && d.BusinessOrder.LinkManId == userId);
            var purchase = _purchaseRepository.LoadEntities(d => d.IsDelete == false);
            var temp = business.Where(d => d.Status != 0);
            viewModel.OrderCount = temp.Count();
            viewModel.Waiting = business.Count(d => d.Status == Consts.StateLock);
            viewModel.Doing = (from b in business
                               from p in purchase
                               where b.Id == p.BusinessOrderDetailId && p.Status == Consts.PurchaseStatusTodo
                               select b).Count();
            viewModel.Confirm = (from b in business
                                 from p in purchase
                                 where b.Id == p.BusinessOrderDetailId && p.Status == Consts.PurchaseStatusConfirm
                                 select b).Count();
            //viewModel.Done = (from b in business
            //    from p in purchase
            //    where b.Id == p.BusinessOrderDetailId && p.Status == Consts.PurchaseStatusSuccess && b.Status == Consts.StateNormal
            //    select b).Count();
            var start = DateTime.Now.Date;
            var end = start.AddDays(1);
            viewModel.Today = business.Count(d => d.PrePublishDate >= start && d.PrePublishDate < end);
            var start1 = DateTime.Now.Date.AddDays(1);
            var end1 = start1.AddDays(1);
            viewModel.Tomorrow = business.Count(d => d.PrePublishDate >= start1 && d.PrePublishDate < end1);
            return View(viewModel);
        }

        public ActionResult GetList(BusinessOrderDetailView viewModel)
        {
            var userId = CurrentUser.Id;
            var purchaseOrderDetails = _purchaseRepository.LoadEntities(d => d.IsDelete == false);
            var allList = _businessRepository.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false && d.BusinessOrder.LinkManId == userId);
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaName))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.MediaName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.OrderNum))
            {
                allList = allList.Where(d => d.BusinessOrder.OrderNum == viewModel.OrderNum);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Remark))
            {
                allList = allList.Where(d => d.BusinessOrder.Remark.Contains(viewModel.Remark));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeId))
            {
                allList = allList.Where(d => d.MediaPrice.Media.MediaTypeId == viewModel.MediaTypeId);
            }
            if (viewModel.Status != null)
            {
                allList = allList.Where(d => d.Status == viewModel.Status);
            }
            if (viewModel.PurchaseStatus != null)
            {

                allList = from b in allList
                          from p in purchaseOrderDetails
                          where b.Id == p.BusinessOrderDetailId && p.Status == viewModel.PurchaseStatus
                          select b;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.PublishDateStr))
            {
                var temp = viewModel.PublishDateStr.Replace("至", "#").Split('#');
                var start = DateTime.Parse(temp[0].Trim());
                var end = DateTime.Parse(temp[1].Trim()).AddDays(1);
                allList = from b in allList
                          from p in purchaseOrderDetails
                          where b.Id == p.BusinessOrderDetailId && p.PublishDate >= start && p.PublishDate < end
                          select b;
            }
            var result = from b in allList
                         from p in purchaseOrderDetails
                         where b.Id == p.BusinessOrderDetailId
                         select new BusinessOrderDetailView()
                         {
                             Id = b.Id,
                             BusinessOrderId = b.BusinessOrderId,
                             MediaName = b.MediaName,
                             MediaTypeName = b.MediaTypeName,
                             AdPositionName = b.AdPositionName,
                             Status = b.Status,
                             Transactor = b.BusinessOrder.Transactor,
                             OrderDate = b.BusinessOrder.OrderDate,
                             OrderNum = b.BusinessOrder.OrderNum,
                             OrderRemark = b.BusinessOrder.Remark,
                             PrePublishDate = b.PrePublishDate,
                             PurchaseStatus = p.Status,
                             PublishDate = p.PublishDate,
                             PublishLink = p.PublishLink,
                         };

            viewModel.total = result.Count();
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            result = order == "desc" ? result.OrderByDescending(d => d.Id).Skip(offset).Take(rows) : result.OrderBy(d => d.Id).Skip(offset).Take(rows);
            return Json(new
            {
                viewModel.total,
                rows = result.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Group()
        {
            var groups = _mediaGroupRepository.LoadEntities(d => d.AddedById == CurrentUser.Id).ToList();
            return View(groups);
        }
        

        public ActionResult Quit()
        {
            //Session.Abandon();
            //Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", string.Empty) { HttpOnly = true });
            //清缓存 清COOKIE
            var session = Request.Cookies["UserSession"];
            if (session == null) return RedirectToAction("Index", "Login");
            var sessionId = session.Value;
            _cacheService.Remove(sessionId);
            session.Expires = DateTime.Now.AddDays(-999);
            return RedirectToAction("Index", "Login");
        }
    }
}