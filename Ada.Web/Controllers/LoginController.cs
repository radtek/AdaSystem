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
using Ada.Core.Domain.Resource;
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
        private readonly IRepository<Media> _media;
        private readonly IRepository<PurchaseOrderDetail> _ptemp;
        private readonly IDbContext _dbContext;
        private readonly ISignals _signals;
        public LoginController(IRepository<Manager> repository,
            IDbContext dbContext,
            ISignals signals,
            IRepository<BusinessOrderDetail> temp,
            IRepository<PurchaseOrderDetail> ptemp,
            IRepository<Media> media)
        {
            _repository = repository;
            _dbContext = dbContext;
            _signals = signals;
            _ptemp = ptemp;
            _temp = temp;
            _media = media;
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

        public ActionResult CheckOrder()
        {

            var b = _temp.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false).ToList();
            List<string> ids = new List<string>();
            foreach (var item in b)
            {
                if (_ptemp.LoadEntities(d => d.BusinessOrderDetailId == item.Id).Count() > 1)
                {
                    ids.Add(item.Id);
                }
            }

            var business = _temp
                .LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false && d.Status != 0).Count();
            var purchase = _ptemp.LoadEntities(d => d.IsDelete == false).Count();

            if (ids.Count > 0)
            {
                return Content("存在重复订单：" + string.Join(",", ids));
            }

            return Content("未找到重复订单，销售订单数：" + business + "，采购订单数：" + purchase);


        }
        public ActionResult Update()
        {

            //var b = _temp.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false && d.VerificationStatus == Consts.StateLock).ToList();
            //int i = 0;
            //foreach (var businessOrderDetail in b)
            //{
            //    if (businessOrderDetail.VerificationMoney != businessOrderDetail.SellMoney)
            //    {
            //        businessOrderDetail.VerificationMoney = businessOrderDetail.SellMoney;
            //        i++;
            //    }


            //}
            var p = _ptemp.LoadEntities(d => d.IsDelete == false);
            int i = 0;
            foreach (var purchaseOrderDetail in p)
            {
                var tax = purchaseOrderDetail.Tax ?? 0;
                purchaseOrderDetail.Money = purchaseOrderDetail.PurchaseMoney * (1 + tax / 100);
                i++;
            }
            _dbContext.SaveChanges();
            return Content(i.ToString());


        }
        public ActionResult UpdateMedia()
        {

            var medias = _media.LoadEntities(d => d.IsDelete == false && d.MediaType.CallIndex == "sinablog").ToList();
            //int start = medias.Count;
            //var last = medias.Distinct(new FastPropertyComparer<Media>("MediaID"));
            //var end = last.Count();
            int i = 0;
            foreach (var media in medias)
            {
                if (!string.IsNullOrWhiteSpace(media.MediaLink))
                {
                    media.MediaID = Utils.GetBlogId(media.MediaLink);
                    i++;
                }
            }
            _dbContext.SaveChanges();
            return Content("成功更新"+i+"个UID");
            //return Content("原有：" + start + ",去重后：" + end);


        }

        public ActionResult CheckMedia()
        {

            var m = _media.LoadEntities(d => d.IsDelete == false && d.MediaType.CallIndex== "sinablog").ToList();
            List<string> ids = new List<string>();
            List<string> isnulls = new List<string>();
            foreach (var item in m)
            {
                if (string.IsNullOrWhiteSpace(item.MediaID))
                {
                    isnulls.Add(item.MediaName);
                    continue;
                }
                var temp = _media.LoadEntities(d =>
                    d.MediaID.Equals(item.MediaID.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                    d.IsDelete == false &&
                    d.MediaTypeId == item.MediaTypeId && d.Id != item.Id).FirstOrDefault();
                if (temp!=null)
                {
                    ids.Add(item.MediaID);
                }
            }

            return Content("存在重复资源：" + string.Join(",", ids)+"，媒体ID是空的："+string.Join(",",isnulls));


        }
    }
}