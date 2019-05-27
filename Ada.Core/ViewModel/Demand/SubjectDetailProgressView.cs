
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Ada.Core.ViewModel.Demand
{
   public class SubjectDetailProgressView : BaseView
    {
        public SubjectDetailProgressView()
        {
            MaterialImage=new System.Collections.Generic.List<string>();
            MaterialThumbImage = new System.Collections.Generic.List<string>();
            MaterialRemark = new System.Collections.Generic.List<string>();
            MaterialSize = new System.Collections.Generic.List<int>();
            MaterialName = new System.Collections.Generic.List<string>();
            MaterialExt = new System.Collections.Generic.List<string>();
        }
        /// <summary>
        /// 明细状态
        /// </summary>
        [Display(Name = "明细状态")]
        public short? SubjectDetailStatus { get; set; }
        /// <summary>
        /// 需求明细
        /// </summary>
        [Display(Name = "需求明细")]
        public string SubjectDetailId { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string Remark { get; set; }
        /// <summary>
        /// 跳转页面
        /// </summary>
        [Display(Name = "跳转页面")]
        public string Redirect { get; set; }
        /// <summary>
        /// 需求素材
        /// </summary>
        [Display(Name = "需求素材")]
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
    }
}
