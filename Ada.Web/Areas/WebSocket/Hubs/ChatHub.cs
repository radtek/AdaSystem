using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;

namespace WebSocket.Hubs
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        public static ConcurrentDictionary<string, string> OnLineUsers = new ConcurrentDictionary<string, string>();
        [HubMethodName("send")]
        public void Send(string message)
        {
            string clientName = OnLineUsers[Context.ConnectionId];
            Clients.All.receiveMessage(JsonConvert.SerializeObject(new
            {
                date= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                clientName,
                message
            }));
        }

        [HubMethodName("sendOne")]
        public void Send(string toUserId, string message)
        {
            string clientName = OnLineUsers[Context.ConnectionId];
            Clients.Caller.receiveMessage(JsonConvert.SerializeObject(new
            {
                date= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                info= $"您对 {OnLineUsers[toUserId]}",
                message
            }));
            Clients.Client(toUserId).receiveMessage(JsonConvert.SerializeObject(new
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                info = $"{clientName} 对您",
                message
            }));
        }
        public override Task OnConnected()
        {
            string clientName = Context.QueryString["clientName"];
            OnLineUsers.AddOrUpdate(Context.ConnectionId, clientName, (key, value) => clientName);

            Clients.All.userChange(JsonConvert.SerializeObject(new
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                msg =
                    $"{clientName} 上线了",
                users = OnLineUsers.ToArray()
            }));
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string clientName = Context.QueryString["clientName"];
            Clients.All.userChange(JsonConvert.SerializeObject(new
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                msg =
                    $"{clientName} 离线了",
                users = OnLineUsers.ToArray()
            }));
            OnLineUsers.TryRemove(Context.ConnectionId, out clientName);
            return base.OnDisconnected(stopCalled);
        }
    }
}