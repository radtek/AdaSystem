using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Vote
{
   public class VoteConfig
    {
        public VoteConfig()
        {
            VoteTimes = 1;
            Weixin = false;
            Subscribe = false;
            IpRestrict = false;
        }
        /// <summary>
        /// 投票次数
        /// </summary>
        [Display(Name = "投票次数")]
        public short VoteTimes { get; set; }
        /// <summary>
        /// 是否只限微信
        /// </summary>
        [Display(Name = "是否只限微信")]
        public bool Weixin { get; set; }
        /// <summary>
        /// 是否关注公众号
        /// </summary>
        [Display(Name = "是否关注公众号")]
        public bool Subscribe { get; set; }
        /// <summary>
        /// 是否IP限制
        /// </summary>
        [Display(Name = "是否IP限制")]
        public bool IpRestrict { get; set; }
    }
}
