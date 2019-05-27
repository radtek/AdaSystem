using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Files.Models
{
    public class PostView
    {
        public PostView()
        {
            water = false;
            thumbnail = false;
            input = "upfile";
        }
        public string input { get; set; }
        public bool water { get; set; }
        public bool thumbnail { get; set; }
    }
}