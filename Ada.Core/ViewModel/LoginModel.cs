using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Tools;

namespace Ada.Core.ViewModel
{
   public class LoginModel
    {
        public LoginModel()
        {
            LoginLog=new LoginLog();
            IsSuccess = false;
        }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string OpenId { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public LoginLog LoginLog { get; set; }

    }

    public class LoginLog
    {
        public LoginLog()
        {
            LoginTime=DateTime.Now;
            IPAddress = Utils.GetIpAddress();
        }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }

        public DateTime LoginTime { get; set; }
    }
}
