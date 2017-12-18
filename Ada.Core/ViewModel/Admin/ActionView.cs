using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Admin
{
   public class ActionView
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        [Display(Name = "权限名称")]
        public string ActionName { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        [Display(Name = "区域")]
        public string Area { get; set; }
        /// <summary>
        /// 控制器
        /// </summary>
        [Display(Name = "控制器")]
        public string ControllerName { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        [Display(Name = "方法")]
        public string MethodName { get; set; }

        /// <summary>
        /// HTTP方法
        /// </summary>
        [Display(Name = "请求方法")]
        public string HttpMethod { get; set; }
        /// <summary>
        /// 外部链接
        /// </summary>
        [Display(Name = "外部链接")]
        public string LinkUrl { get; set; }

        /// <summary>
        /// 图标样式
        /// </summary>
        [Display(Name = "图标样式")]
        public string IconCls { get; set; }
        /// <summary>
        /// 父级节点
        /// </summary>
        [Display(Name = "父级节点")]
        public string ParentId { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        [Display(Name = "主键")]
        public string Id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? Taxis { get; set; }
        /// <summary>
        /// 是否按钮
        /// </summary>
        [Display(Name = "是否按钮")]
        public bool? IsButton { get; set; }
        /// <summary>
        /// 是否菜单
        /// </summary>
        [Display(Name = "是否菜单")]
        public bool? IsMenu { get; set; }
        /// <summary>
        /// 是否同时添加常用权限
        /// </summary>
        [Display(Name = "常用权限")]
        public bool? IsCURD { get; set; }
    }

    
}
