﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Business
{
   public class BusinessTotal
    {
        
        /// <summary>
        /// 待转单
        /// </summary>
        [Display(Name = "待转单")]
        public int? Waiting { get; set; }
        /// <summary>
        /// 正处理
        /// </summary>
        [Display(Name = "正处理")]
        public int? Doing { get; set; }
        /// <summary>
        /// 已确认
        /// </summary>
        [Display(Name = "已确认")]
        public int? Confirm { get; set; }
        /// <summary>
        /// 已完成
        /// </summary>
        [Display(Name = "已完成")]
        public int? Done { get; set; }
        /// <summary>
        /// 今日预出刊
        /// </summary>
        [Display(Name = "今日预出刊")]
        public int? Today { get; set; }
        /// <summary>
        /// 明日预出刊
        /// </summary>
        [Display(Name = "明日预出刊")]
        public int? Tomorrow { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? Sort { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        [Display(Name = "月份")]
        public string Month { get; set; }
        /// <summary>
        /// 订单数据
        /// </summary>
        [Display(Name = "订单数据")]
        public BusinessOrderDetailView BusinessOrderDetailView { get; set; }

    }
}
