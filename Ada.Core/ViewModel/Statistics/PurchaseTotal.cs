using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Statistics
{
  public  class PurchaseTotal
    {
        public PurchaseTotal()
        {
            Tops=new List<MediaAddTop>();
            CompanyTops=new List<CompanyTop>();
            MediaUpdates=new List<MediaUpdate>();
        }
        /// <summary>
        /// 订单总数
        /// </summary>
        [Display(Name = "订单总数")]
        public int? OrderCount { get; set; }
        /// <summary>
        /// 待响应
        /// </summary>
        [Display(Name = "待响应")]
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
        /// 未到票数
        /// </summary>
        [Display(Name = "未到票数")]
        public int? InvoiceCount { get; set; }
        /// <summary>
        /// 今日预出刊
        /// </summary>
        [Display(Name = "今日出刊")]
        public int? Today { get; set; }
        /// <summary>
        /// 明日预出刊
        /// </summary>
        [Display(Name = "明日出刊")]
        public int? Tomorrow { get; set; }
        /// <summary>
        /// 预付金额
        /// </summary>
        [Display(Name = "预付金额")]
        public decimal? TotalPayMoney { get; set; }
        /// <summary>
        /// 已付金额
        /// </summary>
        [Display(Name = "已付金额")]
        public decimal? PayMoney { get; set; }
        /// <summary>
        /// 媒体资源
        /// </summary>
        [Display(Name = "媒体资源")]
        public string Medias { get; set; }
        /// <summary>
        /// 开发排行
        /// </summary>
        [Display(Name = "开发排行")]
        public List<MediaAddTop> Tops { get; set; }
        /// <summary>
        /// 供应商排行
        /// </summary>
        [Display(Name = "供应商排行")]
        public List<CompanyTop> CompanyTops { get; set; }
        /// <summary>
        /// 媒体更新情况
        /// </summary>
        [Display(Name = "媒体更新情况")]
        public List<MediaUpdate> MediaUpdates { get; set; }
    }

    public class MediaAddTop
    {
        /// <summary>
        /// 媒介人员
        /// </summary>
        [Display(Name = "媒介人员")]
        public string Transactor { get; set; }
        /// <summary>
        /// 开发数
        /// </summary>
        [Display(Name = "开发数")]
        public int? MediasCount { get; set; }
    }
    public class PurchaseAchievement
    {
        /// <summary>
        /// 媒介人员
        /// </summary>
        [Display(Name = "媒介人员")]
        public string Transactor { get; set; }
        /// <summary>
        /// 订单数
        /// </summary>
        [Display(Name = "订单数")]
        public int? OrderCount { get; set; }
        /// <summary>
        /// 采购金额（含税）
        /// </summary>
        [Display(Name = "采购金额（含税）")]
        public decimal? TotalMoney { get; set; }
        /// <summary>
        /// 已采购金额（含税）
        /// </summary>
        [Display(Name = "已采购金额（含税）")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        [Display(Name = "付款金额")]
        public decimal? PayMoney { get; set; }
        /// <summary>
        /// 压款金额
        /// </summary>
        [Display(Name = "压款金额")]
        public decimal? Unpaid { get; set; }
        /// <summary>
        /// 节省金额
        /// </summary>
        [Display(Name = "节省金额")]
        public decimal? Economize { get; set; }
    }

    public class CompanyTop
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        [Display(Name = "公司名称")]
        public string Name { get; set; }
        /// <summary>
        /// 开发数
        /// </summary>
        [Display(Name = "采购总额")]
        public decimal? TotalMoney { get; set; }
    }

    public class MediaUpdate
    {
        /// <summary>
        /// 媒体类型
        /// </summary>
        [Display(Name = "媒体类型")]
        public string TypeName { get; set; }
        /// <summary>
        /// 媒体总数
        /// </summary>
        [Display(Name = "媒体总数")]
        public int Total { get; set; }
        /// <summary>
        /// 已更新数
        /// </summary>
        [Display(Name = "已更新数")]
        public int Updated { get; set; }
        /// <summary>
        /// 未更新数
        /// </summary>
        [Display(Name = "未更新数")]
        public int NoUpdated { get; set; }
    }
}
