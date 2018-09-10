using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tools.Models
{
    public class FriendsSet
    {
        public FriendsSet()
        {
            PhoneType = "iphone";
            Signal = 5;
            Power = 88;
            LockIco = false;
            LocationIco = false;
            ClockIco = false;
            BluetoothIco = false;
            PowerIco = true;
            Likes = 0;
            Comments = 0;
        }
        /// <summary>
        /// 手机类型
        /// </summary>
        [Display(Name = "手机类型")]
        public string PhoneType { get; set; }
        /// <summary>
        /// 信号强弱
        /// </summary>
        [Display(Name = "信号强弱")]
        public short Signal { get; set; }
        /// <summary>
        /// 运营商
        /// </summary>
        [Display(Name = "运营商")]
        public string Operator { get; set; }
        /// <summary>
        /// 网络制式
        /// </summary>
        [Display(Name = "网络制式")]
        public string Network { get; set; }
        /// <summary>
        /// 电量百分比
        /// </summary>
        [Display(Name = "电量百分比")]
        public short Power { get; set; }
        /// <summary>
        /// 锁定
        /// </summary>
        [Display(Name = "锁定")]
        public bool LockIco { get; set; }
        /// <summary>
        /// 定位
        /// </summary>
        [Display(Name = "定位")]
        public bool LocationIco { get; set; }
        /// <summary>
        /// 闹钟
        /// </summary>
        [Display(Name = "闹钟")]
        public bool ClockIco { get; set; }
        /// <summary>
        /// 蓝牙
        /// </summary>
        [Display(Name = "蓝牙")]
        public bool BluetoothIco { get; set; }
        /// <summary>
        /// 电量
        /// </summary>
        [Display(Name = "电量")]
        public bool PowerIco { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        [Display(Name = "发布人")]
        public string FriendId { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        [Display(Name = "发布人")]
        public string FriendName { get; set; }
        /// <summary>
        /// 内容类型
        /// </summary>
        [Display(Name = "内容类型")]
        public string ContentType { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        [Display(Name = "发布时间")]
        public string PublishDate { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        [Display(Name = "点赞数")]
        public int Likes { get; set; }
        /// <summary>
        /// 留言数
        /// </summary>
        [Display(Name = "留言数")]
        public int Comments { get; set; }
        /// <summary>
        /// 文字内容
        /// </summary>
        [Display(Name = "文字内容")]
        public string Text { get; set; }
        /// <summary>
        /// 上传图片
        /// </summary>
        [Display(Name = "上传图片")]
        public string Images { get; set; }
        /// <summary>
        /// 链接内容
        /// </summary>
        [Display(Name = "链接内容")]
        public string LinkContent { get; set; }
        /// <summary>
        /// 评价内容
        /// </summary>
        [Display(Name = "评价内容")]
        public string CommentContent { get; set; }
    }
}