using System.Web.Mvc;

namespace Finance
{
    public class FinanceAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Finance";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "Finance.Controllers" }
            );
        }
    }
}