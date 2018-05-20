using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Finance;
using Ada.Core.Domain.Purchase;
using Ada.Core.Domain.Resource;

namespace Ada.Core.Domain.Customer
{
   public class LinkMan:BaseEntity
    {
        public LinkMan()
        {
            PayAccounts=new HashSet<PayAccount>();
            Medias=new HashSet<Media>();
            BusinessOrders=new HashSet<BusinessOrder>();
            PurchaseOrderDetails = new HashSet<PurchaseOrderDetail>();
            BusinessPayees=new HashSet<BusinessPayee>();
            BillPayments=new HashSet<BillPayment>();
            PurchasePayments=new HashSet<PurchasePayment>();
            FollowUps=new HashSet<FollowUp>();
            BusinessOffers=new HashSet<BusinessOffer>();
            BusinessInvoices=new HashSet<BusinessInvoice>();
        }
        /// <summary>
        /// 联系人
        /// </summary>
        [Display(Name = "联系人")]
        public string Name { get; set; }
        /// <summary>
        /// 工作称呼
        /// </summary>
        [Display(Name = "工作称呼")]
        public string WorkName { get; set; }
        /// <summary>
        /// 获取方式
        /// </summary>
        [Display(Name = "获取方式")]
        public string LinkManType { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [Display(Name = "联系电话")]
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
        /// QQ
        /// </summary>
        [Display(Name = "QQ")]
        public string QQ { get; set; }
        /// <summary>
        /// 微信
        /// </summary>
        [Display(Name = "微信")]
        public string WeiXin { get; set; }
        /// <summary>
        /// 通信地址
        /// </summary>
        [Display(Name = "通信地址")]
        public string Address { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string Sex { get; set; }
        /// <summary>
        /// 客户状态
        /// </summary>
        [Display(Name = "客户状态")]
        public string Status { get; set; }
        /// <summary>
        /// 相片
        /// </summary>
        [Display(Name = "相片")]
        public string Image { get; set; }
        /// <summary>
        /// OpenId
        /// </summary>
        [Display(Name = "OpenId")]
        public string OpenId { get; set; }
        /// <summary>
        /// 开发平台授权ID
        /// </summary>
        [Display(Name = "开发平台授权ID")]
        public string UnionId { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码")]
        public string Password { get; set; }
        /// <summary>
        /// 登陆账户
        /// </summary>
        [Display(Name = "登陆账户")]
        public string LoginName { get; set; }
        /// <summary>
        /// 是否锁定
        /// </summary>
        [Display(Name = "是否锁定")]
        public bool? IsLock { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [Display(Name = "公司名称")]
        public string CommpanyId { get; set; }
        public virtual Commpany Commpany { get; set; }

        public virtual ICollection<PayAccount> PayAccounts { get; set; }
        public virtual ICollection<Media> Medias { get; set; }
        public virtual ICollection<BusinessOrder> BusinessOrders { get; set; }
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual ICollection<BusinessPayee> BusinessPayees { get; set; }
        public virtual ICollection<BillPayment> BillPayments { get; set; }
        public virtual ICollection<PurchasePayment> PurchasePayments { get; set; }
        public virtual ICollection<FollowUp> FollowUps { get; set; }
        public virtual ICollection<BusinessOffer> BusinessOffers { get; set; }
        public virtual ICollection<BusinessInvoice> BusinessInvoices { get; set; }
    }
}
