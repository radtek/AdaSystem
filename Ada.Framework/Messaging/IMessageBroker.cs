using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;

namespace Ada.Framework.Messaging
{
    public interface IMessageBroker : ISingleDependency
    {
        void Subscribe(string channel, Action<string, string> handler);
        void Publish(string channel, string message);
    }
}
