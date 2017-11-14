using System.Web.Mvc;

namespace Business
{
    public class BusinessAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Business";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "Business.Controllers" }
            );
        }
    }
}