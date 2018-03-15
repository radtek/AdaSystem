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
            AdPositions=new HashSet<AdPosition>();
            MediaDevelops=new HashSet<MediaDevelop>();
        }
        /// <summary>
        /// 类型名称
        /// </summary>
        [Display(Name = "类型名称")]
        public string TypeName { get; set; }
        /// <summary>
        /// 调用别名
        /// </summary>
        [Display(Name = "调用别名")]
        public string CallIndex { get; set; }
        /// <summary>
        /// 父级类型
        /// </summary>
        [Display(Name = "父级类型")]
        public string ParentId { get; set; }
        /// <summary>
        /// 封面图片
        /// </summary>
        [Display(Name = "封面图片")]
        public string Image { get; set; }
        /// <summary>
        /// 是否评论
        /// </summary>
        [Display(Name = "是否评论")]
        public bool? IsComment { get; set; }

        public virtual ICollection<Media> Medias { get; set; }
        public virtual ICollection<MediaDevelop> MediaDevelops { get; set; }
        public virtual ICollection<AdPosition> AdPositions { get; set; }
    }
}
