using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Resource.Controllers
{
    /// <summary>
    /// 刷量
    /// </summary>
    public class BrushController : BaseController
    {
        // GET: Brush
        public ActionResult Index()
        {
            return View();
        }
    }
}