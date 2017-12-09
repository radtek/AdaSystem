using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Purchase
{
  public  class PurchasePaymentView:BaseView
    {
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
        /// <summary>
        /// 订单明细
        /// </summary>
        [Display(Name = "订单明细")]
        public string OrderDetails { get; set; }
        /// <summary>
        /// 付款明细
        /// </summary>
        [Display(Name = "付款明细")]
        public string PayDetails { get; set; }
    }
}
