using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Business
{
   public class BusinessWriteOffDetail: BaseEntity
    {
        public BusinessWriteOffDetail()
        {
            SellMoney = 0;
            CostMoney = 0;
            Profit = 0;
            Percentage = 0;
            Commission = 0;
            MoneyBackDay = 0;
        }
        /// <summary>
        /// 出刊日期
        /// </summary>
        [Display(Name = "出刊日期")]
        public DateTime? PublishDate { get; set; }
        /// <summary>
        /// 销售金额（无税）
        /// </summary>
        [Display(Name = "销售金额（无税）")]
        public decimal SellMoney { get; set; }
        /// <summary>
        /// 成本金额（无税）
        /// </summary>
        [Display(Name = "成本金额（无税）")]
        public decimal CostMoney { get; set; }
        /// <summary>
        /// 利润金额
        /// </summary>
        [Display(Name = "利润金额")]
        public decimal Profit { get; set; }
        /// <summary>
        /// 提成系数
        /// </summary>
        [Display(Name = "提成系数")]
        public decimal Percentage { get; set; }
        /// <summary>
        /// 提成佣金
        /// </summary>
        [Display(Name = "提成佣金")]
        public decimal Commission { get; set; }
        /// <summary>
        /// 回款天数
        /// </summary>
        [Display(Name = "回款天数")]
        public int MoneyBackDay { get; set; }
        /// <summary>
        /// 销售订单
        /// </summary>
        [Display(Name = "销售订单")]
        public string BusinessOrderDetailId { get; set; }
        /// <summary>
        /// 销售订单
        /// </summary>
        [Display(Name = "销售项目")]
        public string BusinessOrderId { get; set; }
        /// <summary>
        /// 媒体类型
        /// </summary>
        [Display(Name = "媒体类型")]
        public string MediaTypeId { get; set; }
        /// <summary>
        /// 核销记录
        /// </summary>
        [Display(Name = "核销记录")]
        public string BusinessWriteOffId { get; set; }
        public virtual BusinessWriteOff BusinessWriteOff { get; set; }
    }
}
