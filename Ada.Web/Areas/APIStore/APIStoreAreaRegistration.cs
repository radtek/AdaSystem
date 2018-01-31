using System.Web.Mvc;

namespace APIStore
{
    public class APIStoreAreaRegistration : AreaRegistration
    {
        public override string AreaName => "APIStore";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "APIStore.Controllers" }
            );
        }
    }
}