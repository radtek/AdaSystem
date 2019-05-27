using System.Web.Mvc;

namespace Demand
{
    public class DemandAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Demand";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "Demand.Controllers" }
            );
        }
    }
}