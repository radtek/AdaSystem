using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Vote;
using Ada.Core.ViewModel.Vote;
using Ada.Framework.Filter;
using Ada.Services.Vote;

namespace Vote.Controllers
{
    public class ItemController : BaseController
    {
        private readonly IVoteItemService _service;

        public ItemController(IVoteItemService service)
        {
            _service = service;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(VoteItemView viewModel)
        {
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new VoteItemView
                {
                    Id = d.Id,
                    Title = d.Title,
                    Taxis = d.Taxis,
                    TotalCount = d.TotalCount,
                    VoteThemeTitle = d.VoteTheme.Title

                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            return View(new VoteItemView());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(VoteItemView viewModel)
        {
            VoteItem voteItem = new VoteItem();
            voteItem.Id = IdBuilder.CreateIdNum();
            voteItem.Title = viewModel.Title;
            voteItem.Url = viewModel.Url;
            voteItem.Abstract = viewModel.Abstract;
            voteItem.Image = viewModel.Image;
            voteItem.IsTop = viewModel.IsTop;
            voteItem.Content = viewModel.Content;
            voteItem.Taxis = viewModel.Taxis;
            voteItem.Status = viewModel.Status;
            voteItem.VoteThemeId = viewModel.VoteThemeId;


            voteItem.AddedDate = DateTime.Now;
            voteItem.AddedBy = CurrentManager.UserName;
            voteItem.AddedById = CurrentManager.Id;
            _service.Add(voteItem);
            TempData["Msg"] = "保存成功";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult Update(string id)
        {
            var entity = _service.GetById(id);
            VoteItemView viewModel = new VoteItemView();
            viewModel.Id = id;
            viewModel.Title = entity.Title;
            viewModel.Url = entity.Url;
            viewModel.Abstract = entity.Abstract;
            viewModel.Image = entity.Image;
            viewModel.IsTop = entity.IsTop;
            viewModel.Content = entity.Content;
            viewModel.Taxis = entity.Taxis;
            viewModel.Status = entity.Status;
            viewModel.VoteThemeId = entity.VoteThemeId;
            return View(viewModel);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Update(VoteItemView viewModel)
        {
            var entity = _service.GetById(viewModel.Id);
            entity.Title = viewModel.Title;
            entity.Url = viewModel.Url;
            entity.Abstract = viewModel.Abstract;
            entity.Image = viewModel.Image;
            entity.IsTop = viewModel.IsTop;
            entity.Content = viewModel.Content;
            entity.Taxis = viewModel.Taxis;
            entity.Status = viewModel.Status;
            entity.VoteThemeId = viewModel.VoteThemeId;

            entity.ModifiedBy = CurrentManager.UserName;
            entity.ModifiedById = CurrentManager.Id;
            entity.ModifiedDate = DateTime.Now;
            _service.Update(entity);
            TempData["Msg"] = "保存成功";
            return RedirectToAction("Index");
        }
        [HttpPost]

        public ActionResult Delete(string id)
        {
            var entity = _service.GetById(id);
            _service.Delete(entity);
            return Json(new { State = 1, Msg = "删除成功" });
        }
    }
}