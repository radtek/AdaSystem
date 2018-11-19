using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;
using MvcThrottle;

namespace Ada.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AdaExceptionAttribute());
            const int secondCount = 5;
            filters.Add(new ThrottleCustomFilter()
            {
                Policy = new ThrottlePolicy(
                    secondCount,
                    secondCount * 10,
                    secondCount * 10 * 5 * 2,
                    secondCount * 100 * 5 * 2)
                {
                    IpThrottling = true
                },
                Repository = new CacheRepository(),
                Logger = new ThrottleCustomLogger(),
                IpAddressParser = new ThrottleIpAddressParser(),
                QuotaExceededResponseCode = HttpStatusCode.MultipleChoices
            });
        }
    }
}