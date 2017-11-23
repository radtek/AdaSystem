using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ada.Core.Domain.Customer;

namespace Ada.Core.Domain.Purchase
{
  public  class PurchaseOrder:BaseEntity
    {
        /// <summary>
        /// 采购订单
        /// </summary>
        public PurchaseOrder()
        {
            PurchaseOrderDetails=new HashSet<PurchaseOrderDetail>();
        }
        /// <summary>
        /// 销售订单
        /// </summary>
        [Display(Name = "销售订单")]
        public string BusinessOrderId { get; set; }
        /// <summary>
        /// 经办业务
        /// </summary>
        [Display(Name = "经办业务")]
        public string BusinessBy { get; set; }
        /// <summary>
        /// 经办业务
        /// </summary>
        [Display(Name = "经办业务")]
        public string BusinessById { get; set; }
        /// <summary>
        /// 采购编号
        /// </summary>
        [Display(Name = "采购编号")]
        public string OrderNum { get; set; }
        /// <summary>
        /// 采购金额
        /// </summary>
        [Display(Name = "采购金额")]
        public decimal? TotalMoney { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        [Display(Name = "优惠金额")]
        public decimal? TotalDiscountMoney { get; set; }
        /// <summary>
        /// 预付定金
        /// </summary>
        [Display(Name = "预付定金")]
        public decimal? TotalBargainMoney { get; set; }
        /// <summary>
        /// 无税金额
        /// </summary>
        [Display(Name = "无税金额")]
        public decimal? TotalPurchaseMoney { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        [Display(Name = "税额")]
        public decimal? TotalTaxMoney { get; set; }
       
        /// <summary>
        /// 单据时间
        /// </summary>
        [Display(Name = "单据时间")]
        public DateTime? OrderDate { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [Display(Name = "订单状态")]
        public short? Status { get; set; }

        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
    }
}
