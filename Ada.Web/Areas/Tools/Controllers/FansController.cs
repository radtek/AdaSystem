using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Common;
using Ada.Core.ViewModel.Common;
using Ada.Framework.Filter;
using Ada.Framework.UploadFile;
using Ada.Services.Common;

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
            viewModel.Managers = PremissionData();
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
        public ActionResult Add()
        {
            return View(new FansView());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                var path = UploadImage(filesAllKey);
                idc.Add(filesAllKey, path);
            }
            Fans entity = new Fans
            {
                Id = IdBuilder.CreateIdNum(),
                AddedById = CurrentManager.Id,
                AddedBy = CurrentManager.UserName,
                AddedDate = DateTime.Now,
                NickName = viewModel.NickName,
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
            FansView entity = new FansView
            {
                Id = item.Id,
                NickName = item.NickName,
                Avatar = item.Avatar,
                Type = item.Type,
                Cover = item.Cover
            };
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                var path = UploadImage(filesAllKey);
                idc.Add(filesAllKey, path);
            }
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedDate = DateTime.Now;
            entity.NickName = viewModel.NickName;
            entity.Type = viewModel.Type;
            entity.Avatar = idc["Avatar"] == null ? entity.Avatar : idc["Avatar"]?.ToString();
            entity.Cover = idc["Cover"] == null ? entity.Cover : idc["Cover"]?.ToString();
            _service.Update(entity);
            TempData["Msg"] = "编辑成功";
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

        private string UploadImage(string name)
        {
            UEditorModel uploadConfig = new UEditorModel()
            {
                AllowExtensions = UEditorConfig.GetStringList("imageAllowFiles"),
                PathFormat = UEditorConfig.GetString("imagePathFormat"),
                SizeLimit = UEditorConfig.GetInt("imageMaxSize"),
                UploadFieldName = name
            };
            var file = Request.Files[name];
            if (file == null) return null;
            try
            {
                if (string.IsNullOrWhiteSpace(file.FileName))
                {
                    return null;
                }
                var uploadFileName = file.FileName;
                var fileExtension = Path.GetExtension(uploadFileName).ToLower();
                if (!uploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension))
                {
                    throw new ApplicationException("请上传正确的图片格式文件");
                    //ModelState.AddModelError("message", "请上传正确的图片格式文件");
                }
                if (!(file.ContentLength < uploadConfig.SizeLimit))
                {
                    throw new ApplicationException("上传的图片最大只能为：" + uploadConfig.SizeLimit + "B");
                    //ModelState.AddModelError("message", "上传的图片最大只能为：" + uploadConfig.SizeLimit + "B");
                }
                var uploadFileBytes = new byte[file.ContentLength];
                file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);
                var savePath = PathFormatter.Format(uploadFileName, uploadConfig.PathFormat);
                var localPath = Server.MapPath(savePath);
                if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                }
                System.IO.File.WriteAllBytes(localPath, uploadFileBytes);
                return savePath;

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}