using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Ada.Framework.Messaging
{
    public class DefaultMessageService :  IMessageService
    {
        private readonly IMessageChannelManager _messageChannelManager;
        public ILog Log { get; set; }
        public DefaultMessageService(IMessageChannelManager messageChannelManager)
        {
            _messageChannelManager = messageChannelManager;
        }

        public void Send(string type, IDictionary<string, object> parameters)
        {
            var messageChannel = _messageChannelManager.GetMessageChannel(type, parameters);
            if (messageChannel == null)
            {
                throw new ApplicationException("没有发现相关的消息处理频道:" + type);
            }
            messageChannel.Process(parameters);
        }

    }
}
