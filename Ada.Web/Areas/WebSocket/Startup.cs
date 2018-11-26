using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebSocket.Startup))]

namespace WebSocket
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
