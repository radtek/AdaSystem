using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core
{
    
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            IsDelete = false;
            Taxis = 99;
        }
        /// <summary>
        /// 主键
        /// </summary>
        [Display(Name = "主键")]
        public string Id { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [Display(Name = "添加时间")]
        public DateTime? AddedDate { get; set; }
        /// <summary>
        /// 添加者
        /// </summary>
        [Display(Name = "添加者")]
        public string AddedBy { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [Display(Name = "修改时间")]
        public DateTime? ModifiedDate { get; set; }
        /// <summary>
        /// 修改者
        /// </summary>
        [Display(Name = "修改者")]
        public string ModifiedBy { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [Display(Name = "是否删除")]
        public bool IsDelete { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        [Display(Name = "删除时间")]
        public DateTime? DeletedDate { get; set; }
        /// <summary>
        /// 删除者
        /// </summary>
        [Display(Name = "删除者")]
        public string DeletedBy { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        [Display(Name = "IP地址")]
        public string IpAddress { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? Taxis { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}
