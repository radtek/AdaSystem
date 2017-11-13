using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Business
{
  public  class BusinessReturnOrderDetail : BaseEntity
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [Display(Name = "订单名称")]
        public string BusinessOrderDetailId { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        [Display(Name = "退款金额")]
        public decimal? Money { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        [Display(Name = "退款原因")]
        public string ReturnReason { get; set; }
        /// <summary>
        /// 经办业务
        /// </summary>
        [Display(Name = "退款类型")]
        public string ReturnType { get; set; }
        /// <summary>
        /// 退款日期
        /// </summary>
        [Display(Name = "退款日期")]
        public DateTime? ReturnDate { get; set; }
        /// <summary>
        /// 联系客户
        /// </summary>
        [Display(Name = "退款订单")]
        public string BusinessReturnOrderId { get; set; }
        public virtual BusinessReturnOrder BusinessReturnOrder { get; set; }
    }
}
