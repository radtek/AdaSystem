using Ada.Core;

namespace Ada.Framework.Caching {
    public interface ICacheContextAccessor : ISingleDependency
    {
        IAcquireContext Current { get; set; }
    }
}