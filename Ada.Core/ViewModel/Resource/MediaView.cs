﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ada.Core.Domain.Resource;

namespace Ada.Core.ViewModel.Resource
{
    public class MediaView : BaseView
    {
        public MediaView()
        {
            Medias=new List<Media>();
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
        /// 发布频率
        /// </summary>
        [Display(Name = "发布频率")]
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
        /// 媒体说明
        /// </summary>
        [Display(Name = "媒体说明")]
        public string Content { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string Remark { get; set; }
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
        /// 是否轮播
        /// </summary>
        [Display(Name = "是否轮播")]
        public bool? IsSlide { get; set; }
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
        /// 广告位
        /// </summary>
        [Display(Name = "广告位")]
        public string AdPositionName { get; set; }
        /// <summary>
        /// 结算人
        /// </summary>
        [Display(Name = "结算人")]
        [Required]
        public string LinkManId { get; set; }
        /// <summary>
        /// 结算人
        /// </summary>
        [Display(Name = "结算人")]
        public string LinkManName { get; set; }
        /// <summary>
        /// 媒体分类
        /// </summary>
        [Display(Name = "媒体分类")]
        public List<MediaTagView> MediaTags { get; set; }
        /// <summary>
        /// 媒体组
        /// </summary>
        [Display(Name = "媒体组")]
        public List<MediaGroupView> MediaGroups { get; set; }
        /// <summary>
        /// 媒体分类
        /// </summary>
        [Display(Name = "媒体分类")]
        public string MediaTagStr { get; set; }
        /// <summary>
        /// 媒体分类
        /// </summary>
        [Display(Name = "媒体分类")]
        public List<string> MediaTagIds { get; set; }
        /// <summary>
        /// 媒体类别
        /// </summary>
        [Display(Name = "媒体类别")]
        public string MediaTypeId { get; set; }
        /// <summary>
        /// 媒体类别
        /// </summary>
        [Display(Name = "媒体类别")]
        public string MediaTypeIndex { get; set; }
        /// <summary>
        /// 媒体类别
        /// </summary>
        [Display(Name = "媒体类别")]
        public string MediaTypeName { get; set; }
        /// <summary>
        /// 渠道类型
        /// </summary>
        [Display(Name = "渠道类型")]
        public string ChannelType { get; set; }
        /// <summary>
        /// 媒体价格
        /// </summary>
        [Display(Name = "媒体价格")]
        public List<MediaPriceView> MediaPrices { get; set; }

        /// <summary>
        /// 价格范围
        /// </summary>
        [Display(Name = "价格范围")]
        public decimal? PriceStart { get; set; }
        /// <summary>
        /// 价格范围
        /// </summary>
        [Display(Name = "价格范围")]
        public decimal? PriceEnd { get; set; }
        /// <summary>
        /// 创建人员
        /// </summary>
        [Display(Name = "创建人员")]
        public string AddedBy { get; set; }
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
        /// 媒体名称集合
        /// </summary>
        [Display(Name = "媒体名称集合")]
        public string MediaNames { get; set; }
        /// <summary>
        /// 媒体ID集合
        /// </summary>
        [Display(Name = "媒体ID集合")]
        public string MediaIDs { get; set; }
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
        /// 媒体集合
        /// </summary>
        [Display(Name = "媒体集合")]
        public List<Media> Medias { get; set; }
    }

}
