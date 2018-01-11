using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Framework.Filter;
using Ada.Services.Business;

namespace DataReport.Controllers
{
    /// <summary>
    /// 销售业绩
    /// </summary>
    public class BusinessController : BaseController
    {
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<Manager> _managerRepository;
        private readonly IBusinessOrderDetailService _businessOrderDetailService;
        public BusinessController(IRepository<Organization> organizationRepository,
            IRepository<Manager> managerRepository, IBusinessOrderDetailService businessOrderDetailService)
        {
            _organizationRepository = organizationRepository;
            _managerRepository = managerRepository;
            _businessOrderDetailService = businessOrderDetailService;
        }
        public ActionResult Index()
        {
            var organization = _organizationRepository.LoadEntities(d => d.IsDelete == false && d.OrganizationName == "业务部").FirstOrDefault();
            var allManagers = _managerRepository.LoadEntities(d => d.Status == Consts.StateNormal && d.IsDelete == false);
            var managers = from m in allManagers
                           from o in m.Organizations
                           where o.TreePath.Contains(organization.Id)
                           select m;
           var model= _businessOrderDetailService.BusinessPerformance(managers.ToList());
            return View(model);
        }
    }
}