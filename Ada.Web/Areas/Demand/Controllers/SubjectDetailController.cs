﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Common;
using Ada.Core.Domain.Demand;
using Ada.Core.ViewModel.Demand;
using Ada.Framework.Filter;
using Ada.Services.Demand;
using Files.Services;

namespace Demand.Controllers
{
    public class SubjectDetailController : BaseController
    {
        private readonly ISubjectDetailService _service;
        private readonly IFileService _fileService;
        private readonly object _olocker = new object();
        public SubjectDetailController(ISubjectDetailService service, IFileService fileService)
        {
            _service = service;
            _fileService = fileService;
        }
        public ActionResult GetList(SubjectDetailView viewModel)
        {
            viewModel.Managers = PremissionData();
            
            return GetAllList(viewModel);
        }
        public ActionResult GetAllList(SubjectDetailView viewModel)
        {
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new SubjectDetailView
                {
                    Id = d.Id,
                    Title = d.Title,

                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail(string id)
        {
          var entity=  _service.GetById(id);
            return View(entity);
        }
        [HttpPost]
        public ActionResult Order(string id)
        {
            lock (_olocker)
            {
                var detail = _service.GetById(id);
                //制作者枪弹
                if (!string.IsNullOrWhiteSpace(detail.TransactorId) && string.IsNullOrWhiteSpace(detail.ProducerById))
                {
                    detail.ProducerDate = DateTime.Now;
                    detail.ProducerBy = CurrentManager.UserName;
                    detail.ProducerById = CurrentManager.Id;
                    _service.Update(detail);
                    return Json(new { State = 1, Msg = "获取成功，请抓紧时间制作并上传素材！" });
                }
                //全体抢单
                if (string.IsNullOrWhiteSpace(detail.TransactorId))
                {
                    detail.GetDate = DateTime.Now;
                    detail.Transactor = CurrentManager.UserName;
                    detail.TransactorId = CurrentManager.Id;
                    detail.Status = Consts.StateNormal;
                    _service.Update(detail);
                    return Json(new { State = 1, Msg = "获取成功，请抓紧时间截图上传素材" });
                }
                return Json(new { State = 0, Msg = "抱歉，此需求已被他人获取！" });
            }


        }
        public ActionResult Download(string id)
        {
            var entity = _service.GetById(id);
            var imgs = entity.Subject.Attachments.Select(d => new SelectListItem() { Value = Server.MapPath(d.Path), Text = string.IsNullOrWhiteSpace(d.Describe) ? d.Title : d.Describe }).ToList();
            var zip = _fileService.ZipFiles(imgs);
            return File(zip,
                "application/zip", entity.Title + ".zip");
        }
        public ActionResult Upload(string id, string returnurl)
        {
            return View(new SubjectDetailProgressView() { SubjectDetailId = id, Redirect = returnurl });
        }
        [HttpPost]
        public ActionResult Upload(SubjectDetailProgressView viewModel)
        {

            var detail = _service.GetById(viewModel.SubjectDetailId);
            if (detail.Status == 3)
            {
                ModelState.AddModelError("message", "该需求已经完结，无需再进行上传！");
                return View(viewModel);
            }
            var progress = new SubjectDetailProgress()
            {
                Id = IdBuilder.CreateIdNum(),
                UploadDate = DateTime.Now,
                Remark = string.IsNullOrWhiteSpace(viewModel.Remark) ? "上传素材" : viewModel.Remark,
                AddedBy = CurrentManager.UserName,
                AddedById = CurrentManager.Id
            };
            //图片
            for (int i = 0; i < viewModel.MaterialImage.Count; i++)
            {
                string remark = null;
                if (!string.IsNullOrWhiteSpace(viewModel.MaterialRemark[i]))
                {
                    remark = viewModel.MaterialRemark[i];
                }
                progress.Attachments.Add(new Attachment
                {
                    Id = IdBuilder.CreateIdNum(),
                    Path = viewModel.MaterialImage[i],
                    Title = viewModel.MaterialName[i],
                    FileSize = viewModel.MaterialSize[i],
                    FileExt = viewModel.MaterialExt[i],
                    ThumbPath = viewModel.MaterialThumbImage[i],
                    Describe = remark,
                    AddedDate = DateTime.Now,
                    AddedBy = CurrentManager.UserName,
                    AddedById = CurrentManager.Id
                });
            }
            //改变成待验收状态
            if (CurrentManager.Id == detail.ProducerById)
            {
                detail.Status = Consts.StateOK;
            }
            detail.SubjectDetailProgresses.Add(progress);
            _service.Update(detail);
            TempData["Msg"] = "上传成功";
            return RedirectToAction("Self", viewModel.Redirect);
        }

    }
}