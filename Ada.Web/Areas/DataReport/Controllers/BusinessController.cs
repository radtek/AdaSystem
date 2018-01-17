using System.Linq;
using System.Web.Mvc;
using Ada.Core.ViewModel.Business;
using Ada.Framework.Filter;
using Ada.Services.Admin;
using Ada.Services.Business;

namespace DataReport.Controllers
{
    /// <summary>
    /// 销售业绩
    /// </summary>
    public class BusinessController : BaseController
    {
        private readonly IManagerService _managerService;
        private readonly IBusinessOrderDetailService _businessOrderDetailService;
        public BusinessController(IManagerService managerService, IBusinessOrderDetailService businessOrderDetailService)
        {
            _managerService = managerService;

            _businessOrderDetailService = businessOrderDetailService;
        }
        public ActionResult Index()
        {
            var managers = _managerService.GetByOrganizationName("业务部");
            var model = _businessOrderDetailService.BusinessPerformance(managers.ToList(), new BusinessOrderDetailView());
            return View(model);
        }
    }
}