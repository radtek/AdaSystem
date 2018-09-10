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
using Tools.Models;

namespace Tools.Controllers
{
    public class FriendsController : BaseController
    {
        private readonly IRepository<Fans> _repository;
        private static Random _random=new Random(DateTime.Now.Second);
        public FriendsController(IRepository<Fans> repository)
        {
            _repository = repository;
        }
        public ActionResult Index()
        {
            FriendsSet view = new FriendsSet
            {
                Network = "4G",
                Operator = "中国移动"
            };
            return View(view);
        }
        [HttpPost]
        public ActionResult Preview(FriendsSet friendsSet)
        {

            return View();
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
            var fans = GetRandomFans(friendsSet.Comments);
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

        private List<Fans> GetRandomFans(int count)
        {
           var fans= _repository.LoadEntities(d => d.IsDelete == false).ToList();
            List<Fans> result=new List<Fans>();
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