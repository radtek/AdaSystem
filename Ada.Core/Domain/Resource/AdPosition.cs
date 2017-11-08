using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Resource
{
   public class AdPosition:BaseEntity
    {
        /// <summary>
        /// 位置名称
        /// </summary>
        [Display(Name = "位置名称")]
        public string Name { get; set; }
        /// <summary>
        /// 媒体类型
        /// </summary>
        [Display(Name = "媒体类型")]
        public string MediaTypeId { get; set; }
        public virtual MediaType MediaType { get; set; }
    }
}
