using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using WebSocket.Models;

namespace WebSocket.Hubs
{
    [HubName("pushHub")]
    public class PushHub : Hub
    {
        public static ConcurrentDictionary<string, OnLineUser> OnLineUsers = new ConcurrentDictionary<string, OnLineUser>();
        public override Task OnConnected()
        {
            var user = new OnLineUser
            {
                Name = Context.QueryString["Name"],
                Image = Context.QueryString["Image"],
                UId = Context.QueryString["UId"]
            };
            if (OnLineUsers.Count(d => d.Value.UId == user.UId) >4)
            {
                var item = OnLineUsers.OrderBy(d => d.Value.Date).FirstOrDefault();
                Clients.Client(item.Key).stop(item.Key);
            }
            else
            {
                OnLineUsers.AddOrUpdate(Context.ConnectionId, user, (key, value) => user);
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = new OnLineUser
            {
                Name = Context.QueryString["Name"],
                Image = Context.QueryString["Image"],
                UId = Context.QueryString["UId"]
            };
            OnLineUsers.TryRemove(Context.ConnectionId, out user);
            return base.OnDisconnected(stopCalled);
        }
    }


}