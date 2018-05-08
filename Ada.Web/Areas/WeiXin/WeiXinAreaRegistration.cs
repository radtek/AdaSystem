using System.Web.Mvc;

namespace WeiXin
{
    public class SettingAreaRegistration : AreaRegistration
    {
        public override string AreaName => "WeiXin";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "WeiXin.Controllers" }
            );
        }
    }
}