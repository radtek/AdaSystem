using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel
{
  public  class HttpResultView
    {
        /// <summary>
        /// 响应码
        /// </summary>
        public int HttpCode { get; set; }
        /// <summary>
        /// 相关信息
        /// </summary>
        public string Msg { get; set; }
    }
}
