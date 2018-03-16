using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Resource
{
    public class MediaDevelopProgressView
    {
        /// <summary>
        /// 进度内容
        /// </summary>
        [Display(Name = "进度内容")]
        public string ProgressContent { get; set; }
        /// <summary>
        /// 媒体开发
        /// </summary>
        [Display(Name = "媒体开发")]
        public string MediaDevelopId { get; set; }
    }
}
