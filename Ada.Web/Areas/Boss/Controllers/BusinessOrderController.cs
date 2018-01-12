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
        [AdaValidateAntiForgeryToken]
        public ActionResult Audit(string ids)
        {
            var arr = ids.Split(',');
            foreach (var id in arr)
            {
                var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
                entity.Status = Consts.StateOK;
                entity.AuditStatus = Consts.StateNormal;
            }
            _dbContext.SaveChanges();
            return Json(new { State = 1, Msg = "审批成功" });
        }
    }
}