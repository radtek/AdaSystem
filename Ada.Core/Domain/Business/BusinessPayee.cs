using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Customer;
using Ada.Core.Domain.Finance;

namespace Ada.Core.Domain.Business
{
    /// <summary>
    /// 业务领款单
    /// </summary>
    public class BusinessPayee : BaseEntity
    {
        public BusinessPayee()
        {
            BusinessPayments=new HashSet<BusinessPayment>();
            BusinessWriteOffs=new HashSet<BusinessWriteOff>();
        }
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
        /// 领款金额
        /// </summary>
        [Display(Name = "领款金额")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 领款日期
        /// </summary>
        [Display(Name = "领款日期")]
        public DateTime? ClaimDate { get; set; }
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
        /// 收款单
        /// </summary>
        [Display(Name = "收款单")]
        public string ReceivablesId { get; set; }
        public virtual Receivables Receivables { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [Display(Name = "客户名称")]
        public string LinkManId { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [Display(Name = "客户名称")]
        public string LinkManName { get; set; }
        public virtual LinkMan LinkMan { get; set; }
        public virtual ICollection<BusinessPayment> BusinessPayments { get; set; }
        public virtual ICollection<BusinessWriteOff> BusinessWriteOffs { get; set; }
    }
}
