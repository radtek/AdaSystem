using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;

namespace Boss.Controllers
{
    public class BusinessOrderDetailController : BaseController
    {
        private readonly IRepository<BusinessOrderDetail> _repository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseOrderDetailRepository;

        public BusinessOrderDetailController(IRepository<BusinessOrderDetail> repository,
            IRepository<PurchaseOrderDetail> purchaseOrderDetailRepository)
        {
            _repository = repository;
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(BusinessOrderDetailView viewModel)
        {

            var purchaseOrderDetails = _purchaseOrderDetailRepository.LoadEntities(d => d.IsDelete == false);
            var allList = _repository.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.BusinessOrder.Transactor.Contains(viewModel.search) || d.MediaName.Contains(viewModel.search));
            }

            
            var result = from b in allList
                         from p in purchaseOrderDetails
                         where b.Id == p.BusinessOrderDetailId &&
                               p.Status != Consts.PurchaseStatusFail &&
                               p.PublishDate < p.AddedDate &&
                               p.MediaPrice.Media.MediaType.IsComment == true &&
                               SqlFunctions.DateDiff("day", p.PublishDate, p.AddedDate) > 0
                         select new
                         {
                             b.MediaName,
                             b.BusinessOrder.Transactor,
                             p.AddedDate,
                             p.PublishDate,
                             b.BusinessOrderId,
                             b.BusinessOrder.OrderNum,
                             b.Id,
                             Diff = SqlFunctions.DateDiff("day", p.PublishDate, p.AddedDate)
                         };
            if (!string.IsNullOrWhiteSpace(viewModel.PublishDateStr))
            {
                var temp = viewModel.PublishDateStr.Trim().Replace("至", "#").Split('#');
                var min = Convert.ToDateTime(temp[0].Trim());
                var max = Convert.ToDateTime(temp[1].Trim()).AddDays(1);
                result = result.Where(d => d.PublishDate >= min && d.PublishDate < max);
            }
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

    }
}