using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Common;
using Ada.Core.ViewModel.Common;
using Ada.Framework.Filter;
using Ada.Services.Common;
using Newtonsoft.Json.Linq;

namespace Tools.Controllers
{
    public class FansController : BaseController
    {
        private readonly IRepository<Fans> _repository;
        private readonly IFansService _service;
        public FansController(IRepository<Fans> repository, IFansService service)
        {
            _service = service;
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(FansView viewModel)
        {
            //viewModel.Managers = PremissionData();
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new FansView()
                {
                    Id = d.Id,
                    NickName = d.NickName,
                    Avatar = d.Avatar
                })
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Export(FansView viewModel)
        {
            viewModel.limit = int.MaxValue;
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            JArray jObjects = new JArray();
            foreach (var item in result)
            {
                var jo = new JObject();
                jo.Add("主键", item.Id);
                jo.Add("昵称", item.NickName);
                jObjects.Add(jo);
            }
            return Json(new { State = 1, Msg = ExportFile(jObjects.ToString()) });
        }
        public ActionResult Add()
        {
            return View(new FansView());
        }
        [HttpPost]
        
        public ActionResult Add(FansView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            IDictionary idc = new Dictionary<string, string>();
            foreach (var filesAllKey in Request.Files.AllKeys)
            {
                var path = UploadImageFile(filesAllKey);
                idc.Add(filesAllKey, path);
            }
            Fans entity = new Fans
            {
                Id = IdBuilder.CreateIdNum(),
                AddedById = CurrentManager.Id,
                AddedBy = CurrentManager.UserName,
                AddedDate = DateTime.Now,
                NickName = viewModel.NickName.Trim(),
                ParentId = viewModel.ParentId,
                AvatarRange = viewModel.AvatarRange,
                FansRange = viewModel.FansRange,
                Type = viewModel.Type,
                Avatar = idc["Avatar"]?.ToString(),
                Cover = idc["Cover"]?.ToString()
            };
            _service.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            string parentName=null;
            if (!string.IsNullOrWhiteSpace(item.ParentId))
            {
                var parent= _repository.LoadEntities(d => d.Id == item.ParentId).FirstOrDefault();
                parentName = parent.NickName;
            }
            FansView entity = new FansView
            {
                Id = item.Id,
                NickName = item.NickName,
                Avatar = item.Avatar,
                Type = item.Type,
                Cover = item.Cover,
                ParentId = item.ParentId,
                ParentName = parentName,
                AvatarRange = item.AvatarRange,
                FansRange = item.FansRange
            };
            return View(entity);
        }
        [HttpPost]
        
        public ActionResult Update(FansView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            var entity = _repository.LoadEntities(d => d.Id == viewModel.Id).FirstOrDefault();
            IDictionary idc = new Dictionary<string, string>();
            foreach (var filesAllKey in Request.Files.AllKeys)
            {
                var path = UploadImageFile(filesAllKey);
                idc.Add(filesAllKey, path);
            }
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.NickName = viewModel.NickName.Trim();
            entity.ParentId = viewModel.ParentId;
            entity.AvatarRange = viewModel.AvatarRange;
            entity.FansRange = viewModel.FansRange;
            entity.Type = viewModel.Type;
            entity.Avatar = idc["Avatar"] == null ? entity.Avatar : idc["Avatar"]?.ToString();
            entity.Cover = idc["Cover"] == null ? entity.Cover : idc["Cover"]?.ToString();
            _service.Update(entity);
            TempData["Msg"] = "编辑成功";
            return RedirectToAction("Index");
        }
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            _service.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }

        
    }
}