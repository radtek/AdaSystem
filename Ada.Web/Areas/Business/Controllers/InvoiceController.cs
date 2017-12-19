using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Business.Controllers
{
    /// <summary>
    /// 销售发票
    /// </summary>
    public class InvoiceController : BaseController
    {
        // GET: Invoice
        public ActionResult Index()
        {
            return View();
        }
    }
}