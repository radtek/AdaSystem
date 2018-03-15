using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Business
{
   public class OfferRatio:BaseEntity
    {
        public OfferRatio()
        {
            OfferRatioDetails=new HashSet<OfferRatioDetail>();
        }
        /// <summary>
        /// 规则状态
        /// </summary>
        [Display(Name = "规则状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 报价规则类型
        /// </summary>
        [Display(Name = "规则类型")]
        public string OfferType { get; set; }
        /// <summary>
        /// 报价规则名称
        /// </summary>
        [Display(Name = "规则名称")]
        public string Title { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        [Display(Name = "经办人")]
        public string Transactor { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        [Display(Name = "经办人")]
        public string TransactorId { get; set; }
        public virtual ICollection<OfferRatioDetail> OfferRatioDetails { get; set; }
    }
}
