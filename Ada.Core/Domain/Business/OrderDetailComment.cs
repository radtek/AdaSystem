﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Business
{
   public class OrderDetailComment : BaseEntity
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
        public virtual BusinessOrderDetail BusinessOrderDetail { get; set; }
    }
}
