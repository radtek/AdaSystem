using System.Web.Mvc;

namespace QuartzTask
{
    public class QuartzTaskAreaRegistration : AreaRegistration
    {
        public override string AreaName => "QuartzTask";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "QuartzTask.Controllers" }
            );
        }
    }
}