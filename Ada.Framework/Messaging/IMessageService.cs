using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;

namespace Ada.Framework.Messaging
{
    public interface IMessageService : IDependency
    {
        void Send(string type, IDictionary<string, object> parameters);
    }
}
