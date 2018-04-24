using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core
{
    public interface IMessageBroker : ISingleDependency
    {
        void Subscribe(string channel, Action<string, string> handler);
        void Publish(string channel, string message);
    }
}
