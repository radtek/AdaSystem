using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Resource
{
    public class MediaArticleView : BaseView
    {

        /// <summary>
        /// 文章标题
        /// </summary>
        [Display(Name = "文章标题")]
        public string Title { get; set; }
        /// <summary>
        /// 文章链接
        /// </summary>
        [Display(Name = "文章链接")]
        public string ArticleUrl { get; set; }

        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaId { get; set; }

    }
}
