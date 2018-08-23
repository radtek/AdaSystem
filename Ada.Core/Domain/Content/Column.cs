using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Content
{
   public class Column : BaseEntity
    {
        public Column()
        {
            Articles = new HashSet<Article>();
        }
        /// <summary>
        /// 栏目名称
        /// </summary>
        [Display(Name = "栏目名称")]
        public string Title { get; set; }

        /// <summary>
        /// 上级栏目
        /// </summary>
        [Display(Name = "上级栏目")]
        public string ParentId { get; set; }

        /// <summary>
        /// 是否节点
        /// </summary>
        [Display(Name = "是否节点")]
        public bool? IsLeaf { get; set; }

        /// <summary>
        /// 节点级别
        /// </summary>
        [Display(Name = "节点级别")]
        public int? Level { get; set; }

        /// <summary>
        /// 树路径
        /// </summary>
        [Display(Name = "树路径")]
        public string TreePath { get; set; }
        /// <summary>
        /// 栏目简介
        /// </summary>
        [Display(Name = "栏目简介")]
        public string Content { get; set; }

        /// <summary>
        /// 栏目封面
        /// </summary>
        [Display(Name = "栏目封面")]
        public string CoverPic { get; set; }

        /// <summary>
        /// 跳转链接
        /// </summary>
        [Display(Name = "跳转链接")]
        public string Url { get; set; }

        /// <summary>
        /// 是否置首
        /// </summary>
        [Display(Name = "是否置首")]
        public short? IsTop { get; set; }

        /// <summary>
        /// 调用别名
        /// </summary>
        [Display(Name = "调用别名")]
        public string CallIndex { get; set; }

        /// <summary>
        /// 栏目类别
        /// </summary>
        [Display(Name = "栏目类别")]
        public string Type { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
    }
}
