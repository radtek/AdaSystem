using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ada.Core.ViewModel.Demand
{
    public class SubjectView : BaseView
    {
        public SubjectView()
        {
            Offer = 10;
            Details=new List<SelectListItem>();
            MaterialImage=new List<string>();
            MaterialRemark=new List<string>();
            MaterialThumbImage=new List<string>();
            MaterialSize = new List<int>();
            MaterialName=new List<string>();
            MaterialExt=new List<string>();
        }
        /// <summary>
        /// 需求名称
        /// </summary>
        [Display(Name = "需求名称")]
        public string Title { get; set; }
        /// <summary>
        /// 需求内容
        /// </summary>
        [Display(Name = "文案内容"),AllowHtml]
        public string Content { get; set; }
        /// <summary>
        /// 需求报价
        /// </summary>
        [Display(Name = "需求报价")]
        public decimal Offer { get; set; }
        /// <summary>
        /// 需求类型
        /// </summary>
        [Display(Name = "需求类型")]
        public string Type { get; set; }
        /// <summary>
        /// 需求类型
        /// </summary>
        [Display(Name = "发布者")]
        public string AddedBy { get; set; }
        /// <summary>
        /// 发布日期
        /// </summary>
        [Display(Name = "发布日期")]
        public DateTime? AddedDate { get; set; }
        /// <summary>
        /// 发布者信息
        /// </summary>
        [Display(Name = "发布者信息")]
        public List<SelectListItem> Details { get; set; }
        /// <summary>
        /// 发布者信息
        /// </summary>
        [Display(Name = "发布者信息")]
        public string DetailsJson { get; set; }
        /// <summary>
        /// 发布图片
        /// </summary>
        [Display(Name = "发布图片")]
        public List<string> MaterialImage { get; set; }
        /// <summary>
        /// 需求素材
        /// </summary>
        [Display(Name = "需求素材")]
        public List<string> MaterialThumbImage { get; set; }
        /// <summary>
        /// 素材描述
        /// </summary>
        [Display(Name = "素材描述")]
        public List<string> MaterialRemark { get; set; }
        /// <summary>
        /// 素材尺寸
        /// </summary>
        [Display(Name = "素材尺寸")]
        public List<int> MaterialSize { get; set; }
        /// <summary>
        /// 素材名称
        /// </summary>
        [Display(Name = "素材名称")]
        public List<string> MaterialName { get; set; }
        /// <summary>
        /// 素材扩展名
        /// </summary>
        [Display(Name = "素材扩展名")]
        public List<string> MaterialExt { get; set; }
        /// <summary>
        /// 执行进度
        /// </summary>
        [Display(Name = "执行进度")]
        public string Schedule { get; set; }

    }

}
