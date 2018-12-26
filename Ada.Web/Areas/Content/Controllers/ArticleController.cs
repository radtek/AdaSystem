using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Content;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Content;
using Ada.Framework.Filter;
using Ada.Framework.UploadFile;
using Ada.Services.Content;

namespace Content.Controllers
{
    public class ArticleController : BaseController
    {
        private readonly IRepository<Article> _repository;
        private readonly IRepository<Column> _columnRepository;
        private readonly IArticleService _service;
        public ArticleController(IRepository<Article> repository, 
            IArticleService service,
            IRepository<Column> columnRepository)
        {
            _service = service;
            _repository = repository;
            _columnRepository = columnRepository;
        }
        public ActionResult Index()
        {
            var entities = _columnRepository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis).ToList();
            ViewBag.Trees = GetTree(null, entities);
            return View();
        }
        private List<TreeView> GetTree(string parentId, List<Column> entities)
        {
            var newlist = entities.Where(d => d.ParentId == parentId).ToList();
            List<TreeView> treeViews = new List<TreeView>();
            newlist.ForEach(item =>
            {
                treeViews.Add(new TreeView
                {
                    Id = item.Id,
                    Children = GetTree(item.Id, entities),
                    ParentId = item.ParentId,
                    Text = item.Title
                });
            });
            return treeViews;
        }
        public ActionResult GetList(ArticleView viewModel)
        {
            viewModel.Managers = PremissionData();
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new ArticleView()
                {
                    Id = d.Id,
                    Title = d.Title,
                    ColumnName = d.Column.Title,
                    Status = d.Status,
                    Summary = d.Summary
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add(string id)
        {
            var entities = _columnRepository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis).ToList();
            ViewBag.Trees = GetTree(null, entities);
            return View(new ArticleView(){Taxis = 99,ColumnId = id,Status = 1});
        }
        [HttpPost,ValidateInput(false)]
        
        public ActionResult Add(ArticleView viewModel)
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
            Article entity = new Article
            {
                Id = IdBuilder.CreateIdNum(),
                AddedById = CurrentManager.Id,
                AddedBy = CurrentManager.UserName,
                AddedDate = DateTime.Now,
                Title = viewModel.Title,
                Content = viewModel.Content,
                Type = viewModel.Type,
                ColumnId = viewModel.ColumnId,
                CoverPic = idc["CoverPic"]?.ToString(),
                Taxis = viewModel.Taxis,
                IsHot = viewModel.IsHot,
                IsPush = viewModel.IsPush,
                IsRecommend = viewModel.IsRecommend,
                IsTop = viewModel.IsTop,
                IsSlide = viewModel.IsSlide,
                Status = viewModel.Status,
                Url = viewModel.Url,
                Summary = Utils.DropHtml(string.IsNullOrWhiteSpace(viewModel.Summary) ? viewModel.Content : viewModel.Summary, 255),
                Author = viewModel.Author
            };
            _service.Add(entity);
            TempData["Msg"] = "添加成功";
            return RedirectToAction("Index");
        }
        public ActionResult Update(string id)
        {
            var entities = _columnRepository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Taxis).ToList();
            ViewBag.Trees = GetTree(null, entities);
            var item = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            ArticleView entity = new ArticleView
            {
                Id = item.Id,
                Title = item.Title,
                CoverPic = item.CoverPic,
                Type = item.Type,
                ColumnId = item.ColumnId,
                Content = item.Content,
                Taxis = item.Taxis,
                IsHot = item.IsHot,
                IsPush = item.IsPush,
                IsRecommend = item.IsRecommend,
                IsTop = item.IsTop,
                IsSlide = item.IsSlide,
                Status = item.Status,
                Url = item.Url,
                Summary = item.Summary,
                Author = item.Author

            };
            return View(entity);
        }
        [HttpPost, ValidateInput(false)]
        
        public ActionResult Update(ArticleView viewModel)
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
            entity.Title = viewModel.Title;
            entity.Type = viewModel.Type;
            entity.CoverPic = idc["CoverPic"] == null ? entity.CoverPic : idc["CoverPic"]?.ToString();
            entity.ColumnId = viewModel.ColumnId;
            entity.Content = viewModel.Content;
            entity.Taxis = viewModel.Taxis;
            entity.IsHot = viewModel.IsHot;
            entity.IsPush = viewModel.IsPush;
            entity.IsRecommend = viewModel.IsRecommend;
            entity.IsTop = viewModel.IsTop;
            entity.IsSlide = viewModel.IsSlide;
            entity.Status = viewModel.Status;
            entity.Url = viewModel.Url;
            entity.Summary = Utils.DropHtml(string.IsNullOrWhiteSpace(viewModel.Summary) ? viewModel.Content : viewModel.Summary, 255);
            entity.Author = viewModel.Author;
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
        [HttpPost]
        
        public ActionResult Publish(string id)
        {
            var entity = _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
            if (entity.Status==1)
            {
                entity.Status = null;
            }
            else
            {
                entity.Status = 1;
            }
            _service.Update(entity);
            return Json(new { State = 1, Msg = "操作成功" });
        }
       
    }
}