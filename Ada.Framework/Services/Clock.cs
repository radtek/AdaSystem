using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Framework.Caching;

namespace Ada.Framework.Services
{
    public class Clock : IClock
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public IVolatileToken When(TimeSpan duration)
        {
            return new AbsoluteExpirationToken(this, duration);
        }

        public IVolatileToken WhenAbs(DateTime absolute)
        {
            return new AbsoluteExpirationToken(this, absolute);
        }

        public class AbsoluteExpirationToken : IVolatileToken
        {
            private readonly IClock _clock;
            private readonly DateTime _invalidate;

            public AbsoluteExpirationToken(IClock clock, DateTime invalidate)
            {
                _clock = clock;
                _invalidate = invalidate;
            }

            public AbsoluteExpirationToken(IClock clock, TimeSpan duration)
            {
                _clock = clock;
                _invalidate = _clock.Now.Add(duration);
            }

            public bool IsCurrent
            {
                get
                {
                    return _clock.Now < _invalidate;
                }
            }
        }
    }
}
