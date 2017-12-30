using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Admin
{
  public class Setting:BaseEntity
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        [Display(Name = "配置名称")]
        public string SettingName { get; set; }
        /// <summary>
        /// 设置内容
        /// </summary>
        [Display(Name = "设置内容")]
        public string Content { get; set; }
    }
}
