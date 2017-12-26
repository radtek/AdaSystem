using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Resource
{
    /// <summary>
    /// 媒体组
    /// </summary>
    public class MediaGroup : BaseEntity
    {
        public MediaGroup()
        {
            Medias=new HashSet<Media>();
        }
        /// <summary>
        /// 媒体组名
        /// </summary>
        [Display(Name = "媒体组名")]
        public string GroupName { get; set; }
        public virtual ICollection<Media> Medias { get; set; }
    }
}
