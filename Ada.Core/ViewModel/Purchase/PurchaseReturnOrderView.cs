using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Purchase
{
   public class PurchaseReturnOrderView: BaseView
    {
        /// <summary>
        /// 退款状态
        /// </summary>
        [Display(Name = "退款状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 采购订单
        /// </summary>
        [Display(Name = "采购订单")]
        public string PurchaseOrderId { get; set; }
        /// <summary>
        /// 退款编号
        /// </summary>
        [Display(Name = "退款编号")]
        public string ReturnOrderNum { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        [Display(Name = "退款金额")]
        public decimal? TotalMoney { get; set; }
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
        /// 审核人
        /// </summary>
        [Display(Name = "审核人")]
        public string AuditBy { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        [Display(Name = "审核人")]
        public string AuditById { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        [Display(Name = "审核时间")]
        public DateTime? AuditDate { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        [Display(Name = "审核状态")]
        public short? AuditStatus { get; set; }
        /// <summary>
        /// 作废人
        /// </summary>
        [Display(Name = "作废人")]
        public string CancelBy { get; set; }
        /// <summary>
        /// 作废人
        /// </summary>
        [Display(Name = "作废人")]
        public string CancelById { get; set; }
        /// <summary>
        /// 作废时间
        /// </summary>
        [Display(Name = "作废时间")]
        public DateTime? CancelDate { get; set; }
        /// <summary>
        /// 单据时间
        /// </summary>
        [Display(Name = "单据时间")]
        public DateTime? ReturnDate { get; set; }
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
    }
}
