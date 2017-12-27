using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ada.Core.ViewModel.Resource
{
    public class MediaGroupView : BaseView
    {
        /// <summary>
        /// 媒体组名
        /// </summary>
        [Display(Name = "媒体组名")]
        public string GroupName { get; set; }
        /// <summary>
        /// 媒体资源
        /// </summary>
        [Display(Name = "媒体资源")]
        public List<string> Medias { get; set; }
        /// <summary>
        /// 媒体资源
        /// </summary>
        [Display(Name = "媒体资源")]
        public string MediaData { get; set; }
        /// <summary>
        /// 媒体资源
        /// </summary>
        [Display(Name = "媒体资源")]
        public List<MediaView> MediaViews { get; set; }
    }
}
