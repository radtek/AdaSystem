using System.Collections.Generic;
using Ada.Core;

namespace Ada.Framework.Messaging
{
    /// <summary>
    /// 消息处理公共接口
    /// </summary>
    public interface IMessageChannel : IDependency
    {
        void Process(IDictionary<string, object> parameters);
    }
}
