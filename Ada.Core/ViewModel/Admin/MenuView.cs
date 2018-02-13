using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Admin
{
    public class MenuView
    {
        [Display(Name = "操作")]
        public string Id { get; set; }
        [Display(Name = "菜单名称")]
        public string Name { get; set; }
        [Display(Name = "上级菜单")]
        public string ParentId { get; set; }
        [Display(Name = "是否节点")]
        public bool? IsLeaf { get; set; }
        [Display(Name = "级别")]
        public int? Level { get; set; }
        [Display(Name = "树路径")]
        public string TreePath { get; set; }
        [Display(Name = "链接地址")]
        public Url Url { get; set; }
        [Display(Name = "菜单地址")]
        public string ActionId { get; set; }
        [Display(Name = "图标样式")]
        public string IconCls { get; set; }
        [Display(Name = "排序")]
        public int? Taxis { get; set; }
        [Display(Name = "隐藏")]
        public bool? IsVisable { get; set; }
        [Display(Name = "是否新窗口")]
        public bool? IsBlank { get; set; }

    }

    public class Url
    {
        public string Area { get; set; }
        public string Colltroller { get; set; }
        public string Action { get; set; }
        public string LinkUrl { get; set; }
    }
}
