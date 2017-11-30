﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Purchase
{
    /// <summary>
    /// 采购付款申请清单
    /// </summary>
   public class PurchasePaymentDetail:BaseEntity
    {
        /// <summary>
        /// 采购订单
        /// </summary>
        [Display(Name = "采购订单")]
        public string PurchaseOrderDetailId { get; set; }
        /// <summary>
        /// 付款申请单
        /// </summary>
        [Display(Name = "付款申请单")]
        public string PurchasePaymentId { get; set; }
        public virtual PurchasePayment PurchasePayment { get; set; }
        public virtual PurchaseOrderDetail PurchaseOrderDetail { get; set; }
    }
}
