using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Demand
{
   public class SubjectDetailView: BaseView
    {
        /// <summary>
        /// 人物名称
        /// </summary>
        [Display(Name = "人物名称")]
        public string Title { get; set; }
        /// <summary>
        /// 人物类型
        /// </summary>
        [Display(Name = "人物类型")]
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
        /// 完成日期开始
        /// </summary>
        [Display(Name = "完成日期开始")]
        public DateTime? CompletDateStart { get; set; }
        /// <summary>
        /// 完成日期结束
        /// </summary>
        [Display(Name = "完成日期结束")]
        public DateTime? CompletDateEnd { get; set; }
        /// <summary>
        /// 需求项目
        /// </summary>
        [Display(Name = "需求项目")]
        public string SubjectId { get; set; }
        /// <summary>
        /// 发布者
        /// </summary>
        [Display(Name = "发布者")]
        public string AddedBy { get; set; }
        /// <summary>
        /// 发布日期
        /// </summary>
        [Display(Name = "发布日期")]
        public DateTime? AddedDate { get; set; }
        /// <summary>
        /// 是否编辑人员
        /// </summary>
        [Display(Name = "是否编辑人员")]
        public bool? IsProducer { get; set; }
        /// <summary>
        /// 是否发布人员
        /// </summary>
        [Display(Name = "是否发布人员")]
        public bool? IsDo { get; set; }
        /// <summary>
        /// 是否编辑人员
        /// </summary>
        [Display(Name = "是否编辑人员")]
        public bool? IsSelfProducer { get; set; }
        /// <summary>
        /// 是否发布人员
        /// </summary>
        [Display(Name = "是否发布人员")]
        public bool? IsSelfDo { get; set; }
    }
}
