using System.Web.Mvc;

namespace Vote
{
    public class VoteAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Vote";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "Vote.Controllers" }
            );
        }
    }
}