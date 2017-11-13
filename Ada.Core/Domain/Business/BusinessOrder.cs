using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Customer;

namespace Ada.Core.Domain.Business
{
  public  class BusinessOrder:BaseEntity
    {
        public BusinessOrder()
        {
            BusinessOrderDetails=new HashSet<BusinessOrderDetail>();
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        [Display(Name = "订单编号")]
        public string OrderNum { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [Display(Name = "订单金额")]
        public decimal? TotalMoney { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [Display(Name = "未核销金额")]
        public decimal? VerificationMoney { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [Display(Name = "已核销金额")]
        public decimal? ConfirmVerificationMoney { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [Display(Name = "核销状态")]
        public short? VerificationStatus { get; set; }
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
        /// 订单状态
        /// </summary>
        [Display(Name = "订单状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 联系客户
        /// </summary>
        [Display(Name = "联系客户")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 联系客户
        /// </summary>
        [Display(Name = "联系客户")]
        public string LinkManId { get; set; }
        public virtual LinkMan LinkMan { get; set; }
        public virtual ICollection<BusinessOrderDetail> BusinessOrderDetails { get; set; }
    }
}
