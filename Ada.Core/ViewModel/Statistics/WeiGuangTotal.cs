using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;

namespace Ada.Core.ViewModel.Statistics
{
   public class WeiGuangTotal
    {
        public WeiGuangTotal()
        {
            MediaOrders=new List<MediaOrder>();
            OrderComments=new List<Comment>();
            MediaComments=new List<Comment>();
        }
        /// <summary>
        /// 销售订单
        /// </summary>
        [Display(Name = "销售订单")]
        public int? BusinessCount { get; set; }
        /// <summary>
        /// 媒介订单
        /// </summary>
        [Display(Name = "媒介订单")]
        public int? PurchaseCount { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [Display(Name = "订单状态")]
        public bool OrderStatus { get; set; }
        /// <summary>
        /// 金额状态
        /// </summary>
        [Display(Name = "金额状态")]
        public bool MoneyStatus { get; set; }
        /// <summary>
        /// 销售金额
        /// </summary>
        [Display(Name = "销售金额")]
        public decimal? BusinessSellMoney { get; set; }
        /// <summary>
        /// 未核销金额
        /// </summary>
        [Display(Name = "未核销金额")]
        public decimal? BusinessVerificationMoney { get; set; }
        /// <summary>
        /// 已核销金额
        /// </summary>
        [Display(Name = "已核销金额")]
        public decimal? BusinessConfirmVerificationMoney { get; set; }
        /// <summary>
        /// 总收入
        /// </summary>
        [Display(Name = "总收入")]
        public decimal? Income { get; set; }
        /// <summary>
        /// 总支出
        /// </summary>
        [Display(Name = "总支出")]
        public decimal? Expend { get; set; }
        /// <summary>
        /// 资源类型
        /// </summary>
        [Display(Name = "资源类型")]
        public List<MediaTypeView> MediaTypes { get; set; }
        /// <summary>
        /// 资源类型
        /// </summary>
        [Display(Name = "资源类型")]
        public List<MediaOrder> MediaOrders { get; set; }
        /// <summary>
        /// 订单评价
        /// </summary>
        [Display(Name = "订单评价")]
        public List<Comment> OrderComments { get; set; }
        /// <summary>
        /// 资源评价
        /// </summary>
        [Display(Name = "资源评价")]
        public List<Comment> MediaComments { get; set; }

    }

    public class MediaOrder
    {
        public string TypeName { get; set; }
        public string MediaName { get; set; }
        public string MediaID { get; set; }
        public string AdPostion { get; set; }
        public decimal? SellMoney { get; set; }
        public int? Count { get; set; }
    }
    public class Comment
    {
        public string Transactor { get; set; }
        public int? Count { get; set; }
    }
}
