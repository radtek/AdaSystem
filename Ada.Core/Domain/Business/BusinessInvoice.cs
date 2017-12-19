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
    /// 销售发票
    /// </summary>
   public class BusinessInvoice : BaseEntity
    {
        public BusinessInvoice()
        {
            BusinessInvoiceDetails = new HashSet<BusinessInvoiceDetail>();
        }
        /// <summary>
        /// 发票抬头
        /// </summary>
        [Display(Name = "发票抬头")]
        public string InvoiceTitle { get; set; }
        /// <summary>
        /// 发票类型
        /// </summary>
        [Display(Name = "发票类型")]
        public string InvoiceType { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [Display(Name = "公司名称")]
        public string Company { get; set; }
        /// <summary>
        /// 纳税识别号
        /// </summary>
        [Display(Name = "纳税识别号")]
        public string TaxNum { get; set; }
        /// <summary>
        /// 注册地址
        /// </summary>
        [Display(Name = "注册地址")]
        public string Address { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        [Display(Name = "开户行")]
        public string Bank { get; set; }
        /// <summary>
        /// 开户账号
        /// </summary>
        [Display(Name = "开户账号")]
        public string BankNum { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [Display(Name = "联系电话")]
        public string Phone { get; set; }
        /// <summary>
        /// 发票状态
        /// </summary>
        [Display(Name = "发票状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 到款状态
        /// </summary>
        [Display(Name = "到款状态")]
        public short? MoneyStatus { get; set; }
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
        /// 付款金额
        /// </summary>
        [Display(Name = "发票金额")]
        public decimal? TotalMoney { get; set; }

        public virtual ICollection<BusinessInvoiceDetail> BusinessInvoiceDetails { get; set; }
    }
}
