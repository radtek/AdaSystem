using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Setting
{
    /// <summary>
    /// 微广参数配置
    /// </summary>
    public class WeiGuang
    {
        public WeiGuang()
        {
            PurchaseExportRows = 10;
            BusinessExportRows = 10;
            PurchaseSeachRows = 10;
            BusinessSeachRows = 10;
            BusinessOrderExportRows = 10;
            Percentage1 = 0.15M;
            Percentage2 = 0.12M;
            ReturnDays1 = 183;
            ReturnDays2 = 365;
        }
        /// <summary>
        /// 媒介资源查询数
        /// </summary>
        [Display(Name = "媒介资源查询数")]
        public int PurchaseSeachRows { get; set; }
        /// <summary>
        /// 销售资源查询数
        /// </summary>
        [Display(Name = "销售资源查询数")]
        public int BusinessSeachRows { get; set; }
        /// <summary>
        /// 媒介资源导出数
        /// </summary>
        [Display(Name = "媒介资源导出数")]
        public int PurchaseExportRows { get; set; }
        /// <summary>
        /// 销售资源导出数
        /// </summary>
        [Display(Name = "销售资源导出数")]
        public int BusinessExportRows { get; set; }
        /// <summary>
        /// 销售订单导出数
        /// </summary>
        [Display(Name = "销售订单导出数")]
        public int BusinessOrderExportRows { get; set; }
        /// <summary>
        /// 提成系数
        /// </summary>
        [Display(Name = "提成系数")]
        public decimal Percentage1 { get; set; }
        /// <summary>
        /// 提成系数
        /// </summary>
        [Display(Name = "提成系数")]
        public decimal Percentage2 { get; set; }
        /// <summary>
        /// 回款天数
        /// </summary>
        [Display(Name = "回款天数")]
        public int ReturnDays1 { get; set; }
        /// <summary>
        /// 回款天数
        /// </summary>
        [Display(Name = "回款天数")]
        public int ReturnDays2 { get; set; }
    }
}
