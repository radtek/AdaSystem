using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Vote
{
   public class VoteItemRecordView:BaseView
    {
        public VoteItemRecordView()
        {
            Date = DateTime.Now;
            Score = 0;
        }
        /// <summary>
        /// 投票日期
        /// </summary>
        [Display(Name = "投票日期")]
        public DateTime Date { get; set; }
        /// <summary>
        /// 投票人
        /// </summary>
        [Display(Name = "投票人")]
        public string Name { get; set; }
        /// <summary>
        /// 投票人Id
        /// </summary>
        [Display(Name = "投票人Id")]
        public string UID { get; set; }
        /// <summary>
        /// 联系手机
        /// </summary>
        [Display(Name = "联系手机")]
        public string Phone { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [Display(Name = "头像")]
        public string Image { get; set; }
        /// <summary>
        /// 微信ID
        /// </summary>
        [Display(Name = "微信ID")]
        public string OpenId { get; set; }
        /// <summary>
        /// Cookie
        /// </summary>
        [Display(Name = "Cookie")]
        public string Cookies { get; set; }
        /// <summary>
        /// 分数
        /// </summary>
        [Display(Name = "分数")]
        public int Score { get; set; }
    }
}
