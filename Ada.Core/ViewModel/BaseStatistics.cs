using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel
{
   public class BaseStatistics
    {
        public BaseStatistics()
        {
            TotalCount = 0;
            TotalMoney = 0;
        }
        public string Key { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalMoney { get; set; }
    }
}
