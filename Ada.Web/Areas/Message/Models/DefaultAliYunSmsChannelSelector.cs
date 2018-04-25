using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ada.Core.Infrastructure;
using Ada.Framework.Messaging;

namespace Message.Models
{
    public class DefaultAliYunSmsChannelSelector : IMessageChannelSelector
    {
        public MessageChannelSelectorResult GetChannel(string messageType, object payload)
        {
            if (messageType == "AliYun")
            {
                return new MessageChannelSelectorResult
                {
                    Priority = 50,
                    MessageChannel = () => EngineContext.Current.Resolve<IAliYunSmsChannel>()
            };
            }
            return null;
        }
    }
}