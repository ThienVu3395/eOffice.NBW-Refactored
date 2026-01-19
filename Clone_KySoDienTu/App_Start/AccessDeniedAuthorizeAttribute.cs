using System.Web.Mvc;

namespace Clone_KySoDienTu.App_Start
{
    public class AccessDeniedAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectResult("~/Login/AuthLogin");
            }
        }
    }
}