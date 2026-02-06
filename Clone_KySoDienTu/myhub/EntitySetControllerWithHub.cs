using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Clone_KySoDienTu.MyHub
{
    [EnableCors("*", "*", "*")]
    public class EntitySetControllerWithHub<THub> : ApiController
    where THub : IHub {
    Lazy<IHubContext> hub = new Lazy<IHubContext>(
      () => GlobalHost.ConnectionManager.GetHubContext<THub>()
    );

    protected IHubContext Hub {
      get { return hub.Value; }
    }
  }
}