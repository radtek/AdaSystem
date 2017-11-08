using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Resource
{
   public class MediaTag:BaseEntity
    {
        public MediaTag()
        {
            Medias=new HashSet<Media>();
        }
        /// <summary>
        /// 媒体标签
        /// </summary>
        [Display(Name = "媒体标签")]
        public string TagName { get; set; }
        /// <summary>
        /// 热门标签
        /// </summary>
        [Display(Name = "热门标签")]
        public bool? IsHot { get; set; }
        public virtual ICollection<Media> Medias { get; set; }
    }
}
