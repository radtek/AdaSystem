using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Framework.Filter;

namespace Boss.Controllers
{
    public class BusinessOrderController : BaseController
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<BusinessOrderDetail> _repository;
        public BusinessOrderController(IDbContext dbContext, IRepository<BusinessOrderDetail> repository)
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
                entity.Status = Consts.StateOK;
                entity.AuditStatus = Consts.StateNormal;
                entity.AuditDate=DateTime.Now;
            }
            _dbContext.SaveChanges();
            return Json(new { State = 1, Msg = "审批成功" });
        }
        [HttpPost]
        
        public ActionResult Cancle(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.VerificationStatus==Consts.StateNormal)
            {
                return Json(new { State = 0, Msg = "该订单已核销，无法撤销。" });
            }
            entity.Status = Consts.StateNormal;
            entity.AuditStatus = Consts.StateLock;
            entity.AuditDate = DateTime.Now;
            _dbContext.SaveChanges();
            return Json(new { State = 1, Msg = "撤销成功" });
        }
    }
}