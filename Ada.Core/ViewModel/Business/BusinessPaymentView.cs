using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Business
{
    /// <summary>
    /// 请款单
    /// </summary>
    public class BusinessPaymentView : BaseView
    {
        /// <summary>
        /// 开户行
        /// </summary>
        [Display(Name = "开户行")]
        public string AccountBank { get; set; }
        /// <summary>
        /// 开户名
        /// </summary>
        [Display(Name = "开户名")]
        public string AccountName { get; set; }
        /// <summary>
        /// 开户号
        /// </summary>
        [Display(Name = "开户号")]
        public string AccountNum { get; set; }
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
        /// 申请时间
        /// </summary>
        [Display(Name = "申请时间")]
        public DateTime? ApplicationDate { get; set; }
        /// <summary>
        /// 申请金额
        /// </summary>
        [Display(Name = "申请金额")]
        public decimal? PayMoney { get; set; }
        /// <summary>
        /// 申请号
        /// </summary>
        [Display(Name = "申请号")]
        public string ApplicationNum { get; set; }
        /// <summary>
        /// 付款性质
        /// </summary>
        [Display(Name = "付款性质")]
        public string PaymentType { get; set; }
        /// <summary>
        /// 付款状态
        /// </summary>
        [Display(Name = "付款状态")]
        public short? Status { get; set; }
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
        /// 申请凭证
        /// </summary>
        [Display(Name = "申请凭证")]
        public string Image { get; set; }
        /// <summary>
        /// 领款单
        /// </summary>
        [Display(Name = "领款单")]
        public string BusinessPayeeId { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [Display(Name = "客户名称")]
        public string LinkmanName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}
