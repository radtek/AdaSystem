using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain
{
    public class Consts
    {
        /// <summary>
        /// 状态开启 1
        /// </summary>
        public const short StateNormal = (short)StatusEnum.Normal;
        /// <summary>
        /// 状态关闭 0
        /// </summary>
        public const short StateLock = (short)StatusEnum.Locked;
    }
}
