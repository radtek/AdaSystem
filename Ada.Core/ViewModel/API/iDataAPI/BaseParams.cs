using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.API.iDataAPI
{
    public class BaseParams
    {
        public BaseParams()
        {
            IsLog = true;
        }
        /// <summary>
        /// API
        /// </summary>
        [Display(Name = "API")]
        public string CallIndex { get; set; }
        /// <summary>
        /// 媒体ID
        /// </summary>
        [Display(Name = "媒体ID")]
        public string UID { get; set; }
        /// <summary>
        /// 取页数
        /// </summary>
        [Display(Name = "取页数")]
        public int? PageNum { get; set; }
        /// <summary>
        /// 是否记录日志
        /// </summary>
        [Display(Name = "是否记录日志")]
        public bool IsLog { get; set; }
    }
}
