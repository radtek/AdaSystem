using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Setting
{
    /// <summary>
    /// 微广参数配置
    /// </summary>
    public class WeiGuang
    {
        public WeiGuang()
        {
            PurchaseExportRows = 10;
            BusinessExportRows = 10;
            PurchaseSeachRows = 10;
            BusinessSeachRows = 10;
            BusinessOrderExportRows = 10;
            Percentage1 = 0.15M;
            Percentage2 = 0.12M;
            ReturnDays1 = 183;
            ReturnDays2 = 365;
            RequestArticleCount = 1;
            RequestMediaCount = 1;
            UserExportRows = 10;
            UserExportTimes = 5;
            UserExportGroupRows = 500;
            UserDemo = "18888888888";
            UserVIPExportRatio = 1;
            WebDevelopPush = false;
            CancleAppointmentPush = true;
            AppointmentPush = false;
            UserRequestMediaCount = 10;
            WorkFlowPush = false;
            ErpWebSocket = false;
            MediaGroupTotal = 10;
        }
        /// <summary>
        /// 资源分组数
        /// </summary>
        [Display(Name = "资源分组数")]
        public int MediaGroupTotal { get; set; }
        /// <summary>
        /// 媒介资源查询数
        /// </summary>
        [Display(Name = "资源查询数")]
        public int PurchaseSeachRows { get; set; }
        /// <summary>
        /// 销售资源查询数
        /// </summary>
        [Display(Name = "资源查询数")]
        public int BusinessSeachRows { get; set; }
        /// <summary>
        /// 媒介资源导出数
        /// </summary>
        [Display(Name = "媒介资源导出数")]
        public int PurchaseExportRows { get; set; }
        /// <summary>
        /// 销售资源导出数
        /// </summary>
        [Display(Name = "销售资源导出数")]
        public int BusinessExportRows { get; set; }
        /// <summary>
        /// 会员资源导出数
        /// </summary>
        [Display(Name = "会员资源导出数")]
        public int UserExportRows { get; set; }
        /// <summary>
        /// 会员资源分组导出数
        /// </summary>
        [Display(Name = "会员资源分组导出数")]
        public int UserExportGroupRows { get; set; }
        /// <summary>
        /// 会员每日资源导出次数
        /// </summary>
        [Display(Name = "会员每日资源导出次数")]
        public int UserExportTimes { get; set; }
        /// <summary>
        /// 会员VIP导出系数
        /// </summary>
        [Display(Name = "会员VIP导出系数")]
        public int UserVIPExportRatio { get; set; }
        /// <summary>
        /// 会员VIP组
        /// </summary>
        [Display(Name = "会员VIP组")]
        public string UserVIPGroup { get; set; }
        /// <summary>
        /// 会员测试账号
        /// </summary>
        [Display(Name = "会员测试账号")]
        public string UserDemo { get; set; }
        /// <summary>
        /// 会员测试账号IP白名单
        /// </summary>
        [Display(Name = "会员测试账号IP白名单")]
        public string UserDemoAllowIP { get; set; }
        /// <summary>
        /// 销售订单导出数
        /// </summary>
        [Display(Name = "销售订单导出数")]
        public int BusinessOrderExportRows { get; set; }
        /// <summary>
        /// 提成系数
        /// </summary>
        [Display(Name = "提成系数")]
        public decimal Percentage1 { get; set; }
        /// <summary>
        /// 提成系数
        /// </summary>
        [Display(Name = "提成系数")]
        public decimal Percentage2 { get; set; }
        /// <summary>
        /// 回款天数
        /// </summary>
        [Display(Name = "回款天数")]
        public int ReturnDays1 { get; set; }
        /// <summary>
        /// 回款天数
        /// </summary>
        [Display(Name = "回款天数")]
        public int ReturnDays2 { get; set; }
        /// <summary>
        /// 每日限定采集文章次数
        /// </summary>
        [Display(Name = "每日限定采集文章次数")]
        public int RequestArticleCount { get; set; }
        /// <summary>
        /// 每日限定采集资源次数
        /// </summary>
        [Display(Name = "每日限定采集资源次数")]
        public int RequestMediaCount { get; set; }
        /// <summary>
        /// 会员每日限定查看文章数据次数
        /// </summary>
        [Display(Name = "会员每日限定查看文章数据次数")]
        public int UserRequestMediaCount { get; set; }
        /// <summary>
        /// 网站开发推送
        /// </summary>
        [Display(Name = "网站开发推送")]
        public bool WebDevelopPush { get; set; }
        /// <summary>
        /// 取消预约推送
        /// </summary>
        [Display(Name = "取消预约推送")]
        public bool CancleAppointmentPush { get; set; }
        /// <summary>
        /// 预约提醒推送
        /// </summary>
        [Display(Name = "预约提醒推送")]
        public bool AppointmentPush { get; set; }
        /// <summary>
        /// 工作流程提醒推送
        /// </summary>
        [Display(Name = "工作流程提醒推送")]
        public bool WorkFlowPush { get; set; }
        /// <summary>
        /// 系统内部网页推送
        /// </summary>
        [Display(Name = "系统内部网页推送")]
        public bool ErpWebSocket { get; set; }
        /// <summary>
        /// 当月自营号负责人
        /// </summary>
        [Display(Name = "当月自营号负责人")]
        public string WeiXinHolder { get; set; }
    }
}
