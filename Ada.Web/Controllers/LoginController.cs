﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Log;
using Ada.Core.Domain.Purchase;
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
        private readonly IRepository<BusinessOrderDetail> _temp;
        private readonly IDbContext _dbContext;
        private readonly ISignals _signals;
        public LoginController(IRepository<Manager> repository, IDbContext dbContext, ISignals signals, IRepository<BusinessOrderDetail> temp)
        {
            _repository = repository;
            _dbContext = dbContext;
            _signals = signals;

            _temp = temp;
        }
        public ActionResult Index()
        {
            if (Session["LoginManager"] != null)
            {
                return RedirectToAction("Index", "Home", new { area = "Dashboards" });
            }
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
            if (manager.Roles.Count == 0)
            {
                ModelState.AddModelError("message", "未分配角色，请联系管理员");
                return View();
            }
            //根据角色级别排序，获取最高的那个
            var role = manager.Roles.OrderBy(d => d.RoleGrade).FirstOrDefault();
            //记录日志
            manager.ManagerLoginLogs.Add(new ManagerLoginLog()
            {
                Id = IdBuilder.CreateIdNum(),
                IpAddress = Utils.GetIpAddress(),
                LoginTime = DateTime.Now,
                WebInfo = Request.UserAgent,
                Remark = "成功"
            });
            _dbContext.SaveChanges();
            ManagerView managerView = new ManagerView()
            {
                Id = manager.Id,
                Phone = manager.Phone,
                RealName = manager.RealName,
                Image = manager.Image,
                UserName = manager.UserName,
                RoleId = role.Id,
                RoleName = role.RoleName,
                RoleList = manager.Roles.Select(d => new RoleView() { Id = d.Id, RoleName = d.RoleName }),
                Roles = manager.Roles.Count > 0 ? string.Join(",", manager.Roles.Select(d => d.RoleName)) : "",
                Organizations = manager.Organizations.Count > 0 ? String.Join("-", manager.Organizations.Select(d => d.OrganizationName)) : ""
            };
            Session["LoginManager"] = SerializeHelper.SerializeToString(managerView);
            //清空登陆日志缓存
            _signals.Trigger("LoginLog" + managerView.Id + ".Changed");

            return RedirectToAction("Index", "Home", new { area = "Dashboards" });

        }

        public ActionResult Quit()
        {
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", string.Empty) { HttpOnly = true });
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        public ActionResult Update()
        {
            var list = _temp.LoadEntities(d => d.IsDelete == false && d.VerificationStatus == Consts.StateLock);
            foreach (var businessOrderDetail in list)
            {
                businessOrderDetail.VerificationMoney = businessOrderDetail.SellMoney;
            }

            _dbContext.SaveChanges();
            return Content("OK");
        }

    }
}