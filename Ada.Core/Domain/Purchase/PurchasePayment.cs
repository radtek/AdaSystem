using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Customer;

namespace Ada.Core.Domain.Purchase
{
    /// <summary>
    /// 采购付款申请单
    /// </summary>
   public class PurchasePayment:BaseEntity
    {
        public PurchasePayment()
        {
            PurchasePaymentDetails = new HashSet<PurchasePaymentDetail>();
            PurchasePaymentOrderDetails=new HashSet<PurchasePaymentOrderDetail>();
        }
        /// <summary>
        /// 供应商
        /// </summary>
        [Display(Name = "供应商")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        [Display(Name = "供应商")]
        public string LinkManId { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [Display(Name = "申请日期")]
        public DateTime? BillDate { get; set; }
        /// <summary>
        /// 申请号
        /// </summary>
        [Display(Name = "申请号")]
        public string BillNum { get; set; }
        /// <summary>
        /// 付款状态
        /// </summary>
        [Display(Name = "付款状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 经办媒介
        /// </summary>
        [Display(Name = "经办媒介")]
        public string Transactor { get; set; }
        /// <summary>
        /// 经办媒介
        /// </summary>
        [Display(Name = "经办媒介")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        [Display(Name = "付款金额")]
        public decimal? PayMoney { get; set; }

        public virtual LinkMan LinkMan { get; set; }
        public virtual ICollection<PurchasePaymentDetail> PurchasePaymentDetails { get; set; }
        public virtual ICollection<PurchasePaymentOrderDetail> PurchasePaymentOrderDetails { get; set; }
    }
}
