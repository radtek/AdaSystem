using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Resource
{
    public class MediaDevelop : BaseEntity
    {
        public MediaDevelop()
        {
            MediaDevelopProgresses =new HashSet<MediaDevelopProgress>();
        }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaName { get; set; }
        /// <summary>
        /// 媒体ID
        /// </summary>
        [Display(Name = "媒体ID")]
        public string MediaID { get; set; }
        /// <summary>
        /// 平台
        /// </summary>
        [Display(Name = "平台")]
        public string Platform { get; set; }
        /// <summary>
        /// 开发说明
        /// </summary>
        [Display(Name = "开发说明")]
        public string Content { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        [Display(Name = "申请人")]
        public string SubBy { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        [Display(Name = "申请人")]
        public string SubById { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [Display(Name = "申请日期")]
        public DateTime? SubDate { get; set; }
        /// <summary>
        /// 认领媒介
        /// </summary>
        [Display(Name = "认领媒介")]
        public string Transactor { get; set; }
        /// <summary>
        /// 认领媒介
        /// </summary>
        [Display(Name = "认领媒介")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 认领日期
        /// </summary>
        [Display(Name = "认领日期")]
        public DateTime? GetDate { get; set; }
        /// <summary>
        /// 开发状态
        /// </summary>
        [Display(Name = "开发状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        [Display(Name = "审核状态")]
        public short? AuditStatus { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        [Display(Name = "审核人")]
        public string AuditBy { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        [Display(Name = "审核人")]
        public string AuditById { get; set; }
        /// <summary>
        /// 入库日期
        /// </summary>
        [Display(Name = "入库日期")]
        public DateTime? AuditDate { get; set; }
        /// <summary>
        /// 媒体类型
        /// </summary>
        [Display(Name = "媒体类型")]
        public string MediaTypeId { get; set; }
        public virtual MediaType MediaType { get; set; }
        public virtual ICollection<MediaDevelopProgress> MediaDevelopProgresses { get; set; }
    }
}
