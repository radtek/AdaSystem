using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace WorkFlow.Controllers
{
    /// <summary>
    /// 流程记录
    /// </summary>
    public class RecordController : BaseController
    {
        // GET: Record
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 审批详情
        /// </summary>
        /// <returns></returns>
        public ActionResult Detail()
        {
            return View();
        }
    }
}