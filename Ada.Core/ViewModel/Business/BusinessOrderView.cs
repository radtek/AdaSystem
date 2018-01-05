using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Business
{
    public class BusinessOrderView : BaseView
    {
        /// <summary>
        /// 销售类型
        /// </summary>
        [Display(Name = "销售类型")]
        public string BusinessType { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        [Display(Name = "订单编号")]
        public string OrderNum { get; set; }
        /// <summary>
        /// 销售金额
        /// </summary>
        [Display(Name = "销售金额")]
        public decimal? TotalMoney { get; set; }
       
        /// <summary>
        /// 无税金额
        /// </summary>
        [Display(Name = "无税金额")]
        public decimal? TotalSellMoney { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        [Display(Name = "税额")]
        public decimal? TotalTaxMoney { get; set; }
        /// <summary>
        /// 税率%
        /// </summary>
        [Display(Name = "税率%")]
        public decimal? Tax { get; set; }
        /// <summary>
        /// 折扣%
        /// </summary>
        [Display(Name = "折扣%")]
        public decimal? DiscountRate { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        [Display(Name = "优惠金额")]
        public decimal? DiscountMoney { get; set; }
        /// <summary>
        /// 未核销金额
        /// </summary>
        [Display(Name = "未核销金额")]
        public decimal? VerificationMoney { get; set; }
        /// <summary>
        /// 已核销金额
        /// </summary>
        [Display(Name = "已核销金额")]
        public decimal? ConfirmVerificationMoney { get; set; }
        /// <summary>
        /// 核销状态
        /// </summary>
        [Display(Name = "核销状态")]
        public short? VerificationStatus { get; set; }
        /// <summary>
        /// 经办业务
        /// </summary>
        [Display(Name = "经办业务")]
        [Required]
        public string Transactor { get; set; }
        /// <summary>
        /// 经办业务
        /// </summary>
        [Display(Name = "经办业务")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [Display(Name = "订单状态")]
        public short? Status { get; set; }
        
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
        /// 客户名称
        /// </summary>
        [Display(Name = "客户名称")]
        [Required]
        public string LinkManName { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        [Display(Name = "结算方式")]
        public string SettlementType { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [Display(Name = "客户名称")]
        public string LinkManId { get; set; }
        /// <summary>
        /// 创建人员
        /// </summary>
        [Display(Name = "创建人员")]
        public string AdderBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public string AdderDate { get; set; }
        /// <summary>
        /// 单据时间
        /// </summary>
        [Display(Name = "单据时间")]
        [Required]
        public DateTime? OrderDate { get; set; }
        /// <summary>
        /// 项目摘要
        /// </summary>
        [Display(Name = "项目摘要")]
        public string Remark { get; set; }
        /// <summary>
        /// 订单明细
        /// </summary>
        [Display(Name = "订单明细")]
        [Required]
        public string OrderDetails { get; set; }
        /// <summary>
        /// 采购进度
        /// </summary>
        [Display(Name = "采购进度")]
        public string PurchaseSchedule { get; set; }
        /// <summary>
        /// 订单明细数IsInvoice
        /// </summary>
        [Display(Name = "订单明细数")]
        public int OrderDetailCount { get; set; }
        /// <summary>
        /// 是否开票
        /// </summary>
        [Display(Name = "是否开票")]
        public bool? IsInvoice { get; set; }
        /// <summary>
        /// 转单情况
        /// </summary>
        [Display(Name = "转单情况")]
        public string OrderSchedule { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaName { get; set; }
    }
}
