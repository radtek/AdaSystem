using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Business
{
    public class BusinessWriteOffDetailView : BaseView
    {
        /// <summary>
        /// 销账日期
        /// </summary>
        [Display(Name = "销账日期")]
        public DateTime? WriteOffDate { get; set; }
        /// <summary>
        /// 出刊日期
        /// </summary>
        [Display(Name = "出刊日期")]
        public DateTime? PublishDate { get; set; }
        /// <summary>
        /// 销账金额
        /// </summary>
        [Display(Name = "销账金额")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 销售金额
        /// </summary>
        [Display(Name = "销售金额")]
        public decimal? BusinessMoney { get; set; }
        /// <summary>
        /// 利润金额
        /// </summary>
        [Display(Name = "利润金额")]
        public decimal? Profit { get; set; }
        /// <summary>
        /// 销售提成
        /// </summary>
        [Display(Name = "销售提成")]
        public decimal? Commission { get; set; }
        /// <summary>
        /// 回款天数
        /// </summary>
        [Display(Name = "回款天数")]
        public int? ReturnDays { get; set; }
        /// <summary>
        /// 采购成本
        /// </summary>
        [Display(Name = "采购成本")]
        public decimal? PurchaseMoney { get; set; }
        /// <summary>
        /// 领款金额
        /// </summary>
        [Display(Name = "领款金额")]
        public decimal? PayeeMoney { get; set; }
        /// <summary>
        /// 税率%
        /// </summary>
        [Display(Name = "税率%")]
        public decimal? Tax { get; set; }
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
        /// 订单编号
        /// </summary>
        [Display(Name = "订单编号")]
        [Required]
        public string OrderId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        [Display(Name = "订单编号")]
        [Required]
        public string OrderNum { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        [Display(Name = "客户名称")]
        public string LinkManName { get; set; }

        /// <summary>
        /// 领款核销总额
        /// </summary>
        [Display(Name = "领款核销总额")]
        public decimal? TotalPayeeMoney { get; set; }
        /// <summary>
        /// 销售总额
        /// </summary>
        [Display(Name = "销售总额")]
        public decimal? TotalBusinessMoney { get; set; }
        /// <summary>
        /// 总利润
        /// </summary>
        [Display(Name = "总利润")]
        public decimal? TotalProfit { get; set; }
        /// <summary>
        /// 总成本
        /// </summary>
        [Display(Name = "总成本")]
        public decimal? TotalPurchaseMoney { get; set; }

        /// <summary>
        /// 总提成
        /// </summary>
        [Display(Name = "总提成")]
        public decimal? TotalCommission { get; set; }
    }
}
