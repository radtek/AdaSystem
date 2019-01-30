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
    public class LaunchController : BaseController
    {
        private readonly IVoteThemeService _voteThemeService;
        private readonly IVoteItemService _voteItemService;
        public LaunchController(IVoteThemeService voteThemeService, IVoteItemService voteItemService)
        {
            _voteThemeService = voteThemeService;
            _voteItemService = voteItemService;
        }
        public ActionResult Index()
        {
            var themes = _voteThemeService.LoadEntitiesFilter(new VoteThemeView() { limit = int.MaxValue }).ToList();
            return View(themes);
        }

        public ActionResult List(string id)
        {
            var theme = _voteThemeService.GetById(id);
            return View(theme);
        }
        [HttpPost]
        public ActionResult Vote(string id, int score = 0)
        {
            var item = _voteItemService.GetById(id);
            //验证是否关闭
            if (!item.VoteTheme.Status)
            {
                return Json(new { State = 0, Msg = "投票主题暂未开启" });
            }
            //验证是否在投票期间
            if (DateTime.Now > item.VoteTheme.EndDate || DateTime.Now < item.VoteTheme.StartDate)
            {
                return Json(new { State = 0, Msg = "不在投票日期内" });
            }
            //是否已经投票
            if (item.VoteItemRecords.Any(d => d.UID == CurrentManager.Id))
            {
                return Json(new { State = 0, Msg = "您已投过票" });
            }
            item.VoteItemRecords.Add(new VoteItemRecord()
            {
                Id = IdBuilder.CreateIdNum(),
                Name = CurrentManager.UserName,
                UID = CurrentManager.Id,
                Date = DateTime.Now,
                Score = score
            });
            item.TotalCount = item.TotalCount + score;
            _voteItemService.Update(item);
            return Json(new { State = 1, Msg = "投票成功！" });
        }
    }
}