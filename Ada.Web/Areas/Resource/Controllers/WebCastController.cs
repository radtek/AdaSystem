using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Resource;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Filter;
using Ada.Services.Resource;

namespace Resource.Controllers
{
    /// <summary>
    /// 网络直播 网红
    /// </summary>
    public class WebCastController : BaseController
    {
       
        public ActionResult Index()
        {
            return View();
        }
        
      
        
    }
}