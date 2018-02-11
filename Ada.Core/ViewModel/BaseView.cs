using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel
{
    [Serializable]
    public class BaseView
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int? offset { get; set; }
        /// <summary>
        /// 当前页总条数
        /// </summary>
        public int? limit { get; set; }
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
        /// <summary>
        /// 排序字段
        /// </summary>
        public string search { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 时间范围
        /// </summary>
        public string DateRange { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        public List<string> Managers { get; set; }
    }
}
