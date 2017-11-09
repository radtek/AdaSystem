using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;
using Ada.Framework.Filter;
using Ada.Services.Customer;

namespace Customer.Controllers
{
    public class LinkManController : BaseController
    {
        private readonly ILinkManService _linkManService;
        private readonly IRepository<LinkMan> _repository;
        public LinkManController(ILinkManService linkManService,
            IRepository<LinkMan> repository
           )
        {
            _linkManService = linkManService;
            _repository = repository;
        }
        public ActionResult Business()
        {
            return View();
        }
        public ActionResult Custom()
        {
            return View();
        }
        public ActionResult GetList(LinkManView viewModel)
        {
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
                    CommpanyName = d.Commpany.Name
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddBusiness()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBusiness(LinkManView viewModel)
        {
            return Add(viewModel,"Business");
        }
        public ActionResult AddCustom()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustom(LinkManView viewModel)
        {
            return Add(viewModel, "Custom");
        }
        private ActionResult Add(LinkManView viewModel,string returnView)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View();
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
            entity.Phone = viewModel.Phone;
            entity.WeiXin = viewModel.WeiXin;
            entity.Sex = viewModel.Sex;
            entity.Status = viewModel.Status;
            _linkManService.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction(returnView);
        }
        public ActionResult UpdateBusiness(string id)
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
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateBusiness(LinkManView viewModel)
        {
            return Update(viewModel, "Business");
        }
        public ActionResult UpdateCustom(string id)
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
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCustom(LinkManView viewModel)
        {
            return Update(viewModel, "Custom");
        }
        private ActionResult Update(LinkManView viewModel,string returnView)
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
            entity.Address = viewModel.Address;
            entity.WorkName = viewModel.WorkName;
            entity.LinkManType = viewModel.LinkManType;
            entity.QQ = viewModel.QQ;
            entity.Phone = viewModel.Phone;
            entity.WeiXin = viewModel.WeiXin;
            entity.Sex = viewModel.Sex;
            entity.Status = viewModel.Status;
            entity.CommpanyId = viewModel.CommpanyId;
            _linkManService.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction(returnView);
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _linkManService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}