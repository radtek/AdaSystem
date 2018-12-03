using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Crawler.Models
{
    public class ApiRequest
    {
        [Display(Name = "API地址")]
        public string UrlPath { get; set; }
        [Display(Name = "Form参数")]
        public string Content { get; set; }

    }
}