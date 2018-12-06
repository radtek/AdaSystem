using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Business
{
    /// <summary>
    /// 业务核销
    /// </summary>
   public class BusinessWriteOff:BaseEntity
    {
        public BusinessWriteOff()
        {
            BusinessOrderDetails = new HashSet<BusinessOrderDetail>();
            BusinessPayees=new HashSet<BusinessPayee>();
            BusinessWriteOffDetails=new HashSet<BusinessWriteOffDetail>();
        }
        /// <summary>
        /// 销账日期
        /// </summary>
        [Display(Name = "销账日期")]
        public DateTime? WriteOffDate { get; set; }
        /// <summary>
        /// 销账金额
        /// </summary>
        [Display(Name = "销账金额")]
        public decimal? Money { get; set; }
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
        public virtual ICollection<BusinessPayee> BusinessPayees { get; set; }
        public virtual ICollection<BusinessOrderDetail> BusinessOrderDetails { get; set; }
        public virtual ICollection<BusinessWriteOffDetail> BusinessWriteOffDetails { get; set; }
    }
}
