using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;

namespace Ada.Core.Domain.Finance
{
    /// <summary>
    /// 销售收款单
    /// </summary>
   public class Receivables:BaseEntity
    {
        public Receivables()
        {
            BusinessPayees=new HashSet<BusinessPayee>();
        }
        /// <summary>
        /// 收款类型
        /// </summary>
        [Display(Name = "收款类型")]
        public string ReceivablesType { get; set; }
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
        /// 实收金额
        /// </summary>
        [Display(Name = "实收金额")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 待领金额
        /// </summary>
        [Display(Name = "待领金额")]
        public decimal? BalanceMoney { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        [Display(Name = "税额")]
        public decimal? TaxMoney { get; set; }
        /// <summary>
        /// 收支项目
        /// </summary>
        [Display(Name = "收支项目")]
        public string IncomeExpendId { get; set; }
        /// <summary>
        /// 收支项目
        /// </summary>
        [Display(Name = "收支项目")]
        public string IncomeExpendName { get; set; }
        /// <summary>
        /// 结算账户
        /// </summary>
        [Display(Name = "结算账户")]
        public string SettleAccountName { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        [Display(Name = "结算方式")]
        public string SettleType { get; set; }
        /// <summary>
        /// 结算账户
        /// </summary>
        [Display(Name = "结算账户")]
        public string SettleAccountId { get; set; }
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

        public virtual SettleAccount SettleAccount { get; set; }
        public virtual IncomeExpend IncomeExpend { get; set; }
        public virtual ICollection<BusinessPayee> BusinessPayees { get; set; }
    }
}
