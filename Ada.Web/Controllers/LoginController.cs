using System;
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
        private readonly IRepository<PurchaseOrderDetail> _ptemp;
        private readonly IDbContext _dbContext;
        private readonly ISignals _signals;
        public LoginController(IRepository<Manager> repository, IDbContext dbContext, ISignals signals, IRepository<BusinessOrderDetail> temp, IRepository<PurchaseOrderDetail> ptemp)
        {
            _repository = repository;
            _dbContext = dbContext;
            _signals = signals;
            _ptemp = ptemp;
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

           var b= _temp.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false).ToList();
            List<string> ids = new List<string>();
            foreach (var item in b)
            {
                if (_ptemp.LoadEntities(d => d.BusinessOrderDetailId == item.Id).Count()>1)
                {
                    ids.Add(item.Id);
                }
            }

            if (ids.Count>0)
            {
                return Content(string.Join(",", ids));
            }

            return Content("未找到重复订单");
            //List<string> ids = new List<string>();
            //int x = 0;
            //int y = 0;
            //int z = 0;
            //int q = 0;
            //var p = _ptemp.LoadEntities(d => d.IsDelete == false).ToList();
            //foreach (var item in p)
            //{
            //    var b = _temp.LoadEntities(d => d.Id == item.BusinessOrderDetailId).FirstOrDefault();
            //    if (b == null)
            //    {
            //        ids.Add(item.BusinessOrderDetailId);
            //    }
            //    else
            //    {
            //        if (b.Status==0)
            //        {
            //            x++;
            //        }

            //        if (b.Status==1)
            //        {
            //            y++;
            //        }
            //        if (b.Status == 2)
            //        {
            //            z++;
            //        }
            //        if (b.Status == -1)
            //        {
            //            q++;
            //        }
            //    }

            //}

            //if (ids.Count > 0)
            //{
            //    return Content(string.Join(",", ids));
            //}
            //return Content("未转单："+x+"，已下单："+y+"，已完成："+z+"，待申请："+q);
            //BusinessOrderDetail detail = new BusinessOrderDetail();
            //detail.Id = "X1801121627020781";
            //detail.MediaName = "家居家电指南 - jjdpzn";
            //detail.Money = 0;
            //detail.BusinessOrderId = "X1801121627020779";
            //detail.Status = Consts.StateNormal;
            //detail.Tax = 8;
            //detail.SellMoney = 0;
            //detail.CostMoney = 600;
            //detail.AdPositionName = "头条";
            //detail.MediaTypeName = "微信";
            //detail.MediaByPurchase = "肖柳梦翎";
            //detail.VerificationStatus = 0;
            //detail.VerificationMoney = 0;
            //detail.ConfirmVerificationMoney = 0;
            //detail.AuditStatus = 0;
            //detail.MediaPriceId = "X1712181756410022";
            //_temp.Add(detail);
            //_dbContext.SaveChanges();
            //return Content("OK");

        }

    }
}