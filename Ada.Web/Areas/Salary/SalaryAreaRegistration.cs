using System.Web.Mvc;

namespace Salary
{
    public class SalaryAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Salary";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "Salary.Controllers" }
            );
        }
    }
}