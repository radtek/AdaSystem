using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Vote
{
    public class VoteTheme : BaseEntity
    {
        public VoteTheme()
        {
            StartDate=DateTime.Now;
            EndDate=DateTime.Now.AddDays(7);
            Status = false;
            VoteItems=new HashSet<VoteItem>();
        }
        /// <summary>
        /// 主题
        /// </summary>
        [Display(Name = "主题")]
        public string Title { get; set; }
        /// <summary>
        /// 调用别名
        /// </summary>
        [Display(Name = "调用别名")]
        public string CallIndex { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        [Display(Name = "开始日期")]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        [Display(Name = "结束日期")]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        [Display(Name = "关键字")]
        public string KeyWord { get; set; }
        /// <summary>
        /// 开始封面
        /// </summary>
        [Display(Name = "开始封面")]
        public string CoverStart { get; set; }
        /// <summary>
        /// 结束封面
        /// </summary>
        [Display(Name = "结束封面")]
        public string CoverEnd { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        [Display(Name = "摘要")]
        public string Abstract { get; set; }
        /// <summary>
        /// 规则
        /// </summary>
        [Display(Name = "规则")]
        public string Rule { get; set; }
        /// <summary>
        /// 主题内容
        /// </summary>
        [Display(Name = "主题内容")]
        public string Content { get; set; }
        /// <summary>
        /// 结束主题
        /// </summary>
        [Display(Name = "结束主题")]
        public string EndTitle { get; set; }
        /// <summary>
        /// 结束主题内容
        /// </summary>
        [Display(Name = "结束主题内容")]
        public string EndContent { get; set; }
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
        /// 投票参数
        /// </summary>
        [Display(Name = "投票参数")]
        public string Config { get; set; }
        public virtual ICollection<VoteItem> VoteItems { get; set; }
    }
}
