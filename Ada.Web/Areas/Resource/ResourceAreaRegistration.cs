using System.Web.Mvc;

namespace Resource
{
    public class ResourceAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Resource";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "Resource.Controllers" }
            );
        }
    }
}