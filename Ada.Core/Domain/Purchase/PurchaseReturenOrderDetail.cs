using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Purchase
{
    /// <summary>
    /// 采购退款明细
    /// </summary>
    public class PurchaseReturenOrderDetail : BaseEntity
    {
        /// <summary>
        /// 采购明细单
        /// </summary>
        [Display(Name = "采购明细单")]
        public string PurchaseOrderDetailId { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        [Display(Name = "退款金额")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 退款原因
        /// </summary>
        [Display(Name = "退款原因")]
        public string ReturnReason { get; set; }
        /// <summary>
        /// 退款类型
        /// </summary>
        [Display(Name = "退款类型")]
        public string ReturnType { get; set; }
        /// <summary>
        /// 退款日期
        /// </summary>
        [Display(Name = "退款日期")]
        public DateTime? ReturnDate { get; set; }
        /// <summary>
        /// 采购退款单
        /// </summary>
        [Display(Name = "采购退款单")]
        public string PurchaseReturnOrderId { get; set; }
        public virtual PurchaseReturnOrder PurchaseReturnOrder { get; set; }
    }
}
