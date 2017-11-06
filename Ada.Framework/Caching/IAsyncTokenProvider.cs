using System;
using Ada.Core;

namespace Ada.Framework.Caching {
    public interface IAsyncTokenProvider:ISingleDependency {
        IVolatileToken GetToken(Action<Action<IVolatileToken>> task);
    }
}