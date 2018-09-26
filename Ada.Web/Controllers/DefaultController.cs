using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Content;
using Ada.Core.Infrastructure;
using Ada.Framework.Filter;
using log4net;

namespace Ada.Web.Controllers
{
    [UserException]
    public class DefaultController : Controller
    {
        private readonly IRepository<Article> _articleRepository;

        public DefaultController(IRepository<Article> articleRepository)
        {
            _articleRepository = articleRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Detail(string id)
        {
            var article = _articleRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            return View(article);
        }
    }
}