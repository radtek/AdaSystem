using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Business
{
    /// <summary>
    /// 报价
    /// </summary>
   public class BusinessOfferView:BaseView
    {
        /// <summary>
        /// 报价单号
        /// </summary>
        [Display(Name = "报价单号")]
        public string OfferNum { get; set; }
        /// <summary>
        /// 报价金额
        /// </summary>
        [Display(Name = "报价金额")]
        public decimal? TotalMoney { get; set; }
        /// <summary>
        /// 税率%
        /// </summary>
        [Display(Name = "税率%")]
        public decimal? Tax { get; set; }
        /// <summary>
        /// 折扣%
        /// </summary>
        [Display(Name = "折扣%")]
        public decimal? DiscountRate { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        [Display(Name = "优惠金额")]
        public decimal? DiscountMoney { get; set; }
        /// <summary>
        /// 无税金额
        /// </summary>
        [Display(Name = "无税金额")]
        public decimal? TotalSellMoney { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        [Display(Name = "税额")]
        public decimal? TotalTaxMoney { get; set; }

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
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 有效天数
        /// </summary>
        [Display(Name = "有效天数")]
        public short? ValidDays { get; set; }
        /// <summary>
        /// 报价日期
        /// </summary>
        [Display(Name = "报价日期")]
        public DateTime? OfferDate { get; set; }
        /// <summary>
        /// 联系客户
        /// </summary>
        [Display(Name = "联系客户")]
        public string LinkManId { get; set; }
        /// <summary>
        /// 联系客户
        /// </summary>
        [Display(Name = "联系客户")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 报价摘要
        /// </summary>
        [Display(Name = "报价摘要")]
        public string Remark { get; set; }
        /// <summary>
        /// 资源明细
        /// </summary>
        [Display(Name = "资源明细")]
        public string Details { get; set; }
        /// <summary>
        /// 报价方式
        /// </summary>
        [Display(Name = "报价方式")]
        public short? OfferType { get; set; }
    }
}
