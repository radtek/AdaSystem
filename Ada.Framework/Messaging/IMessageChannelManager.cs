using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;

namespace Ada.Framework.Messaging
{
    /// <summary>
    /// 消息管理接口
    /// </summary>
    public interface IMessageChannelManager : IDependency
    {
        IMessageChannel GetMessageChannel(string type, IDictionary<string, object> parameters);
    }
    public class MessageChannelManager : IMessageChannelManager
    {
        private readonly IEnumerable<IMessageChannelSelector> _messageChannelSelectors;

        public MessageChannelManager(IEnumerable<IMessageChannelSelector> messageChannelSelectors)
        {
            _messageChannelSelectors = messageChannelSelectors;
        }

        public IMessageChannel GetMessageChannel(string type, IDictionary<string, object> parameters)
        {
            var messageChannelResult = _messageChannelSelectors
                .Select(x => x.GetChannel(type, parameters))
                .Where(x => x != null)
                .OrderByDescending(x => x.Priority)
                .FirstOrDefault();

            return messageChannelResult?.MessageChannel();
        }
    }
}
