using System.Web.Mvc;

namespace Purchase
{
    public class PurchaseAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Purchase";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "Purchase.Controllers" }
            );
        }
    }
}