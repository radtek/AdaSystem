using System.Web.Mvc;

namespace Message
{
    public class MessageAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Message";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "Message.Controllers" }
            );
        }
    }
}