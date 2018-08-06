using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ada.Core.ViewModel.Business;

namespace Ada.Core.ViewModel.Statistics
{
   public class BusinessTotal
    {
        public BusinessTotal()
        {
            BusinessPerformances=new List<BusinessPerformance>();
            VerificationInfos=new List<VerificationInfo>();
        }
        /// <summary>
        /// 待转单
        /// </summary>
        [Display(Name = "待转单")]
        public int? Waiting { get; set; }
        /// <summary>
        /// 正处理
        /// </summary>
        [Display(Name = "正处理")]
        public int? Doing { get; set; }
        /// <summary>
        /// 已确认
        /// </summary>
        [Display(Name = "已确认")]
        public int? Confirm { get; set; }
        /// <summary>
        /// 已完成
        /// </summary>
        [Display(Name = "已完成")]
        public int? Done { get; set; }
        /// <summary>
        /// 今日预出刊
        /// </summary>
        [Display(Name = "今日预出刊")]
        public int? Today { get; set; }
        /// <summary>
        /// 明日预出刊
        /// </summary>
        [Display(Name = "明日预出刊")]
        public int? Tomorrow { get; set; }
        /// <summary>
        /// 销售金额
        /// </summary>
        [Display(Name = "销售金额")]
        public decimal? SellMoney { get; set; }
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
        /// 销售订单
        /// </summary>
        [Display(Name = "销售订单")]
        public int? OrderCount { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? Sort { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        [Display(Name = "月份")]
        public string Month { get; set; }
        /// <summary>
        /// 销售业绩
        /// </summary>
        [Display(Name = "销售业绩")]
        public List<BusinessPerformance> BusinessPerformances { get; set; }
        /// <summary>
        /// 未核销明细
        /// </summary>
        [Display(Name = "未核销明细")]
        public List<VerificationInfo> VerificationInfos { get; set; }

    }

    public class VerificationInfo
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        [Display(Name = "公司名称")]
        public string CompanyName { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        [Display(Name = "联系人")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        [Display(Name = "联系人")]
        public string LinkManId { get; set; }
        /// <summary>
        /// 未核销金额
        /// </summary>
        [Display(Name = "未核销金额")]
        public decimal? VerificationMoney { get; set; }
    }
    public class BusinessPerformance
    {
        /// <summary>
        /// 经办人
        /// </summary>
        [Display(Name = "经办人")]
        public string Transactor { get; set; }
        /// <summary>
        /// 销售总额
        /// </summary>
        [Display(Name = "销售总额")]
        public decimal? TotalMoney { get; set; }
        /// <summary>
        /// 采购总成本
        /// </summary>
        [Display(Name = "采购总成本")]
        public decimal? TotalPurchaseMoney { get; set; }
        /// <summary>
        /// 总利润
        /// </summary>
        [Display(Name = "总利润")]
        public decimal? TotalProfitMoney { get; set; }
        /// <summary>
        /// 无税金额
        /// </summary>
        [Display(Name = "无税金额")]
        public decimal? TotalSellMoney { get; set; }
        /// <summary>
        /// 未核销金额
        /// </summary>
        [Display(Name = "未核销金额")]
        public decimal? TotalVerificationMoney { get; set; }
        /// <summary>
        /// 已核销金额
        /// </summary>
        [Display(Name = "已核销金额")]
        public decimal? TotalConfirmVerificationMoney { get; set; }
        /// <summary>
        /// 利润率
        /// </summary>
        [Display(Name = "利润率")]
        public decimal? Profit { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        [Display(Name = "月份")]
        public string Month { get; set; }
        /// <summary>
        /// 媒体类型
        /// </summary>
        [Display(Name = "媒体类型")]
        public string MediaTypeName { get; set; }
    }
}
