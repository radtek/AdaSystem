using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Admin;

namespace Ada.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Manager> _repository;
        private readonly IDbContext _dbContext;
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public HomeController(IRepository<Manager> repository,IDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }
        public ActionResult Index()
        {

            var model = _repository.LoadEntities(d => d.IsDelete == false).ToList();
            
            return View(model);
        }
    }
}