using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Customer;
using Ada.Core.Domain.Resource;

namespace Ada.Core.ViewModel.Purchase
{
    public class PurchaseOrderDetailView : BaseView
    {
        /// <summary>
        /// 销售明细单
        /// </summary>
        [Display(Name = "销售明细单")]
        public string BusinessOrderDetailId { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        [Display(Name = "优惠金额")]
        public decimal? DiscountMoney { get; set; }
        /// <summary>
        /// 折扣%
        /// </summary>
        [Display(Name = "折扣%")]
        public decimal? DiscountRate { get; set; }
        /// <summary>
        /// 采购金额
        /// </summary>
        [Display(Name = "采购金额")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 成本金额
        /// </summary>
        [Display(Name = "成本金额")]
        public decimal? CostMoney { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        [Display(Name = "税额")]
        public decimal? TaxMoney { get; set; }
        /// <summary>
        /// 税率%
        /// </summary>
        [Display(Name = "税率%")]
        public decimal? Tax { get; set; }
        /// <summary>
        /// 无税金额
        /// </summary>
        [Display(Name = "无税金额")]
        public decimal? PurchaseMoney { get; set; }
        /// <summary>
        /// 预付定金
        /// </summary>
        [Display(Name = "预付定金")]
        public decimal? BargainMoney { get; set; }
        /// <summary>
        /// 未核销金额
        /// </summary>
        [Display(Name = "未核销金额")]
        public decimal? VerificationMoney { get; set; }
        /// <summary>
        /// 已核销金额
        /// </summary>
        [Display(Name = "已核销金额")]
        public decimal? ConfirmVerificationMoney { get; set; }
        /// <summary>
        /// 核销状态
        /// </summary>
        [Display(Name = "核销状态")]
        public short? VerificationStatus { get; set; }
        /// <summary>
        /// 广告位
        /// </summary>
        [Display(Name = "广告位")]
        public string AdPositionName { get; set; }
        /// <summary>
        /// 出刊日期
        /// </summary>
        [Display(Name = "出刊日期")]
        public DateTime? PublishDate { get; set; }
        /// <summary>
        /// 稿件标题
        /// </summary>
        [Display(Name = "稿件标题")]
        public string MediaTitle { get; set; }
        /// <summary>
        /// 出刊链接
        /// </summary>
        [Display(Name = "出刊链接")]
        public string PublishLink { get; set; }
        /// <summary>
        /// 媒体类型
        /// </summary>
        [Display(Name = "媒体类型")]
        public string MediaTypeName { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaName { get; set; }
        /// <summary>
        /// 采购状态
        /// </summary>
        [Display(Name = "采购状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 经办媒介
        /// </summary>
        [Display(Name = "经办媒介")]
        public string Transactor { get; set; }
        /// <summary>
        /// 经办媒介
        /// </summary>
        [Display(Name = "经办媒介")]
        public string TransactorId { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        [Display(Name = "审核人")]
        public string AuditBy { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        [Display(Name = "审核人")]
        public string AuditById { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        [Display(Name = "审核时间")]
        public DateTime? AuditDate { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        [Display(Name = "审核状态")]
        public short? AuditStatus { get; set; }
        /// <summary>
        /// 作废人
        /// </summary>
        [Display(Name = "作废人")]
        public string CancelBy { get; set; }
        /// <summary>
        /// 作废人
        /// </summary>
        [Display(Name = "作废人")]
        public string CancelById { get; set; }
        /// <summary>
        /// 作废时间
        /// </summary>
        [Display(Name = "作废时间")]
        public DateTime? CancelDate { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
        [Display(Name = "结算方式")]
        public string SettlementType { get; set; }
        /// <summary>
        /// 采购类型
        /// </summary>
        [Display(Name = "采购类型")]
        public string PurchaseType { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaPriceId { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        [Display(Name = "供应商")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        [Display(Name = "供应商")]
        public string LinkManId { get; set; }
        /// <summary>
        /// 采购订单
        /// </summary>
        [Display(Name = "采购订单")]
        public string PurchaseOrderId { get; set; }
        /// <summary>
        /// 采购编号
        /// </summary>
        [Display(Name = "采购编号")]
        public string PurchaseOrderNum { get; set; }
        /// <summary>
        /// 预出刊日期
        /// </summary>
        [Display(Name = "预出刊日期")]
        public DateTime? PrePublishDate { get; set; }
        /// <summary>
        /// 经办业务
        /// </summary>
        [Display(Name = "经办业务")]
        public string BusinessBy { get; set; }
        /// <summary>
        /// 单据时间
        /// </summary>
        [Display(Name = "单据时间")]
        public DateTime? OrderDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 是否请款
        /// </summary>
        [Display(Name = "是否请款")]
        public bool? IsPayment { get; set; }
    }
}
