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
    }
}
