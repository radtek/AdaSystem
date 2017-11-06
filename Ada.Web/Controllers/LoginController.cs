using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Log;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Admin;
using Ada.Framework.Caching;
using log4net;

namespace Ada.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IRepository<Manager> _repository;
        private readonly IDbContext _dbContext;
        private readonly ISignals _signals;
        public LoginController(IRepository<Manager> repository, IDbContext dbContext, ISignals signals)
        {
            _repository = repository;
            _dbContext = dbContext;
            _signals = signals;
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string userName, string password)
        {
            //校验用户
            var pwd = Encrypt.Encode(password);
            var manager =
                _repository.LoadEntities(u => u.UserName == userName && u.Password == pwd && u.Status == Consts.StateNormal && u.IsDelete == false).FirstOrDefault();
            if (manager == null)
            {
                ModelState.AddModelError("message", "用户名或密码有误");
                return View();
            }
            //记录日志
            manager.ManagerLoginLogs.Add(new ManagerLoginLog()
            {
                Id = IdBuilder.CreateIdNum(),
                IpAddress = Utils.GetIpAddress(),
                LoginTime = DateTime.Now,
                WebInfo = Request.UserAgent
            });
            _dbContext.SaveChanges();
            ManagerView managerView = new ManagerView()
            {
                Id = manager.Id,
                Phone = manager.Phone,
                RealName = manager.RealName,
                Image = manager.Image,
                UserName = manager.UserName,
                Roles = manager.Roles.Count > 0 ? string.Join(",", manager.Roles.Select(d => d.RoleName)) : "",
                Organizations = manager.Organizations.Count > 0 ? String.Join("-", manager.Organizations.Select(d => d.OrganizationName)) : ""
            };
            Session["LoginManager"] = SerializeHelper.SerializeToString(managerView);
            //清空缓存
            _signals.Trigger("LoginLog" + managerView.Id + ".Changed");

            return RedirectToAction("Index", "Home", new { area = "Dashboards" });

        }

        public ActionResult Quit()
        {
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", string.Empty) { HttpOnly = true });
            return RedirectToAction("Index", "Default", new { area = "" });
        }

    }
}