using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel
{
   public class BaseView
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int? page { get; set; }
        /// <summary>
        /// 当前页总条数
        /// </summary>
        public int? rows { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        public int? total { get; set; }
        /// <summary>
        /// 升降序
        /// </summary>
        public string order { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string sort { get; set; }
    }
}
