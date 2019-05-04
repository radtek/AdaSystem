using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;

namespace Ada.Core.Domain.Customer
{
   public class Commpany: BaseEntity
    {
        public Commpany()
        {
            LinkMans=new HashSet<LinkMan>();
            BusinessInvoices = new HashSet<BusinessInvoice>();
        }
        /// <summary>
        /// 公司名称
        /// </summary>
        [Display(Name = "公司名称")]
        public string Name { get; set; }
        /// <summary>
        /// 所在城市
        /// </summary>
        [Display(Name = "所在城市")]
        public string City { get; set; }
        /// <summary>
        /// 行业分类
        /// </summary>
        [Display(Name = "行业分类")]
        public string CommpanyType { get; set; }
        /// <summary>
        /// 公司等级
        /// </summary>
        [Display(Name = "公司等级")]
        public string CommpanyGrade { get; set; }
        /// <summary>
        /// 通信地址
        /// </summary>
        [Display(Name = "通信地址")]
        public string Address { get; set; }
        /// <summary>
        /// 公司电话
        /// </summary>
        [Display(Name = "公司电话")]
        public string Phone { get; set; }
        /// <summary>
        /// 经办人员
        /// </summary>
        [Display(Name = "经办人员")]
        public string Transactor { get; set; }
        /// <summary>
        /// 经办人员
        /// </summary>
        [Display(Name = "经办人员")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 是否供应商
        /// </summary>
        [Display(Name = "是否供应商")]
        public bool IsBusiness { get; set; }
        /// <summary>
        /// 是否合作公司
        /// </summary>
        [Display(Name = "是否合作公司")]
        public bool? IsCooperation { get; set; }

        public virtual ICollection<LinkMan> LinkMans { get; set; }
        public virtual ICollection<BusinessInvoice> BusinessInvoices { get; set; }

    }
}
