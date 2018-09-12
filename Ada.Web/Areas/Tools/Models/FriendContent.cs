using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ada.Core.Domain.Common;

namespace Tools.Models
{
    public class FriendContent
    {
        public FriendContent()
        {
            FansMessages = new List<FansMessage>();
            Likes = 0;
            PublishDate = DateTime.Now;
        }
        public Fans PublishFans { get; set; }
        public string PublishFansId { get; set; }
        public DateTime PublishDate { get; set; }
        public int Likes { get; set; }
        public string Content { get; set; }
        public string LinkContent { get; set; }
        public string Image { get; set; }
        public string Type { get; set; }
        public List<FansMessage> FansMessages { get; set; }
    }

    public class FansMessage
    {
        public Fans Fans { get; set; }
        public DateTime MessageDate { get; set; }
        public string FansId { get; set; }
        public string ReplyFans { get; set; }
        public string Message { get; set; }

    }
}