using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ada.Web.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "请输入您的用户名")]
        public string LoginName { get; set; }
        //[Required(ErrorMessage = "请输入登陆密码")]
        public string PassWord { get; set; }
        [Required(ErrorMessage = "请输入手机验证码")]
        public string Code { get; set; }
    }
}