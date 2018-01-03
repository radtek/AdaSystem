using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Finance
{
   public class BillPaymentView:BaseView
    {
        /// <summary>
        /// 付款类型
        /// </summary>
        [Display(Name = "付款类型")]
        public string PaymentType { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        [Display(Name = "单据号")]
        public string BillNum { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        [Display(Name = "单据日期")]
        public DateTime? BillDate { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        [Display(Name = "商户名称")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        [Display(Name = "商户名称")]
        public string LinkManId { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        [Display(Name = "开户行")]
        [Required]
        [StringLength(32, ErrorMessage = "字符长度不能超过32个")]
        public string AccountBank { get; set; }
        /// <summary>
        /// 开户名
        /// </summary>
        [Display(Name = "开户名")]
        [Required]
        [StringLength(32,ErrorMessage = "字符长度不能超过32个")]
        public string AccountName { get; set; }
        /// <summary>
        /// 开户号
        /// </summary>
        [Display(Name = "开户号")]
        [Required]
        [StringLength(32, ErrorMessage = "字符长度不能超过32个")]
        public string AccountNum { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        [Display(Name = "经办人")]
        public string Transactor { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        [Display(Name = "经办人")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 申请单类型 0 采购付款单 1 业务付款单
        /// </summary>
        [Display(Name = "申请单类型")]
        public short? RequestType { get; set; }
        /// <summary>
        /// 申请单号
        /// </summary>
        [Display(Name = "申请单号")]
        public string RequestNum { get; set; }
        /// <summary>
        /// 付款凭证
        /// </summary>
        [Display(Name = "付款凭证")]
        public string Image { get; set; }
        /// <summary>
        /// 付款凭证
        /// </summary>
        [Display(Name = "付款凭证")]
        public string ThumbnailImage { get; set; }
        /// <summary>
        /// 结算账户
        /// </summary>
        [Display(Name = "结算账户")]
        public string PayDetails { get; set; }
        /// <summary>
        /// 申请金额
        /// </summary>
        [Display(Name = "申请金额")]
        public decimal? PayMoney { get; set; }
        /// <summary>
        /// 是否开票
        /// </summary>
        [Display(Name = "是否开票")]
        public bool? IsInvoice { get; set; }
        /// <summary>
        /// 开票公司
        /// </summary>
        [Display(Name = "开票公司")]
        public string InvoiceTitle { get; set; }
        /// <summary>
        /// 付款摘要
        /// </summary>
        [Display(Name = "付款摘要")]
        public string PayInfo { get; set; }
    }
}
