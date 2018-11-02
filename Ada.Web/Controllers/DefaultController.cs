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
using Ada.Services.Content;
using log4net;

namespace Ada.Web.Controllers
{
    [UserException, Compress]
    public class DefaultController : Controller
    {
        private readonly IRepository<Article> _articleRepository;
        private readonly IArticleService _articleService;
        public DefaultController(IRepository<Article> articleRepository,
            IArticleService articleService)
        {
            _articleRepository = articleRepository;
            _articleService = articleService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Detail(string id)
        {
            var article = _articleRepository.LoadEntities(d => d.Id == id).FirstOrDefault();
            article.Click = article.Click == null ? 1 : article.Click+1;
            _articleService.Update(article);
            return View(article);
        }
    }
}