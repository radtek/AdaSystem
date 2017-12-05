using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Business
{
   public class BusinessPayeeView:BaseView
    {
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
        /// 可领金额
        /// </summary>
        [Display(Name = "可领金额")]
        public decimal? TotalMoney { get; set; }
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
        /// <summary>
        /// 请款记录数
        /// </summary>
        [Display(Name = "请款记录数")]
        public int PaymentCount { get; set; }
    }
}
