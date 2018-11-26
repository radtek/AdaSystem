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
    [HubName("pushHub")]
    public class PushHub : Hub
    {
        public static ConcurrentDictionary<string, string> OnLineUsers = new ConcurrentDictionary<string, string>();
        [HubMethodName("all")]
        public void All(string title,string message)
        {
            Clients.All.recive(JsonConvert.SerializeObject(new{title,message,date= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }));//全部收到
        }
        [HubMethodName("others")]
        public void Others(string title, string message)
        {
            Clients.Others.recive(JsonConvert.SerializeObject(new { title, message, date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }));//别人可以收到，自己收不到
        }
        [HubMethodName("caller")]
        public void Caller(string id, string title, string message)
        {
            //string clientName = OnLineUsers[Context.ConnectionId];
            Clients.Caller.recive(JsonConvert.SerializeObject(new { title, message, date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }));//只有自己收到
            Clients.Client(id).recive(JsonConvert.SerializeObject(new { title, message, date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }));
        }

        public override Task OnConnected()
        {
            string clientName = Context.QueryString["clientName"];
            OnLineUsers.AddOrUpdate(Context.ConnectionId, clientName, (key, value) => clientName);
            
            Clients.All.userChange(JsonConvert.SerializeObject(new { date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg =
                $"{clientName} 上线了。", users = OnLineUsers.Select(d=>d.Value).Distinct().ToArray() }));
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string clientName = Context.QueryString["clientName"];
            Clients.All.userChange(JsonConvert.SerializeObject(new
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                msg =
                    $"{clientName} 离线了。",
                users = OnLineUsers.Select(d => d.Value).Distinct().ToArray()
            }));
            OnLineUsers.TryRemove(Context.ConnectionId, out clientName);
            return base.OnDisconnected(stopCalled);
        }
    }
}