using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ada.Core.Infrastructure;
using Ada.Framework.Messaging;

namespace WeiXin.Services
{
    public class WeiXinPushChannelSelector : IMessageChannelSelector
    {
        public MessageChannelSelectorResult GetChannel(string messageType, object payload)
        {
            if (messageType == "Push")
            {
                return new MessageChannelSelectorResult
                {
                    Priority = 50,
                    MessageChannel = () => EngineContext.Current.Resolve<IWeiXinPushChannel>()
            };
            }
            return null;
        }
    }
}