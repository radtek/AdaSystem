using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Common;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Common;
using Ada.Framework.Filter;
using Tools.Models;

namespace Tools.Controllers
{
    public class FriendsController : BaseController
    {
        private readonly IRepository<Fans> _repository;
        public FriendsController(IRepository<Fans> repository)
        {
            _repository = repository;
        }
        public ActionResult Index()
        {
            FriendsSet view = new FriendsSet();
            view.Network = "4G";
            view.Operator = "中国移动";
            return View(view);
        }
        [AllowAnonymous]
        public ActionResult Preview()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Publish(FriendsSet friendsSet)
        {
            var dateRange = friendsSet.PublishDate.Split('至');
            FriendContent friendContent=new FriendContent();
            friendContent.PublishFans = _repository.LoadEntities(d => d.Id == friendsSet.FriendId).FirstOrDefault();
            friendContent.Content = friendsSet.Text;
            friendContent.Likes = friendsSet.Likes;
            friendContent.PublishDate = DateTime.Parse(dateRange[0]);
            var fans = _repository.LoadEntities(d => d.IsDelete == false).Take(friendsSet.Comments).ToList();
            foreach (var fan in fans)
            {
                FansMessage fansMessage=new FansMessage();
                fansMessage.Fans = fan;
                fansMessage.MessageDate =
                    Utils.GetRandomTime(DateTime.Parse(dateRange[0]), DateTime.Parse(dateRange[1]),new Random(DateTime.Now.Second));
                friendContent.FansMessages.Add(fansMessage);
            }
            return PartialView("ContentPreview", friendContent);
        }
    }
}