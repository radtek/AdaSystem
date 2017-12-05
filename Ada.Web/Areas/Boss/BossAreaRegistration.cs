using System.Web.Mvc;

namespace Boss
{
    public class BossAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Boss";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "Boss.Controllers" }
            );
        }
    }
}