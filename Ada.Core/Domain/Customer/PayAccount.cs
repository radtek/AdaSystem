using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Customer
{
   public class PayAccount:BaseEntity
    {
        /// <summary>
        /// 账户类型
        /// </summary>
        [Display(Name = "账户类型")]
        public string AccountType { get; set; }
        /// <summary>
        /// 账户名称
        /// </summary>
        [Display(Name = "账户名称")]
        public string AccountName { get; set; }
        /// <summary>
        /// 账户账号
        /// </summary>
        [Display(Name = "账户账号")]
        public string AccountNum { get; set; }
        /// <summary>
        /// 账户状态
        /// </summary>
        [Display(Name = "账户状态")]
        public short? Status{ get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        [Display(Name = "联系人")]
        public string LinkManId { get; set; }
        public virtual LinkMan LinkMan { get; set; }
    }
}
