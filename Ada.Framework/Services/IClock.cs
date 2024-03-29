﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Framework.Caching;

namespace Ada.Framework.Services
{
    public interface IClock : IVolatileProvider
    {
        /// <summary>
        /// Gets the current <see cref="DateTime"/> of the system, expressed in Utc
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Provides a <see cref="IVolatileToken"/> instance which can be used to cache some information for a 
        /// specific duration.
        /// </summary>
        /// <param name="duration">The duration that the token must be valid.</param>
        /// <example>
        /// This sample shows how to use the <see cref="When"/> method by returning the result of
        /// a method named LoadVotes(), which is computed every 10 minutes only.
        /// <code>
        /// _cacheManager.Get("votes",
        ///     ctx => {
        ///         ctx.Monitor(_clock.When(TimeSpan.FromMinutes(10)));
        ///         return LoadVotes();
        /// });
        /// </code>
        /// </example>
        IVolatileToken When(TimeSpan duration);

        /// <summary>
        /// Provides a <see cref="IVolatileToken"/> instance which can be used to cache some 
        /// until a specific date and time.
        /// </summary>
        /// <param name="absoluteUtc">The date and time that the token must be valid until.</param>
        /// <example>
        /// This sample shows how to use the <see cref="WhenAbs"/> method by returning the result of
        /// a method named LoadVotes(), which is computed once, and no more until the end of the year.
        /// <code>
        /// var endOfYear = _clock.UtcNow;
        /// endOfYear.Month = 12;
        /// endOfYear.Day = 31;
        /// 
        /// _cacheManager.Get("votes",
        ///     ctx => {
        ///         ctx.Monitor(_clock.WhenUtc(endOfYear));
        ///         return LoadVotes();
        /// });
        /// </code>
        /// </example>
        IVolatileToken WhenAbs(DateTime absoluteUtc);
    }
}
