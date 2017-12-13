using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Finance.Controllers
{
    /// <summary>
    /// 费用支出
    /// </summary>
    public class ExpenseOutController : BaseController
    {
        // GET: ExpenseOut
        public ActionResult Index()
        {
            return View();
        }
    }
}