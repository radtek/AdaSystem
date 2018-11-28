using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSocket.Models
{
    public class OnLineUser
    {
        public OnLineUser()
        {
            Date = DateTime.Now;
        }
        public string Name { get; set; }
        public string Image { get; set; }
        public string UId { get; set; }
        public DateTime Date { get; set; }
    }
}