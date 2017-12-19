using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Business
{
    /// <summary>
    /// 销售发票明细
    /// </summary>
   public class BusinessInvoiceDetail : BaseEntity
    {
        /// <summary>
        /// 销售订单
        /// </summary>
        [Display(Name = "销售订单")]
        public string BusinessOrderId { get; set; }
        /// <summary>
        /// 可开票金额
        /// </summary>
        [Display(Name = "可开票金额")]
        public decimal? OrderMoney { get; set; }
        /// <summary>
        /// 本次开票金额
        /// </summary>
        [Display(Name = "本次开票金额")]
        public decimal? InvoiceMoney { get; set; }
        /// <summary>
        /// 销售发票
        /// </summary>
        [Display(Name = "销售发票")]
        public string BusinessInvoiceId { get; set; }
        public virtual BusinessInvoice BusinessInvoice { get; set; }
        public virtual BusinessOrder BusinessOrder { get; set; }
    }
}
