using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Resource
{
   public class MediaCommentView:BaseView
    {

        /// <summary>
        /// 平均分
        /// </summary>
        [Display(Name = "平均分")]
        public double? AvgScore { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        [Display(Name = "评分")]
        public short? Score { get; set; }
        /// <summary>
        /// 评价内容
        /// </summary>
        [Display(Name = "评价内容")]
        public string Content { get; set; }
        /// <summary>
        /// 评价人
        /// </summary>
        [Display(Name = "评价人")]
        public string Transactor { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [Display(Name = "头像")]
        public string TransactorImage { get; set; }
        /// <summary>
        /// 所属机构
        /// </summary>
        [Display(Name = "所属机构")]
        public string Organization { get; set; }
        /// <summary>
        /// 媒体资源
        /// </summary>
        [Display(Name = "媒体资源")]
        public string MediaId { get; set; }
        /// <summary>
        /// 媒体类型
        /// </summary>
        [Display(Name = "媒体类型")]
        public string MediaTypeId { get; set; }
        /// <summary>
        /// 媒体资源
        /// </summary>
        [Display(Name = "媒体资源")]
        public string MediaName { get; set; }
        /// <summary>
        /// 评价日期
        /// </summary>
        [Display(Name = "评价日期")]
        public DateTime? CommentDate { get; set; }
        /// <summary>
        /// 订单信息
        /// </summary>
        [Display(Name = "订单信息")]
        public string OrderRemark { get; set; }
    }
}
