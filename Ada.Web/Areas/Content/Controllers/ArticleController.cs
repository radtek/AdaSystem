using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;

namespace Content.Controllers
{
    public class ArticleController : BaseController
    {
        // GET: Article
        public ActionResult Index()
        {
            return View();
        }
    }
}