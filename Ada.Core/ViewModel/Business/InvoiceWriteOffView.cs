using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Business
{
   public class InvoiceWriteOffView
    {
        /// <summary>
        /// 发票总额
        /// </summary>
        [Display(Name = "发票总额")]
        public decimal?  TotalInvoiceMoney{ get; set; }
        /// <summary>
        /// 收款总额
        /// </summary>
        [Display(Name = "收款总额")]
        public decimal? TotalReceivalesMoney { get; set; }
        /// <summary>
        /// 销售发票
        /// </summary>
        [Display(Name = "销售发票"),Required]
        public string BusinessInvoicesIds { get; set; }
        /// <summary>
        /// 收款单据
        /// </summary>
        [Display(Name = "收款单据"),Required]
        public string ReceivalesIds { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "备注信息")]
        public string Remark { get; set; }
    }
}
