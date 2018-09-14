using System.Web.Mvc;

namespace Content
{
    public class ContentAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Content";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "Content.Controllers" }
            );
        }
    }
}