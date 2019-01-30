using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Vote
{
    public class VoteItem : BaseEntity
    {
        public VoteItem()
        {
            IsTop = false;
            Status = true;
            TotalCount = 0;
            VoteItemRecords=new HashSet<VoteItemRecord>();
        }
        /// <summary>
        /// 选项名称
        /// </summary>
        [Display(Name = "选项名称")]
        public string Title { get; set; }
        /// <summary>
        /// 外链
        /// </summary>
        [Display(Name = "外链")]
        public string Url { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        [Display(Name = "摘要")]
        public string Abstract { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        [Display(Name = "图片")]
        public string Image { get; set; }
        /// <summary>
        /// 置顶
        /// </summary>
        [Display(Name = "置顶")]
        public bool IsTop { get; set; }
        /// <summary>
        /// 选项内容
        /// </summary>
        [Display(Name = "选项内容")]
        public string Content { get; set; }
        /// <summary>
        /// 访问次数
        /// </summary>
        [Display(Name = "访问次数")]
        public int? Click { get; set; }
        /// <summary>
        /// 开启状态
        /// </summary>
        [Display(Name = "开启状态")]
        public bool Status { get; set; }
        /// <summary>
        /// 投票数
        /// </summary>
        [Display(Name = "投票数")]
        public int TotalCount { get; set; }
        /// <summary>
        /// 投票主题
        /// </summary>
        [Display(Name = "投票主题")]
        public virtual VoteTheme VoteTheme { get; set; }
        /// <summary>
        /// 投票主题
        /// </summary>
        [Display(Name = "投票主题")]
        public string VoteThemeId { get; set; }
        public virtual ICollection<VoteItemRecord> VoteItemRecords { get; set; }
    }
}
