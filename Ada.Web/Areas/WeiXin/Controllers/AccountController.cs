using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.WeiXin;
using Ada.Core.ViewModel.WeiXin;
using Ada.Framework.Filter;
using Ada.Services.WeiXin;

namespace WeiXin.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IWeiXinAccountService _service;
        private readonly IRepository<WeiXinAccount> _repository;
        public AccountController(IWeiXinAccountService service, IRepository<WeiXinAccount> repository)
        {
            _service = service;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(WeiXinAccountView viewModel)
        {
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new WeiXinAccountView
                {
                    Id = d.Id,
                    Name = d.Name,
                    AccountType = d.AccountType,
                    Status = d.Status,
                    Url = ""
                    
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            WeiXinAccountView entity = new WeiXinAccountView {Status = true};
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(WeiXinAccountView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            WeiXinAccount entity = new WeiXinAccount();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedById = CurrentManager.Id;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedDate = DateTime.Now;
            entity.Name = viewModel.Name;
            entity.SourceId = viewModel.SourceId;
            entity.AccountType = viewModel.AccountType;
            entity.AppId = viewModel.AppId;
            entity.AppSecret = viewModel.AppSecret;
            entity.Status = viewModel.Status;
            entity.Token = viewModel.Token;
            entity.EncodingAESKey = viewModel.EncodingAESKey;
            entity.MchId = viewModel.MchId;
            entity.MchKey = viewModel.MchKey;
            entity.NotifyUrl = viewModel.NotifyUrl;
            entity.CretPath = viewModel.CretPath;
            entity.Image = viewModel.Image;

            _service.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            WeiXinAccountView entity = new WeiXinAccountView();
            entity.Id = item.Id;
            entity.Name = item.Name;
            entity.SourceId = item.SourceId;
            entity.AccountType = item.AccountType;
            entity.AppId = item.AppId;
            entity.AppSecret = item.AppSecret;
            entity.Status = item.Status;
            entity.Token = item.Token;
            entity.EncodingAESKey = item.EncodingAESKey;
            entity.MchId = item.MchId;
            entity.MchKey = item.MchKey;
            entity.NotifyUrl = item.NotifyUrl;
            entity.CretPath = item.CretPath;
            entity.Image = item.Image;
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(WeiXinAccountView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.Name = viewModel.Name;
            entity.SourceId = viewModel.SourceId;
            entity.AccountType = viewModel.AccountType;
            entity.AppId = viewModel.AppId;
            entity.AppSecret = viewModel.AppSecret;
            entity.Status = viewModel.Status;
            entity.Token = viewModel.Token;
            entity.EncodingAESKey = viewModel.EncodingAESKey;
            entity.MchId = viewModel.MchId;
            entity.MchKey = viewModel.MchKey;
            entity.NotifyUrl = viewModel.NotifyUrl;
            entity.CretPath = viewModel.CretPath;
            entity.Image = viewModel.Image;
            _service.Update(entity);
            TempData["Msg"] = "操作成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _service.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}