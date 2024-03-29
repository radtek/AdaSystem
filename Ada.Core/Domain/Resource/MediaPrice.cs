﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;

namespace Ada.Core.Domain.Resource
{
   public class MediaPrice:BaseEntity
    {
        public MediaPrice()
        {
            PurchaseOrderDetails=new HashSet<PurchaseOrderDetail>();
            BusinessOrderDetails = new HashSet<BusinessOrderDetail>();
            BusinessOfferDetails=new HashSet<BusinessOfferDetail>();
            MediaPriceChanges=new HashSet<MediaPriceChange>();
        }
        /// <summary>
        /// 广告位
        /// </summary>
        [Display(Name = "广告位")]
        public string AdPositionId { get; set; }
        /// <summary>
        /// 广告位
        /// </summary>
        [Display(Name = "广告位")]
        public string AdPositionName { get; set; }
        /// <summary>
        /// 采购价格
        /// </summary>
        [Display(Name = "采购价格")]
        public decimal? PurchasePrice { get; set; }
        /// <summary>
        /// 销售价格
        /// </summary>
        [Display(Name = "销售价格")]
        public decimal? MarketPrice { get; set; }
        /// <summary>
        /// 零售价格
        /// </summary>
        [Display(Name = "零售价格")]
        public decimal? SellPrice { get; set; }
        /// <summary>
        /// 价格日期
        /// </summary>
        [Display(Name = "价格日期")]
        public DateTime? PriceDate { get; set; }
        /// <summary>
        /// 价格失效日期
        /// </summary>
        [Display(Name = "价格失效日期")]
        public DateTime? InvalidDate { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaId { get; set; }
        public virtual Media Media { get; set; }
        public virtual ICollection<BusinessOrderDetail> BusinessOrderDetails { get; set; }
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual ICollection<BusinessOfferDetail> BusinessOfferDetails { get; set; }
        public virtual ICollection<MediaPriceChange> MediaPriceChanges { get; set; }
    }
}
