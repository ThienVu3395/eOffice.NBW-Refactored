using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Web.Http;
using System.Web.Http.Cors;
//using System.Web.Http.OData;

namespace Clone_KySoDienTu.Controllers.API
{
    [EnableCors("*", "*", "*")]
    //public class EntitySetControllerWithHub<THub> : EntitySetController<Employee, int>
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