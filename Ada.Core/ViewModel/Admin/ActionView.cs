using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Admin
{
   public class ActionView: BaseView
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
        /// 方法
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
    }

    
}
