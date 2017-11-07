using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Resource
{
  public class MediaType:BaseEntity
    {
        public MediaType()
        {
            Medias=new HashSet<Media>();
        }
        /// <summary>
        /// 类型名称
        /// </summary>
        [Display(Name = "类型名称")]
        public string TypeName { get; set; }
        /// <summary>
        /// 父级类型
        /// </summary>
        [Display(Name = "父级类型")]
        public string ParentId { get; set; }

        public virtual ICollection<Media> Medias { get; set; }
    }
}
