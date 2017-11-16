using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Customer;

namespace Ada.Core.Domain.Business
{
    /// <summary>
    /// 销售订单
    /// </summary>
    public class BusinessOrder : BaseEntity
    {
        public BusinessOrder()
        {
            BusinessOrderDetails = new HashSet<BusinessOrderDetail>();
        }
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
        public decimal? TotalDiscountMoney { get; set; }
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
        /// 联系客户
        /// </summary>
        [Display(Name = "联系客户")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        [Display(Name = "结算方式")]
        public string SettlementType { get; set; }
        /// <summary>
        /// 单据时间
        /// </summary>
        [Display(Name = "单据时间")]
        public DateTime? OrderDate { get; set; }
        /// <summary>
        /// 联系客户
        /// </summary>
        [Display(Name = "联系客户")]
        public string LinkManId { get; set; }
        public virtual LinkMan LinkMan { get; set; }
        public virtual ICollection<BusinessOrderDetail> BusinessOrderDetails { get; set; }
    }
}
