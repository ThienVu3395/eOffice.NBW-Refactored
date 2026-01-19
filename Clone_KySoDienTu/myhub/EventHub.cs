using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data.Entity;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Clone_KySoDienTu.MyHub
{
    [Authorize]
    public class EventHub : Hub
    {
        private static ConcurrentDictionary<string, List<int>> _mapping = new ConcurrentDictionary<string, List<int>>();

        public override Task OnConnected()
        {
            //_mapping.TryAdd(Context.ConnectionId, new List<int>());
            //Clients.All.newConnection(Context.ConnectionId);
            //return base.OnConnected();
            string userName = "NoName";
            if (Context.User.Identity.IsAuthenticated)
                userName = Context.User.Identity.Name;
            //string name = Context.User.Identity.Name;
            Groups.Add(Context.ConnectionId, userName);
            return base.OnConnected();
            //return Clients.Caller.newConnection(Context.ConnectionId, userName);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = "NoName";
            if (Context.User.Identity.IsAuthenticated)
                userName = Context.User.Identity.Name;
            Groups.Remove(Context.ConnectionId, userName);
            
            return base.OnDisconnected(stopCalled);
        }
        //var list = new List<int>();
        //    _mapping.TryRemove(Context.ConnectionId, out list);
        //    Clients.All.removeConnection(Context.ConnectionId);
        //    return base.OnDisconnected(stopCalled);
        //}
    }
}