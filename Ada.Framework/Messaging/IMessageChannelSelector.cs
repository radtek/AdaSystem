using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;

namespace Ada.Framework.Messaging
{
    /// <summary>
    /// 消息频道选择器接口
    /// </summary>
    public interface IMessageChannelSelector : IDependency
    {
        MessageChannelSelectorResult GetChannel(string messageType, object payload);
    }
    public class MessageChannelSelectorResult
    {
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }
        public Func<IMessageChannel> MessageChannel { get; set; }
    }
}
