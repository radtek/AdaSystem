using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Finance
{
    /// <summary>
    /// 收支
    /// </summary>
    public class ReceiptExpenditureView:BaseView
    {
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
        /// 单据编号
        /// </summary>
        [Display(Name = "单据编号")]
        public string BillNum { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        [Display(Name = "单据日期")]
        public DateTime? BillDate { get; set; }
        /// <summary>
        /// 收入金额
        /// </summary>
        [Display(Name = "收入金额")]
        public decimal? ReceiptMoney { get; set; }
        /// <summary>
        /// 支出金额
        /// </summary>
        [Display(Name = "支出金额")]
        public decimal? ExpenditureMoney { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        [Display(Name = "单据日期")]
        public DateTime? BillDateStart { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        [Display(Name = "单据日期")]
        public DateTime? BillDateEnd { get; set; }
        /// <summary>
        /// 收入总额
        /// </summary>
        [Display(Name = "收入总额")]
        public decimal? TotalReceiptMoney { get; set; }
        /// <summary>
        /// 支出总额
        /// </summary>
        [Display(Name = "支出总额")]
        public decimal? TotalExpenditureMoney { get; set; }
        /// <summary>
        /// 科目类型 1 收入 0 支出
        /// </summary>
        [Display(Name = "科目类型")]
        public short? SubjectType { get; set; }
        /// <summary>
        /// 业务员
        /// </summary>
        [Display(Name = "业务员")]
        public string Employe { get; set; }
    }
}
