using System.Web.Mvc;

namespace WorkLog
{
    public class WorkLogAreaRegistration : AreaRegistration
    {
        public override string AreaName => "WorkLog";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "WorkLog.Controllers" }
            );
        }
    }
}