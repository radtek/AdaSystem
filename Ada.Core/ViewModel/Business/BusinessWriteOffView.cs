using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Business
{
    public class BusinessWriteOffView : BaseView
    {
        /// <summary>
        /// 销账日期
        /// </summary>
        [Display(Name = "销账日期")]
        public DateTime? WriteOffDate { get; set; }
        /// <summary>
        /// 销账金额
        /// </summary>
        [Display(Name = "销账金额")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 经办业务
        /// </summary>
        [Display(Name = "经办业务")]
        [Required]
        public string Transactor { get; set; }
        /// <summary>
        /// 经办业务
        /// </summary>
        [Display(Name = "经办业务")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 销售领款
        /// </summary>
        [Display(Name = "销售领款")]
        [Required]
        public string Payees { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        [Display(Name = "订单编号")]
        public string OrderNum { get; set; }
        /// <summary>
        /// 销售订单
        /// </summary>
        [Display(Name = "销售订单")]
        [Required]
        public string Orders { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [Display(Name = "客户名称")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [Display(Name = "客户名称")]
        public string LinkManId { get; set; }
        /// <summary>
        /// 领款核销总额
        /// </summary>
        [Display(Name = "领款核销总额")]
        public decimal? PayeeMoney { get; set; }
        /// <summary>
        /// 订单核销总额
        /// </summary>
        [Display(Name = "订单核销总额")]
        public decimal? OrderMoney { get; set; }
    }
}
