using System.Web.Mvc;

namespace DataReport
{
    public class DataReportAreaRegistration : AreaRegistration
    {
        public override string AreaName => "DataReport";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "DataReport.Controllers" }
            );
        }
    }
}