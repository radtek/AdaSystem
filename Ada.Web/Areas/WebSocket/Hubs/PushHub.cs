using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WebSocket.Hubs
{
    [HubName("pushHub")]
    public class PushHub : Hub
    {
    }
}