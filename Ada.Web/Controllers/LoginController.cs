using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Customer;
using Ada.Services.Customer;
using Ada.Web.Models;

namespace Ada.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILinkManService _linkManService;
        public LoginController(ILinkManService linkManService)
        {
            _linkManService = linkManService;
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            var user = _linkManService.CheackUser(loginModel.LoginName, loginModel.PassWord);
            if (user == null)
            {
                ModelState.AddModelError("error", "用户名或密码有误！");
                return View(loginModel);
            }
            LinkManView viewModel=new LinkManView();
            viewModel.Id = user.Id;
            viewModel.CommpanyName = user.Commpany.Name;
            viewModel.LoginName = user.LoginName;
            viewModel.Phone = user.Phone;
            Session["User"] = SerializeHelper.SerializeToString(viewModel);
            return RedirectToAction("WeiXin", "Media");
        }
    }
}