using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Business
{
  public  class BusinessReturnOrder:BaseEntity
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [Display(Name = "订单编号")]
        public string BusinessOrderId { get; set; }
        /// <summary>
        /// 退款编号
        /// </summary>
        [Display(Name = "退款编号")]
        public string ReturnOrderNum { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        [Display(Name = "退款金额")]
        public decimal? TotalMoney { get; set; }
        
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
    }
}
