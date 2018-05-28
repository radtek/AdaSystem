using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Customer;

namespace Ada.Core.Domain.Resource
{
    public class Media : BaseEntity
    {
        public Media()
        {
            MediaTags=new HashSet<MediaTag>();
            MediaPrices=new HashSet<MediaPrice>();
            MediaGroups=new HashSet<MediaGroup>();
            MediaComments=new HashSet<MediaComment>();
            MediaArticles=new HashSet<MediaArticle>();
            MediaAppointments = new HashSet<MediaAppointment>();
        }
        /// <summary>
        /// 媒体名称
        /// </summary>
        [Display(Name = "媒体名称")]
        public string MediaName { get; set; }
        /// <summary>
        /// 媒体ID
        /// </summary>
        [Display(Name = "媒体ID")]
        public string MediaID { get; set; }
        /// <summary>
        /// 媒体链接
        /// </summary>
        [Display(Name = "媒体链接")]
        public string MediaLink { get; set; }
        /// <summary>
        /// 媒体Logo
        /// </summary>
        [Display(Name = "媒体Logo")]
        public string MediaLogo { get; set; }
        /// <summary>
        /// 媒体二维码
        /// </summary>
        [Display(Name = "媒体二维码")]
        public string MediaQR { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        [Display(Name = "是否认证")]
        public bool? IsAuthenticate { get; set; }
        /// <summary>
        /// 是否原创
        /// </summary>
        [Display(Name = "是否原创")]
        public bool? IsOriginal { get; set; }
        /// <summary>
        /// 是否带评论
        /// </summary>
        [Display(Name = "是否带评论")]
        public bool? IsComment { get; set; }
        /// <summary>
        /// 粉丝数
        /// </summary>
        [Display(Name = "粉丝数")]
        public int? FansNum { get; set; }
        /// <summary>
        /// 最近头条阅读数
        /// </summary>
        [Display(Name = "最近头条阅读数")]
        public int? LastReadNum { get; set; }
        /// <summary>
        /// 平均阅读数
        /// </summary>
        [Display(Name = "平均阅读数")]
        public int? AvgReadNum { get; set; }
        /// <summary>
        /// 最近发布频率
        /// </summary>
        [Display(Name = "最近发布频率")]
        public int? PublishFrequency { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        [Display(Name = "地区")]
        public string Area { get; set; }
        /// <summary>
        /// 最后推送时间
        /// </summary>
        [Display(Name = "最后推送时间")]
        public DateTime? LastPushDate { get; set; }
        /// <summary>
        /// 认证类型
        /// </summary>
        [Display(Name = "认证类型")]
        public string AuthenticateType { get; set; }
        /// <summary>
        /// 转发数
        /// </summary>
        [Display(Name = "转发数")]
        public int? TransmitNum { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        [Display(Name = "评论数")]
        public int? CommentNum { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        [Display(Name = "点赞数")]
        public int? LikesNum { get; set; }
        /// <summary>
        /// 作品数
        /// </summary>
        [Display(Name = "作品数")]
        public int? PostNum { get; set; }
        /// <summary>
        /// 平均发布数
        /// </summary>
        [Display(Name = "平均发布数")]
        public int? MonthPostNum { get; set; }
        /// <summary>
        /// 关注数
        /// </summary>
        [Display(Name = "关注数")]
        public int? FriendNum { get; set; }
        /// <summary>
        /// 媒体摘要
        /// </summary>
        [Display(Name = "媒体摘要")]
        public string Abstract { get; set; }
        /// <summary>
        /// 媒体说明
        /// </summary>
        [Display(Name = "媒体说明")]
        public string Content { get; set; }
        /// <summary>
        /// 渠道类型
        /// </summary>
        [Display(Name = "渠道类型")]
        public string ChannelType { get; set; }
        /// <summary>
        /// 人气值
        /// </summary>
        [Display(Name = "人气值")]
        public int? ClickNum { get; set; }
        /// <summary>
        /// 是否热门
        /// </summary>
        [Display(Name = "是否热门")]
        public bool? IsHot { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        [Display(Name = "是否置顶")]
        public bool? IsTop { get; set; }
        /// <summary>
        /// 是否采集
        /// </summary>
        [Display(Name = "是否采集")]
        public bool? IsSlide { get; set; }
        /// <summary>
        /// 采集组
        /// </summary>
        [Display(Name = "采集组")]
        public string CollectionGroup { get; set; }
        /// <summary>
        /// 最后一次采集时间
        /// </summary>
        [Display(Name = "最后一次采集时间")]
        public DateTime? CollectionDate { get; set; }
        /// <summary>
        /// 是否轮播
        /// </summary>
        [Display(Name = "是否轮播")]
        public bool? IsLoop { get; set; }
        /// <summary>
        /// 是否推荐
        /// </summary>
        [Display(Name = "是否推荐")]
        public bool? IsRecommend { get; set; }
        /// <summary>
        /// 媒体状态
        /// </summary>
        [Display(Name = "媒体状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 数据更新日期
        /// </summary>
        [Display(Name = "数据更新日期")]
        public DateTime? ApiUpDate { get; set; }
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
        /// 平台
        /// </summary>
        [Display(Name = "平台")]
        public string Platform { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string Sex { get; set; }
        /// <summary>
        /// 客户端
        /// </summary>
        [Display(Name = "客户端")]
        public string Client { get; set; }
        /// <summary>
        /// 收录效果
        /// </summary>
        [Display(Name = "收录效果")]
        public string SEO { get; set; }
        /// <summary>
        /// 出稿速度
        /// </summary>
        [Display(Name = "出稿速度")]
        public string Efficiency { get; set; }
        /// <summary>
        /// 资源类型
        /// </summary>
        [Display(Name = "资源类型")]
        public string ResourceType { get; set; }
        /// <summary>
        /// 媒体频道
        /// </summary>
        [Display(Name = "媒体频道")]
        public string Channel { get; set; }
        /// <summary>
        /// 运营
        /// </summary>
        [Display(Name = "运营")]
        public short? Cooperation { get; set; }
        /// <summary>
        /// 保价期
        /// </summary>
        [Display(Name = "保价期")]
        public short? PriceProtectionDate { get; set; }
        /// <summary>
        /// 是否预付
        /// </summary>
        [Display(Name = "是否预付")]
        public bool? PriceProtectionIsPrePay { get; set; }
        /// <summary>
        /// 是否提供品牌
        /// </summary>
        [Display(Name = "是否提供品牌")]
        public bool? PriceProtectionIsBrand { get; set; }
        /// <summary>
        /// 保价备注
        /// </summary>
        [Display(Name = "保价备注")]
        public string PriceProtectionRemark { get; set; }
        /// <summary>
        /// 媒体类型
        /// </summary>
        [Display(Name = "媒体类型")]
        public string MediaTypeId { get; set; }
        public virtual MediaType MediaType { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        [Display(Name = "结算人")]
        public string LinkManId { get; set; }
        public virtual LinkMan LinkMan { get; set; }
        public virtual ICollection<MediaTag> MediaTags { get; set; }
        public virtual ICollection<MediaPrice> MediaPrices { get; set; }
        public virtual ICollection<MediaGroup> MediaGroups { get; set; }
        public virtual ICollection<MediaComment> MediaComments { get; set; }
        public virtual ICollection<MediaArticle> MediaArticles { get; set; }
        public virtual ICollection<MediaAppointment> MediaAppointments { get; set; }
    }
}
