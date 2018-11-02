using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Ada.Core.Infrastructure;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Setting;
using Ada.Services.Setting;

namespace Ada.Framework.Filter
{
    
    public class WebStatusAttribute : AuthorizeAttribute
    {
       
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //base.OnAuthorization(filterContext);
            var setting = EngineContext.Current.Resolve<ISettingService>().GetSetting<Site>();
            if (!setting.SiteStatus)
            {
                //跳转
                ViewResult result = new ViewResult
                {
                    ViewName = "Maintain", //错误页
                    ViewData = new ViewDataDictionary(new HttpResultView() { HttpCode = 404, Msg = setting.SiteCloseReson }),       //指定模型
                    TempData = filterContext.Controller.TempData
                };
                filterContext.Result = result;
                return;
            }
            
        }
    }
}
