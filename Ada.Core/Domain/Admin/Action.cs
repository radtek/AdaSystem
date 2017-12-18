using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Admin
{
    public class Action : BaseEntity
    {
        public Action()
        {
            this.Roles = new HashSet<Role>();
            this.ManagerActions = new HashSet<ManagerAction>();
        }

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
        /// 图片
        /// </summary>
        [Display(Name = "图片")]
        public string Image { get; set; }

        /// <summary>
        /// 是否节点
        /// </summary>
        [Display(Name = "是否节点")]
        public bool? IsLeaf { get; set; }

        /// <summary>
        /// 上级权限
        /// </summary>
        [Display(Name = "上级权限")]
        public string ParentId { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        [Display(Name = "级别")]
        public int? Level { get; set; }

        /// <summary>
        /// 树路径
        /// </summary>
        [Display(Name = "树路径")]
        public string TreePath { get; set; }

        /// <summary>
        /// 显示按钮
        /// </summary>
        [Display(Name = "显示按钮")]
        public bool? IsButton { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [Display(Name = "编号")]
        public int? Number { get; set; }

        /// <summary>
        /// 权限分类
        /// </summary>
        [Display(Name = "权限分类")]
        public string ActionType { get; set; }

        /// <summary>
        /// 是否菜单
        /// </summary>
        [Display(Name = "是否菜单")]
        public bool? IsMenu { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<ManagerAction> ManagerActions { get; set; }
    }
}
