using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Admin
{
   public class Menu: BaseEntity
    {
        /// <summary>
        /// 是否节点
        /// </summary>
        [Display(Name = "是否节点")]
        public bool? IsLeaf { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        [Display(Name = "级别")]
        public int? Level { get; set; }

        /// <summary>
        /// 数路径
        /// </summary>
        [Display(Name = "树路径")]
        public string TreePath { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [Display(Name = "菜单名称")]
        public string MenuName { get; set; }

        /// <summary>
        /// 菜单地址
        /// </summary>
        [Display(Name = "菜单地址")]
        public string ActionInfoId { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        [Display(Name = "是否显示")]
        public bool? IsVisable { get; set; }

        /// <summary>
        /// 图标样式
        /// </summary>
        [Display(Name = "图标样式")]
        public string IconCls { get; set; }

        /// <summary>
        /// 菜单图片
        /// </summary>
        [Display(Name = "菜单图片")]
        public string Image { get; set; }

        /// <summary>
        /// 菜单父级
        /// </summary>
        [Display(Name = "菜单父级")]
        public string ParentId { get; set; }
    }
}
