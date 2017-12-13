using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Finance
{
    /// <summary>
    /// 收支项目
    /// </summary>
    public class IncomeExpend : BaseEntity
    {
        public IncomeExpend()
        {
            BillPaymentDetails=new HashSet<BillPaymentDetail>();
            Receivableses=new HashSet<Receivables>();
            ExpenseDetails=new HashSet<ExpenseDetail>();
        }
        /// <summary>
        /// 科目名称
        /// </summary>
        [Display(Name = "科目名称")]
        public string SubjectName { get; set; }
        /// <summary>
        /// 科目编号
        /// </summary>
        [Display(Name = "科目编号")]
        public string SubjectNum { get; set; }
        /// <summary>
        /// 科目类型  1 收入  0 支出
        /// </summary>
        [Display(Name = "科目类型")]
        public short? SubjectType { get; set; }
        /// <summary>
        /// 科目类型
        /// </summary>
        [Display(Name = "是否主营项目")]
        public bool? IsMain { get; set; }
        /// <summary>
        /// 父级类别
        /// </summary>
        [Display(Name = "上级科目")]
        public string ParentId { get; set; }

        public virtual ICollection<BillPaymentDetail> BillPaymentDetails { get; set; }
        public virtual ICollection<Receivables> Receivableses { get; set; }
        public virtual ICollection<ExpenseDetail> ExpenseDetails { get; set; }
    }
}
