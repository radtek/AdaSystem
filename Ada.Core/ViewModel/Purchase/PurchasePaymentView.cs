using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Purchase
{
    public class PurchasePaymentView : BaseView
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
        /// 申请日期
        /// </summary>
        [Display(Name = "申请日期")]
        public DateTime? BillDateStart { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [Display(Name = "申请日期")]
        public DateTime? BillDateEnd { get; set; }
        /// <summary>
        /// 申请号
        /// </summary>
        [Display(Name = "申请号")]
        public string BillNum { get; set; }
        /// <summary>
        /// 付款状态
        /// </summary>
        [Display(Name = "付款状态")]
        public string Status { get; set; }
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
        /// 订单总额
        /// </summary>
        [Display(Name = "订单总额")]
        public decimal? PayMoney { get; set; }
        /// <summary>
        /// 已申请金额
        /// </summary>
        [Display(Name = "已申请金额")]
        public decimal? RequstMoney { get; set; }
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
        /// <summary>
        /// 是否显示 根据审核状态来判断
        /// </summary>
        [Display(Name = "是否显示")]
        public bool? IsDisable { get; set; }
        /// <summary>
        /// 税率%
        /// </summary>
        [Display(Name = "税率%")]
        public decimal? Tax { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        [Display(Name = "税额")]
        public decimal? TaxMoney { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        [Display(Name = "优惠金额")]
        public decimal? DiscountMoney { get; set; }
        /// <summary>
        /// 发票状态
        /// </summary>
        [Display(Name = "发票状态")]
        public bool? InvoiceStauts { get; set; }
        /// <summary>
        /// 是否开票
        /// </summary>
        [Display(Name = "是否开票")]
        public bool? IsInvoice { get; set; }
        /// <summary>
        /// 到票日期
        /// </summary>
        [Display(Name = "到票日期")]
        public DateTime? InvoiceDate { get; set; }
        /// <summary>
        /// 到票日期
        /// </summary>
        [Display(Name = "到票日期")]
        public DateTime? InvoiceDateStart { get; set; }
        /// <summary>
        /// 到票日期
        /// </summary>
        [Display(Name = "到票日期")]
        public DateTime? InvoiceDateEnd { get; set; }
        /// <summary>
        /// 发票号
        /// </summary>
        [Display(Name = "发票号")]
        public string InvoiceNum { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>
        [Display(Name = "发票抬头")]
        public string InvoiceTitle { get; set; }
        /// <summary>
        /// 开票公司
        /// </summary>
        [Display(Name = "开票公司")]
        public string InvoiceCompany { get; set; }
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
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaName { get; set; }
        /// <summary>
        /// 总税额
        /// </summary>
        [Display(Name = "总税额")]
        public decimal? TotalTaxMoney { get; set; }
        /// <summary>
        /// 总申请金额
        /// </summary>
        [Display(Name = "总申请金额")]
        public decimal? TotalRequestMoney { get; set; }
        /// <summary>
        /// 开户名
        /// </summary>
        [Display(Name = "开户名")]
        public string BankAccount { get; set; }
    }
}
