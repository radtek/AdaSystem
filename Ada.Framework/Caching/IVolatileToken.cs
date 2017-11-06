namespace Ada.Framework.Caching {
    public interface IVolatileToken {
        bool IsCurrent { get; }
    }
}