using System.Web.Mvc;

namespace Files
{
    public class FilesAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Files";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "Files.Controllers" }
            );
        }
    }
}