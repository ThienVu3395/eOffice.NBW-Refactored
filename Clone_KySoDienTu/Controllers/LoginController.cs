using System.Web.Mvc;

namespace Clone_KySoDienTu.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        [AllowAnonymous]
        public ActionResult AuthLogin()
        {
            return View();
        }
    }
}