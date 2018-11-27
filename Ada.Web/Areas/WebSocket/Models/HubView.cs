using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSocket.Models
{
    public class HubView
    {
        public HubView()
        {
            Date=DateTime.Now;
        }
        public string MessageType { get; set; }
        public string Message { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
    }
}