using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Finance
{
   public class ExpenseView:BaseView
    {
        /// <summary>
        /// 收支类型
        /// </summary>
        [Display(Name = "收支类型")]
        public bool IsIncom { get; set; }
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
        /// <summary>
        /// 收支单位
        /// </summary>
        [Display(Name = "收支单位")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 收支单位
        /// </summary>
        [Display(Name = "收支单位")]
        public string LinkManId { get; set; }
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
        /// 业务员
        /// </summary>
        [Display(Name = "业务员")]
        public string Employe { get; set; }
        /// <summary>
        /// 业务员
        /// </summary>
        [Display(Name = "业务员")]
        public string EmployerId { get; set; }
        /// <summary>
        /// 申请单类型 0 采购付款单 1 业务付款单
        /// </summary>
        [Display(Name = "单据类型")]
        public short? RequestType { get; set; }
        /// <summary>
        /// 申请单号
        /// </summary>
        [Display(Name = "费用单号")]
        public string RequestNum { get; set; }
        /// <summary>
        /// 费用凭证
        /// </summary>
        [Display(Name = "费用凭证")]
        public string Image { get; set; }
        /// <summary>
        /// 费用凭证
        /// </summary>
        [Display(Name = "费用凭证")]
        public string ThumbnailImage { get; set; }
        /// <summary>
        /// 收支金额
        /// </summary>
        [Display(Name = "收支金额")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 结算明细
        /// </summary>
        [Display(Name = "结算明细")]
        public string PayDetails { get; set; }
        /// <summary>
        /// 费用项目
        /// </summary>
        [Display(Name = "费用项目")]
        public string IncomeExpends { get; set; }
    }
}
