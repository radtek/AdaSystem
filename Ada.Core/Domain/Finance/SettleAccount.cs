using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Finance
{
    /// <summary>
    /// 结算账户
    /// </summary>
    public class SettleAccount : BaseEntity
    {
        public SettleAccount()
        {
            BillPaymentDetails=new HashSet<BillPaymentDetail>();
            Receivableses=new HashSet<Receivables>();
            ExpenseDetails=new HashSet<ExpenseDetail>();
        }
        /// <summary>
        /// 结算账户
        /// </summary>
        [Display(Name = "结算账户")]
        public string SettleName { get; set; }
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
        /// 初始金额
        /// </summary>
        [Display(Name = "初始金额")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 税率%
        /// </summary>
        [Display(Name = "税率%")]
        public decimal? Tax { get; set; }
        public virtual ICollection<BillPaymentDetail> BillPaymentDetails { get; set; }
        public virtual ICollection<Receivables> Receivableses { get; set; }
        public virtual ICollection<ExpenseDetail> ExpenseDetails { get; set; }
    }
}
