using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.API.iDataAPI
{
    public class DouYinParams : BaseParams
    {
        /// <summary>
        /// 关键字
        /// </summary>
        [Display(Name = "关键字")]
        public string KeyWord { get; set; }
    }
}
