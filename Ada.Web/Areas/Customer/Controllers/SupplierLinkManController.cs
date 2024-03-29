﻿using System;
using System.Linq;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;
using Ada.Framework.Filter;
using Ada.Services.Customer;
using Newtonsoft.Json;

namespace Customer.Controllers
{
    /// <summary>
    /// 供应商联系人
    /// </summary>
    public class SupplierLinkManController : BaseController
    {
        private readonly ILinkManService _linkManService;
        private readonly IRepository<LinkMan> _repository;

        public SupplierLinkManController(ILinkManService linkManService,
            IRepository<LinkMan> repository
        )
        {
            _linkManService = linkManService;
            _repository = repository;
        }
        public ActionResult Index()
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
                    Transactor = d.Transactor
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            LinkManView viewModel = new LinkManView();
            viewModel.IsBusiness = true;
            viewModel.Transactor = CurrentManager.UserName;
            viewModel.TransactorId = CurrentManager.Id;
            return View(viewModel);
        }
        [HttpPost]
        
        public ActionResult Add(LinkManView viewModel)
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
            //校验唯一性
            var temp = _repository
                .LoadEntities(d => d.Name.Equals(viewModel.Name, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false&&d.CommpanyId==viewModel.CommpanyId)
                .FirstOrDefault();
            if (temp != null)
            {
                ModelState.AddModelError("message", "该公司的联系人：" + viewModel.Name + "，已存在！");
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
            entity.Phone = viewModel.Phone;
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
            //校验唯一性
            var temp = _repository
                .LoadEntities(d => d.Name.Equals(viewModel.Name, StringComparison.CurrentCultureIgnoreCase) && d.IsDelete == false && d.CommpanyId == viewModel.CommpanyId&&d.Id!=viewModel.Id)
                .FirstOrDefault();
            if (temp != null)
            {
                ModelState.AddModelError("message", "该公司的联系人：" + viewModel.Name + "，已存在！");
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
            entity.Transactor = viewModel.Transactor;
            entity.TransactorId = viewModel.TransactorId;
            _linkManService.Update(entity);
            TempData["Msg"] = "更新成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            entity.DeletedBy = CurrentManager.UserName;
            entity.DeletedById = CurrentManager.Id;
            entity.DeletedDate = DateTime.Now;
            _linkManService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
        //[HttpPost]
        //
        public ActionResult Export(LinkManView viewModel)
        {
            viewModel.Managers = PremissionData();
            viewModel.limit = 1000;
            viewModel.IsBusiness = true;
            var result = _linkManService.LoadEntitiesFilter(viewModel).ToList();
            var json = JsonConvert.SerializeObject(result.Select(d => new
            {
                主键 = d.Id,
                公司名称=d.Commpany.Name,
                结算人=d.Name,
                经办媒介=d.Transactor
            }));
            return File(ExportData(json), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "微广联合数据表-" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx");
        }

       
    }
}