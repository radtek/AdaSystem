using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WeiXin.Models
{
    public class TemplateMsgModel
    {
        public TemplateMsgModel()
        {
            KeyWords = new Dictionary<string, string>();
            Templates=new List<SelectListItem>();
        }
        public string Title { get; set; }
        public string KeyWord1 { get; set; }
        public string KeyWord2 { get; set; }
        public string KeyWord3 { get; set; }
        public string KeyWord4 { get; set; }
        public string KeyWord5 { get; set; }
        public string Remark { get; set; }
        public IDictionary<string, string> KeyWords { get; set; }
        public string Url { get; set; }
        public string OpenIds { get; set; }
        public string AppId { get; set; }
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }
        public IEnumerable<SelectListItem> Templates { get; set; }
    }
}