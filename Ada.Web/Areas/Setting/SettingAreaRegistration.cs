using System.Web.Mvc;

namespace Setting
{
    public class SettingAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Setting";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "Setting.Controllers" }
            );
        }
    }
}