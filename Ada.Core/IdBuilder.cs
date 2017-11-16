using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core
{
    public class IdBuilder
    {
        private static object locker = new object();
        private static object olocker = new object();
        private static int _sn = 0;
        public static string CreateIdNum()
        {
            lock (locker)
            {
                if (_sn == 9999)
                {
                    _sn = 0;
                }
                else
                {
                    _sn++;
                }
                return "X" + DateTime.Now.ToString("yyMMddHHmmss") + _sn.ToString().PadLeft(4, '0');
            }
        }
        public static string CreateOrderNum(string orderType="")
        {
            lock (olocker)
            {
                if (_sn == 9999)
                {
                    _sn = 0;
                }
                else
                {
                    _sn++;
                }
                return "VG" + orderType + DateTime.Now.ToString("yyMMdd") + _sn.ToString().PadLeft(4, '0');
            }
        }
        // 防止创建类的实例
        private IdBuilder() { }
    }
}
