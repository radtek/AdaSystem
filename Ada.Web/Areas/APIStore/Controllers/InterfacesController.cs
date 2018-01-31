using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.API;
using Ada.Core.ViewModel.API;
using Ada.Framework.Filter;
using Ada.Services.API;

namespace APIStore.Controllers
{
    public class InterfacesController : BaseController
    {
        private readonly IAPIInterfacesService _service;
        private readonly IRepository<APIInterfaces> _repository;
        public InterfacesController(IRepository<APIInterfaces> repository, IAPIInterfacesService service)
        {
            _service = service;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(APIInterfacesView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new APIInterfacesView
                {
                    Id = d.Id,
                    APIType = d.APIType,
                    APIName = d.APIName,
                    CallIndex = d.CallIndex,
                    APIUrl = d.APIUrl
                    
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            APIInterfacesView viewModel=new APIInterfacesView();
            viewModel.TimeOut = 0;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(APIInterfacesView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            APIInterfaces entity = new APIInterfaces();
            entity.Id = IdBuilder.CreateIdNum();
            entity.AddedById = CurrentManager.Id;
            entity.AddedBy = CurrentManager.UserName;
            entity.AddedDate = DateTime.Now;
            entity.APIType = viewModel.APIType;
            entity.APIName = viewModel.APIName;
            entity.CallIndex = viewModel.CallIndex;
            entity.APIUrl = viewModel.APIUrl;
            entity.HttpMethod = viewModel.HttpMethod;
            entity.Token = viewModel.Token;
            entity.HttpMethod = viewModel.HttpMethod;
            entity.AppId = viewModel.AppId;
            entity.AppSecret = viewModel.AppSecret;
            entity.Parameters = viewModel.Parameters;
            entity.TimeOut = viewModel.TimeOut;
            _service.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            APIInterfacesView entity = new APIInterfacesView();
            entity.Id = item.Id;
            entity.APIType = item.APIType;
            entity.APIName = item.APIName;
            entity.APIUrl = item.APIUrl;
            entity.HttpMethod = item.HttpMethod;
            entity.Token = item.Token;
            entity.HttpMethod = item.HttpMethod;
            entity.AppId = item.AppId;
            entity.AppSecret = item.AppSecret;
            entity.Parameters = item.Parameters;
            entity.TimeOut = item.TimeOut;
            entity.CallIndex = item.CallIndex;
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(APIInterfacesView viewModel)
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
            entity.APIType = viewModel.APIType;
            entity.APIName = viewModel.APIName;
            entity.CallIndex = viewModel.CallIndex;
            entity.APIUrl = viewModel.APIUrl;
            entity.HttpMethod = viewModel.HttpMethod;
            entity.Token = viewModel.Token;
            entity.HttpMethod = viewModel.HttpMethod;
            entity.AppId = viewModel.AppId;
            entity.AppSecret = viewModel.AppSecret;
            entity.Parameters = viewModel.Parameters;
            entity.TimeOut = viewModel.TimeOut;
            _service.Update(entity);
            TempData["Msg"] = "操作成功";
            return View(viewModel);
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