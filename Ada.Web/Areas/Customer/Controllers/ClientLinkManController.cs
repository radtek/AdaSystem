using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Customer;
using Ada.Framework.Filter;
using Ada.Services.Customer;

namespace Customer.Controllers
{
    /// <summary>
    /// 客户联系人
    /// </summary>
    public class ClientLinkManController : BaseController
    {
        private readonly ILinkManService _linkManService;
        private readonly IFollowUpService _followUpService;
        private readonly IRepository<LinkMan> _repository;
        public ClientLinkManController(ILinkManService linkManService,
            IRepository<LinkMan> repository, IFollowUpService followUpService
        )
        {
            _linkManService = linkManService;
            _repository = repository;
            _followUpService = followUpService;
        }
        public ActionResult Index()
        {
            ViewBag.IsAdmin = !PremissionData().Any();
            return View();
        }
        public ActionResult VIP()
        {
            return View();
        }
        public ActionResult GetList(LinkManView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _linkManService.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new LinkManView
                {
                    Id = d.Id,
                    Name = d.Name,
                    WorkName = d.WorkName,
                    LinkManType = d.LinkManType,
                    Phone = d.Phone,
                    WeiXin = d.WeiXin,
                    QQ = d.QQ,
                    Sex = d.Sex,
                    Status = d.Status,
                    Address = d.Address,
                    CommpanyName = d.Commpany.Name,
                    Transactor = d.Transactor,
                    IsLock = d.IsLock,
                    LoginName = d.LoginName,
                    LastLoginTime=d.FollowUps.OrderByDescending(l => l.NextTime).FirstOrDefault() == null ? "从未登陆" : Utils.ToRead(d.FollowUps.OrderByDescending(l => l.NextTime).FirstOrDefault().NextTime)

                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            LinkManView viewModel = new LinkManView();
            viewModel.IsBusiness = false;
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(LinkManView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }

            if (string.IsNullOrWhiteSpace(viewModel.QQ) && string.IsNullOrWhiteSpace(viewModel.Phone)&&string.IsNullOrWhiteSpace(viewModel.WeiXin))
            {
                ModelState.AddModelError("message", "手机，微信，QQ联系方式必填写一种");
                return View(viewModel);
            }
            LinkMan entity = new LinkMan();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedById = CurrentManager.Id;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedDate = DateTime.Now;
            entity.CommpanyId = viewModel.CommpanyId;
            entity.Name = viewModel.Name;
            entity.Address = viewModel.Address;
            entity.WorkName = viewModel.WorkName;
            entity.LinkManType = viewModel.LinkManType;
            entity.QQ = viewModel.QQ;
            entity.Phone = viewModel.Phone?.Trim();
            entity.WeiXin = viewModel.WeiXin;
            entity.Sex = viewModel.Sex;
            entity.Status = viewModel.Status;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            _linkManService.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            LinkManView entity = new LinkManView();
            entity.Id = item.Id;
            entity.Name = item.Name;
            entity.Address = item.Address;
            entity.WorkName = item.WorkName;
            entity.LinkManType = item.LinkManType;
            entity.QQ = item.QQ;
            entity.Phone = item.Phone;
            entity.WeiXin = item.WeiXin;
            entity.Sex = item.Sex;
            entity.Status = item.Status;
            entity.CommpanyId = item.CommpanyId;
            entity.CommpanyName = item.Commpany.Name;
            entity.IsBusiness = item.Commpany.IsBusiness;
            entity.Transactor = item.Transactor;
            entity.TransactorId = item.TransactorId;
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(LinkManView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            if (string.IsNullOrWhiteSpace(viewModel.QQ) && string.IsNullOrWhiteSpace(viewModel.Phone) && string.IsNullOrWhiteSpace(viewModel.WeiXin))
            {
                ModelState.AddModelError("message", "手机，微信，QQ联系方式必填写一种");
                return View(viewModel);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.Name = viewModel.Name;
            entity.Address = viewModel.Address;
            entity.WorkName = viewModel.WorkName;
            entity.LinkManType = viewModel.LinkManType;
            entity.QQ = viewModel.QQ;
            entity.Phone = viewModel.Phone?.Trim();
            entity.WeiXin = viewModel.WeiXin;
            entity.Sex = viewModel.Sex;
            entity.Status = viewModel.Status;
            entity.CommpanyId = viewModel.CommpanyId;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            _linkManService.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.IsLock == false)
            {
                return Json(new { State = 0, Msg = "此账户已开通会员，无法删除。请锁定账户后，再进行删除" });
            }
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _linkManService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
        public ActionResult FollowUp(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            FollowUpView viewModel = new FollowUpView();
            viewModel.LinkManId = id;
            viewModel.CompanyName = item.Commpany.Name;
            viewModel.LinkManName = item.Name;
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FollowUp(FollowUpView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            FollowUp entity = new FollowUp();
            entity.AddedById = CurrentManager.Id;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedDate = DateTime.Now;
            entity.Content = viewModel.Content;
            entity.FollowUpWay = viewModel.FollowUpWay;
            entity.LinkManId = viewModel.LinkManId;
            entity.NextTime = viewModel.NextTime;
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            entity.Id = IdBuilder.CreateIdNum();
            _followUpService.Add(entity);
            TempData["Msg"] = "跟进成功";
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 开通会员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult CreateAccount(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            LinkManView view = new LinkManView();
            view.Id = id;
            view.Name = entity.Commpany.Name + " - " + entity.Name;
            view.LoginName = entity.Phone;
            view.Password = "wglh666666";
            return PartialView("CreateAccount", view);
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult CreateAccount(LinkManView viewModel)
        {
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            if (entity==null)
            {
                return Json(new { State = 0, Msg = "开通会员的客户联系人不存在！" });
            }
            if (string.IsNullOrWhiteSpace(viewModel.LoginName))
            {
                return Json(new { State = 0, Msg = "登陆账户名称不能为空" });
            }
            if (!Utils.IsMobilePhone(viewModel.LoginName))
            {
                return Json(new { State = 0, Msg = "登陆账户名需为手机号码" });
            }
            if (viewModel.Password.Trim().Length < 6)
            {
                return Json(new { State = 0, Msg = "密码长度不能少于6位" });
            }
            entity.LoginName = viewModel.LoginName.Trim();
            //校验唯一性
            var temp = _repository
                .LoadEntities(d => d.LoginName==entity.LoginName && d.IsDelete == false && d.IsLock != true)
                .FirstOrDefault();
            if (temp != null)
            {
                return Json(new { State = 0, Msg = entity.LoginName + ",此手机号已被占用!" });
            }
            entity.IsLock = false;
            entity.Password = Encrypt.Encode(viewModel.Password.Trim());
            _linkManService.Update(entity);
            return Json(new { State = 1, Msg = "开通会员成功!" });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult LockAccount(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.IsLock == null)
            {
                return Json(new { State = 0, Msg = "此用户还未开通会员，请先开通会员" });
            }
            entity.IsLock = true;
            entity.LoginName = null;
            entity.Password = null;
            entity.OpenId = null;
            entity.UnionId = null;
            _linkManService.Update(entity);
            return Json(new { State = 1, Msg = "锁定成功" });
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult ResetPassword(string id,string p= "wglh666666")
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.IsLock == null)
            {
                return Json(new { State = 0, Msg = "此用户还未开通会员，请先开通会员" });
            }
            if (string.IsNullOrWhiteSpace(p))
            {
                p = "wglh666666";
            }
            if (p.Length<6)
            {
                return Json(new { State = 0, Msg = "密码长度不能少于6位！" });
            }
            entity.Password = Encrypt.Encode(p);
            _linkManService.Update(entity);
            return Json(new { State = 1, Msg = "密码已重置为："+ p });
        }
    }
}