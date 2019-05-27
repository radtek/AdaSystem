using System;
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
using Newtonsoft.Json;

namespace Demand.Controllers
{
    public class SubjectController : BaseController
    {
        private readonly ISubjectService _service;
        private readonly ISubjectDetailService _detailService;
        private readonly IFileService _fileService;
        public SubjectController(ISubjectService service, ISubjectDetailService detailService, IFileService fileService)
        {
            _service = service;
            _detailService = detailService;
            _fileService = fileService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(SubjectView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new SubjectView
                {
                    Id = d.Id,
                    Title = d.Title,
                    AddedBy = d.AddedBy,
                    AddedDate = d.AddedDate,
                    Type = d.Type,
                    Schedule = d.SubjectDetails.Count(s => s.Status == 3) + "/" + d.SubjectDetails.Count
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            SubjectView viewModel = new SubjectView();
            return View(viewModel);
        }
        [HttpPost]

        public ActionResult Add(SubjectView viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("message", "数据校验失败，请核对输入的信息是否准确");
                return View(viewModel);
            }
            Subject subject = new Subject
            {
                Id = IdBuilder.CreateIdNum(),
                Title = viewModel.Title,
                Type = viewModel.Type,
                Content = viewModel.Content,
                Offer = viewModel.Offer,
                AddedById = CurrentManager.Id,
                AddedBy = CurrentManager.UserName,
                AddedDate = DateTime.Now,
            };
            if (string.IsNullOrWhiteSpace(viewModel.DetailsJson))
            {
                ModelState.AddModelError("message", "需求明细不能为空");
                return View(viewModel);
            }
            viewModel.Details = JsonConvert.DeserializeObject<List<SelectListItem>>(viewModel.DetailsJson);
            foreach (var item in viewModel.Details)
            {
                if (!string.IsNullOrWhiteSpace(item.Value) && !string.IsNullOrWhiteSpace(item.Text))
                {
                    subject.SubjectDetails.Add(new SubjectDetail()
                    {
                        Id = IdBuilder.CreateIdNum(),
                        Title = item.Text,
                        Type = item.Value,
                        Status = Consts.StateLock
                    });
                }
            }
            if (!subject.SubjectDetails.Any())
            {
                ModelState.AddModelError("message", "需求明细不能为空");
                return View(viewModel);
            }
            //图片
            for (int i = 0; i < viewModel.MaterialImage.Count; i++)
            {
                string remark = null;
                if (!string.IsNullOrWhiteSpace(viewModel.MaterialRemark[i]))
                {
                    remark = viewModel.MaterialRemark[i];
                }
                subject.Attachments.Add(new Attachment
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
            _service.Add(subject);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Detail(string id)
        {
            var entity = _service.GetById(id);
            return PartialView("Detail",entity);
        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            var entity = _service.GetById(id);
            _service.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
        [HttpPost]
        public ActionResult DeleteDetail(string id)
        {
            var entity = _detailService.GetById(id);
            _detailService.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
        [HttpPost]
        public ActionResult Done(string id)
        {
            var entity = _detailService.GetById(id);
            if (string.IsNullOrWhiteSpace(entity.TransactorId)||string.IsNullOrWhiteSpace(entity.ProducerById))
            {
                return Json(new { State = 0, Msg = "需求还未完成！" });
            }
            
            entity.Status = 3;
            entity.CompletDate = DateTime.Now;
            entity.SubjectDetailProgresses.Add(new SubjectDetailProgress()
            {
                Id = IdBuilder.CreateIdNum(),
                Remark = "需求完成",
                UploadDate = DateTime.Now,
                AddedBy = CurrentManager.UserName,
                AddedById = CurrentManager.Id
            });
            _detailService.Update(entity);
            return Json(new { State = 1, Msg = "操作成功" });
        }
        public ActionResult Download(string id)
        {
            var entity = _service.GetById(id);
            var imgs = entity.Attachments.Select(d => new SelectListItem() { Value = Server.MapPath(d.Path), Text = string.IsNullOrWhiteSpace(d.Describe) ? d.Title : d.Describe }).ToList();
            var zip = _fileService.ZipFiles(imgs);
            return File(zip,
                "application/zip", entity.Title + ".zip");
        }
    }
}