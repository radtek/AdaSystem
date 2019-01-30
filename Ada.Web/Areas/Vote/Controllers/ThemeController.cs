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
    public class ThemeController : BaseController
    {
        private readonly IVoteThemeService _service;

        public ThemeController(IVoteThemeService service)
        {
            _service = service;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(VoteThemeView viewModel)
        {
            var result = _service.LoadEntitiesFilter(viewModel).ToList();
            return Json(new
            {
                viewModel.total,
                rows = result.Select(d => new
                {
                    d.Id,
                    d.Title,
                    d.Status,
                    d.StartDate,
                    d.EndDate

                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            return View(new VoteThemeView());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(VoteThemeView viewModel)
        {
            VoteTheme voteTheme = new VoteTheme();
            voteTheme.Id = IdBuilder.CreateIdNum();
            voteTheme.Title = viewModel.Title;
            voteTheme.CallIndex = viewModel.CallIndex;
            voteTheme.StartDate = viewModel.StartDate;
            voteTheme.EndDate = viewModel.EndDate;
            voteTheme.CoverStart = viewModel.CoverStart;
            voteTheme.CoverEnd = viewModel.CoverEnd;
            voteTheme.Abstract = viewModel.Abstract;
            voteTheme.Rule = viewModel.Rule;
            voteTheme.Content = viewModel.Content;
            voteTheme.EndTitle = viewModel.EndTitle;
            voteTheme.EndContent = viewModel.EndContent;
            voteTheme.Status = viewModel.Status;
            //voteTheme.Config = viewModel.Config;
            voteTheme.KeyWord = viewModel.KeyWord;



            voteTheme.AddedDate = DateTime.Now;
            voteTheme.AddedBy = CurrentManager.UserName;
            voteTheme.AddedById = CurrentManager.Id;
            _service.Add(voteTheme);
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
            VoteThemeView viewModel = new VoteThemeView();
            viewModel.Id = id;
            viewModel.Title = entity.Title;
            viewModel.CallIndex = entity.CallIndex;
            viewModel.StartDate = entity.StartDate;
            viewModel.EndDate = entity.EndDate;
            viewModel.CoverStart = entity.CoverStart;
            viewModel.CoverEnd = entity.CoverEnd;
            viewModel.Abstract = entity.Abstract;
            viewModel.Rule = entity.Rule;
            viewModel.Content = entity.Content;
            viewModel.EndTitle = entity.EndTitle;
            viewModel.EndContent = entity.EndContent;
            viewModel.Status = entity.Status;
            //viewModel.Config = entity.Config;
            viewModel.KeyWord = entity.KeyWord;
  
            return View(viewModel);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Update(VoteThemeView viewModel)
        {
            var entity = _service.GetById(viewModel.Id);
            entity.Title = viewModel.Title;
            entity.CallIndex = viewModel.CallIndex;
            entity.StartDate = viewModel.StartDate;
            entity.EndDate = viewModel.EndDate;
            entity.CoverStart = viewModel.CoverStart;
            entity.CoverEnd = viewModel.CoverEnd;
            entity.Abstract = viewModel.Abstract;
            entity.Rule = viewModel.Rule;
            entity.Content = viewModel.Content;
            entity.EndTitle = viewModel.EndTitle;
            entity.EndContent = viewModel.EndContent;
            entity.Status = viewModel.Status;
            //entity.Config = viewModel.Config;
            entity.KeyWord = viewModel.KeyWord;



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