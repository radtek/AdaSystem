using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.ViewModel.Admin;

namespace Ada.Core.ViewModel.Business
{
    public class BusinessOrderDetailView : BaseView
    {
        
        /// <summary>
        /// 税额
        /// </summary>
        [Display(Name = "税额")]
        public decimal? TaxMoney { get; set; }
        /// <summary>
        /// 折扣金额
        /// </summary>
        [Display(Name = "优惠金额")]
        public decimal? DiscountMoney { get; set; }
        /// <summary>
        /// 税率%
        /// </summary>
        [Display(Name = "税率%")]
        public decimal? Tax { get; set; }
        /// <summary>
        /// 折扣%
        /// </summary>
        [Display(Name = "折扣%")]
        public decimal? DiscountRate { get; set; }
        /// <summary>
        /// 广告位
        /// </summary>
        [Display(Name = "广告位")]
        public string AdPositionName { get; set; }
        /// <summary>
        /// 销售金额
        /// </summary>
        [Display(Name = "销售金额")]
        public decimal? Money { get; set; }
        /// <summary>
        /// 参考成本
        /// </summary>
        [Display(Name = "参考成本")]
        public decimal? CostMoney { get; set; }
        /// <summary>
        /// 无税金额
        /// </summary>
        [Display(Name = "无税金额")]
        public decimal? SellMoney { get; set; }
        /// <summary>
        /// 利润金额
        /// </summary>
        [Display(Name = "利润金额")]
        public decimal? ProfitMoney { get; set; }
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
        /// 审核状态
        /// </summary>
        [Display(Name = "审核状态")]
        public short? AuditStatus { get; set; }
        /// <summary>
        /// 出刊日期
        /// </summary>
        [Display(Name = "出刊日期")]
        public DateTime? PublishDate { get; set; }
        /// <summary>
        /// 出刊日期
        /// </summary>
        [Display(Name = "出刊日期")]
        public string PublishDateStr { get; set; }
        /// <summary>
        /// 预出刊日期
        /// </summary>
        [Display(Name = "预出刊日期")]
        public DateTime? PrePublishDate { get; set; }
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
        /// 媒体类型
        /// </summary>
        [Display(Name = "媒体类型")]
        public string MediaTypeId { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaName { get; set; }
        /// <summary>
        /// 经办媒介
        /// </summary>
        [Display(Name = "经办媒介")]
        public string MediaByPurchase { get; set; }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaPriceId { get; set; }
        /// <summary>
        /// 采购状态
        /// </summary>
        [Display(Name = "采购状态")]
        public short? PurchaseStatus { get; set; }
        /// <summary>
        /// 订单状态 0未转单 1已转采购
        /// </summary>
        [Display(Name = "订单状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        [Display(Name = "订单编号")]
        public string OrderNum { get; set; }
        /// <summary>
        /// 订单日期
        /// </summary>
        [Display(Name = "订单日期")]
        public DateTime? OrderDate { get; set; }
        /// <summary>
        /// 项目摘要
        /// </summary>
        [Display(Name = "项目摘要")]
        public string OrderRemark { get; set; }
        /// <summary>
        /// 出刊日期
        /// </summary>
        [Display(Name = "出刊日期")]
        public DateTime? PublishDateStart { get; set; }
        /// <summary>
        /// 出刊日期
        /// </summary>
        [Display(Name = "出刊日期")]
        public DateTime? PublishDateEnd { get; set; }
        /// <summary>
        /// 预出刊日期
        /// </summary>
        [Display(Name = "预出刊日期")]
        public DateTime? PrePublishDateStart { get; set; }
        /// <summary>
        /// 预出刊日期
        /// </summary>
        [Display(Name = "预出刊日期")]
        public DateTime? PrePublishDateEnd { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        [Display(Name = "所属部门")]
        public string OrganizationId { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        [Display(Name = "所属部门")]
        public string OrganizationName { get; set; }
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
        /// 公司名称
        /// </summary>
        [Display(Name = "公司名称")]
        public string CompanyName { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [Display(Name = "客户名称")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [Display(Name = "客户名称")]
        public string LinkManId { get; set; }
        /// <summary>
        /// 采购成本
        /// </summary>
        [Display(Name = "采购成本")]
        public decimal? PurchaseMoney { get; set; }
        /// <summary>
        /// 销售订单
        /// </summary>
        [Display(Name = "销售订单")]
        public string BusinessOrderId { get; set; }
        /// <summary>
        /// 销售总额
        /// </summary>
        [Display(Name = "销售总额")]
        public decimal? TotalMoney { get; set; }
        /// <summary>
        /// 采购总成本
        /// </summary>
        [Display(Name = "采购总成本")]
        public decimal? TotalPurchaseMoney { get; set; }
        /// <summary>
        /// 总利润
        /// </summary>
        [Display(Name = "总利润")]
        public decimal? TotalProfitMoney { get; set; }
        /// <summary>
        /// 无税金额
        /// </summary>
        [Display(Name = "无税金额")]
        public decimal? TotalSellMoney { get; set; }
        /// <summary>
        /// 未核销金额
        /// </summary>
        [Display(Name = "未核销金额")]
        public decimal? TotalVerificationMoney { get; set; }
        /// <summary>
        /// 已核销金额
        /// </summary>
        [Display(Name = "已核销金额")]
        public decimal? TotalConfirmVerificationMoney { get; set; }
        /// <summary>
        /// 利润率
        /// </summary>
        [Display(Name = "利润率")]
        public decimal? Profit { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 申请说明
        /// </summary>
        [Display(Name = "申请说明")]
        public string AuditRemark { get; set; }
        /// <summary>
        /// 申请修改金额
        /// </summary>
        [Display(Name = "申请修改金额")]
        public decimal? RequestSellMoney { get; set; }
        /// <summary>
        /// 是否评论
        /// </summary>
        [Display(Name = "是否评论")]
        public bool? IsComment { get; set; }

    }
}
