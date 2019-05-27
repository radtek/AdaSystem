using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Demand
{
   public class SubjectDetail: BaseEntity
    {
        public SubjectDetail()
        {
            SubjectDetailProgresses=new HashSet<SubjectDetailProgress>();
        }
        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        public string Title { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [Display(Name = "类型")]
        public string Type { get; set; }
        /// <summary>
        /// 认领人员
        /// </summary>
        [Display(Name = "认领人员")]
        public string Transactor { get; set; }
        /// <summary>
        /// 认领人员
        /// </summary>
        [Display(Name = "认领人员")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 认领日期
        /// </summary>
        [Display(Name = "认领日期")]
        public DateTime? GetDate { get; set; }
        /// <summary>
        /// 需求状态
        /// </summary>
        [Display(Name = "需求状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 编辑人员
        /// </summary>
        [Display(Name = "编辑人员")]
        public string ProducerBy { get; set; }
        /// <summary>
        /// 编辑人员
        /// </summary>
        [Display(Name = "编辑人员")]
        public string ProducerById { get; set; }
        /// <summary>
        /// 编辑日期
        /// </summary>
        [Display(Name = "编辑日期")]
        public DateTime? ProducerDate { get; set; }
        /// <summary>
        /// 完成日期
        /// </summary>
        [Display(Name = "完成日期")]
        public DateTime? CompletDate { get; set; }
        /// <summary>
        /// 需求项目
        /// </summary>
        [Display(Name = "需求项目")]
        public string SubjectId { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ICollection<SubjectDetailProgress> SubjectDetailProgresses { get; set; }
    }
}
