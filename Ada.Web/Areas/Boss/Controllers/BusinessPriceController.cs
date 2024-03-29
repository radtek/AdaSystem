﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Framework.Filter;

namespace Boss.Controllers
{
    public class BusinessPriceController : BaseController
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<BusinessOrderDetail> _repository;
        public BusinessPriceController(IDbContext dbContext, IRepository<BusinessOrderDetail> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        
        public ActionResult Audit(string ids)
        {
            var arr = ids.Split(',');
            foreach (var id in arr)
            {
                var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
                if (entity.VerificationStatus==Consts.StateNormal)
                {
                    return Json(new { State = 0, Msg = entity.MediaName+"，此订单已核销！审核失败" });
                }

                var oldSell = entity.SellMoney;
                entity.Status = Consts.StateOK;
                entity.SellMoney = entity.RequestSellMoney;
                entity.RequestSellMoney = oldSell;
                entity.VerificationMoney = entity.SellMoney;
                entity.Money = entity.SellMoney * (1 + entity.Tax / 100);
                entity.TaxMoney = entity.SellMoney * (entity.Tax / 100);
                entity.AuditStatus = Consts.StateNormal;
                entity.AuditDate = DateTime.Now;
            }
            _dbContext.SaveChanges();
            return Json(new { State = 1, Msg = "审批成功" });
        }
    }
}