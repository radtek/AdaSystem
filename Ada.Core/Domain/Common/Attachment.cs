using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Content;
using Ada.Core.Domain.Demand;
using Ada.Core.Domain.WorkFlow;

namespace Ada.Core.Domain.Common
{
   public class Attachment:BaseEntity
    {
        public Attachment()
        {
            Articles=new HashSet<Article>();
            WorkFlowRecords=new HashSet<WorkFlowRecord>();
            Subjects= new HashSet<Subject>();
            SubjectDetailProgresses = new HashSet<SubjectDetailProgress>();
        }
        /// <summary>
        /// 附件名称
        /// </summary>
        [Display(Name = "名称")]
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        public string Describe { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [Display(Name = "路径")]
        public string Path { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        [Display(Name = "文件大小")]
        public int? FileSize { get; set; }

        /// <summary>
        /// 扩展名
        /// </summary>
        [Display(Name = "扩展名")]
        public string FileExt { get; set; }

        /// <summary>
        /// 播放/查看/下载次数
        /// </summary>
        [Display(Name = "播放/查看/下载次数")]
        public int? Times { get; set; }
        /// <summary>
        /// 缩略图路径
        /// </summary>
        [Display(Name = "缩略图路径")]
        public string ThumbPath { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
        public virtual ICollection<WorkFlowRecord> WorkFlowRecords { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
        public virtual ICollection<SubjectDetailProgress> SubjectDetailProgresses { get; set; }
    }
}
