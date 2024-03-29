﻿using System.Web.Mvc;

namespace Dashboards
{
    public class DashboardsAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Dashboards";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                new[] { "Dashboards.Controllers" }
            );
        }
    }
}