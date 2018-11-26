using System.Web.Mvc;

namespace WebSocket
{
    public class WebSocketAreaRegistration : AreaRegistration
    {
        public override string AreaName => "WebSocket";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "WebSocket.Controllers" }
            );
        }
    }
}