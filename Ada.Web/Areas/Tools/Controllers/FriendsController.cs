using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Common;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Common;
using Ada.Framework.Filter;
using Ada.Services.Cache;
using Ada.Services.Setting;
using Newtonsoft.Json;
using Tools.Models;

namespace Tools.Controllers
{
    public class FriendsController : BaseController
    {
        private readonly IRepository<Fans> _repository;
        private readonly ISettingService _settingService;
        private readonly ICacheService _cacheService;
        private static Random _random = new Random(DateTime.Now.Second);
        public FriendsController(IRepository<Fans> repository,
            ISettingService settingService,
            ICacheService cacheService)
        {
            _repository = repository;
            _settingService = settingService;
            _cacheService = cacheService;
        }
        public ActionResult Index()
        {
            IList<string> netWorkList = new List<string>() { "4G", "WIFI" };
            IList<string> operatorList = new List<string>() { "中国移动", "中国联通", "中国电信" };

            FriendsSet view = new FriendsSet
            {
                Network = netWorkList[_random.Next(0, netWorkList.Count)],
                Operator = operatorList[_random.Next(0, operatorList.Count)],
                Is24Hour = true,
                PowerIco = true,
                Signal = (short)_random.Next(2, 5),
                Power = (short)_random.Next(1, 100),
                FansRangeMax = _repository.LoadEntities(d => d.IsDelete == false).Count()
            };
            return View(view);
        }
        [HttpPost]
        
        public ActionResult SaveSet(FriendsSet friendsSet)
        {
            var max = _repository.LoadEntities(d => d.IsDelete == false).Count();
            if (friendsSet.FansRangeMax>max)
            {
                return Json(new { State = 0, Msg = "粉丝区间最大值超出粉丝总数" });
            }
            if (friendsSet.FansRangeMax - friendsSet.FansRangeMin < friendsSet.Likes)
            {
                return Json(new { State = 0, Msg = "粉丝区间总数不满足点赞数" });
            }
            var setting = new Ada.Core.Domain.Admin.Setting
            {
                SettingName = typeof(FriendsSet).Name,
                Content = JsonConvert.SerializeObject(friendsSet)
            };
            _settingService.AddOrUpdate(setting);
            return Json(new { State = 1, Msg = "OK" });
        }
        public ActionResult PreviewDetail()
        {
            var friendsSet = _settingService.GetSetting<FriendsSet>();
            friendsSet.FriendContent = new FriendContent();
            var comments = SerializeHelper.DeserializeToObject<FriendContent>(friendsSet.CommentContent);
            friendsSet.FriendContent.Content = comments.Content;
            friendsSet.FriendContent.Likes = comments.Likes;
            friendsSet.FriendContent.Image = comments.Image;
            friendsSet.FriendContent.Type = comments.Type;
            friendsSet.FriendContent.LinkContent = comments.LinkContent;
            friendsSet.FriendContent.PublishDate = comments.PublishDate;
            friendsSet.FriendContent.PublishFans = _repository.LoadEntities(d => d.Id == comments.PublishFansId).FirstOrDefault();
            foreach (var commentsFansMessage in comments.FansMessages)
            {
                var fans = _repository.LoadEntities(d => d.Id == commentsFansMessage.FansId).FirstOrDefault();
                FansMessage msg = new FansMessage
                {
                    Fans = fans,
                    Message = commentsFansMessage.Message,
                    MessageDate = commentsFansMessage.MessageDate,
                    ReplyFans = commentsFansMessage.ReplyFans
                };
                friendsSet.FriendContent.FansMessages.Add(msg);
            }
            var allFans = _repository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Id)
                .Skip(friendsSet.FansRangeMin).Take(friendsSet.FansRangeMax - friendsSet.FansRangeMin);
            ViewBag.Fans = allFans.OrderBy(d => Guid.NewGuid()).Take(friendsSet.FriendContent.Likes).ToList();
            return View(friendsSet);
        }
        [AllowAnonymous]
        public ActionResult Publish(FriendsSet friendsSet)
        {
            var dateRange = friendsSet.PublishDate.Split('至');
            FriendContent friendContent = new FriendContent();
            friendContent.PublishFans = _repository.LoadEntities(d => d.Id == friendsSet.FriendId).FirstOrDefault();
            friendContent.Content = friendsSet.Text;
            friendContent.Likes = friendsSet.Likes;
            friendContent.PublishDate = DateTime.Parse(dateRange[0]);
            friendContent.Type = friendsSet.ContentType;
            friendContent.LinkContent = friendsSet.LinkContent;
            friendContent.Image = friendsSet.Images;
            var fans = GetRandomFans(friendsSet.Comments,friendsSet.FansRangeMin,friendsSet.FansRangeMax);
            foreach (var fan in fans)
            {
                FansMessage fansMessage = new FansMessage();
                fansMessage.Fans = fan;
                fansMessage.MessageDate =
                    GetRandomTime(DateTime.Parse(dateRange[0]), DateTime.Parse(dateRange[1]));
                friendContent.FansMessages.Add(fansMessage);
            }
            return PartialView("ContentPreview", friendContent);
        }

        private List<Fans> GetRandomFans(int count, int min = 0, int max = 0)
        {
            var allFans = _repository.LoadEntities(d => d.IsDelete == false).OrderBy(d => d.Id);
            if (max == 0)
            {
                max = allFans.Count();
            }
            var fans = allFans.Skip(min).Take(max - min).ToList();
            List<Fans> result = new List<Fans>();
            for (int i = 0; i < count; i++)
            {
                var index = _random.Next(0, fans.Count);
                var fan = fans[index];
                result.Add(fan);
            }
            return result;
        }
        /// <summary>
        /// 得到随机日期
        /// </summary>
        /// <param name="startime"></param>
        /// <param name="endtime"></param>
        /// <returns>间隔日期之间的 随机日期</returns>
        private DateTime GetRandomTime(DateTime startime, DateTime endtime)
        {


            DateTime newtime;
            TimeSpan tsp = endtime - startime;
            do
            {
                int days = _random.Next(0, (int)tsp.TotalDays);
                int hours = _random.Next(0, (int)tsp.TotalHours);
                int minutes = _random.Next(0, (int)tsp.TotalMinutes);
                //int seconds = rd.Next(0, tsp.Seconds);
                newtime = endtime.AddDays(-days).AddHours(-hours).AddMinutes(-minutes).AddSeconds(0);
            } while (newtime < startime);

            return newtime;
        }
    }
}