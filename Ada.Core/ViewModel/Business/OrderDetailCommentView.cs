﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Business
{
   public class OrderDetailCommentView
    {
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
        /// 评价人
        /// </summary>
        [Display(Name = "评价人")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 评价日期
        /// </summary>
        [Display(Name = "评价日期")]
        public DateTime? CommentDate { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string BusinessOrderDetailId { get; set; }
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
    }

    public class CommentsView
    {
        public string OrderId { get; set; }
        public List<OrderDetailCommentView> OrderComments { get; set; }
    }
}
