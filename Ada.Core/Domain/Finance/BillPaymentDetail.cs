using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Finance
{
    /// <summary>
    /// 业务付款明细
    /// </summary>
  public  class BillPaymentDetail:BaseEntity
    {
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
        /// 结算号
        /// </summary>
        [Display(Name = "结算号")]
        public string SettleNum { get; set; }
        /// <summary>
        /// 付款单
        /// </summary>
        [Display(Name = "付款单")]
        public string BillPaymentId { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        [Display(Name = "金额")]
        public decimal? Money { get; set; }
        public virtual SettleAccount SettleAccount { get; set; }
        public virtual IncomeExpend IncomeExpend { get; set; }
        public virtual BillPayment BillPayment { get; set; }
        
    }
}
